using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Kervan.Domain;

namespace Kervan.Presentation
{
    /// <summary>
    /// Pazar listesindeki tek bir ticaret malının satırını / kartını yöneten UI kontrolcüsü.
    /// Alış ve satış butonlarını kervanın anlık parasına ve taşıma kapasitesine göre etkinleştirir/devre dışı bırakır.
    /// </summary>
    public class MarketItemEntryUI : MonoBehaviour
    {
        [Header("Görsel ve Metinler")]
        [SerializeField] private Image? _itemIcon;
        [SerializeField] private TextMeshProUGUI? _nameText;
        [SerializeField] private TextMeshProUGUI? _categoryText;
        [SerializeField] private TextMeshProUGUI? _buyPriceText;
        [SerializeField] private TextMeshProUGUI? _sellPriceText;
        [SerializeField] private TextMeshProUGUI? _marketStockText;
        [SerializeField] private TextMeshProUGUI? _caravanAmountText;

        [Header("Etkileşim Butonları")]
        [SerializeField] private Button? _buyOneButton;
        [SerializeField] private Button? _buyFiveButton;
        [SerializeField] private Button? _sellOneButton;
        [SerializeField] private Button? _sellAllButton;

        public MarketItem? BoundMarketItem { get; private set; }

        private Action<MarketItem, int>? _onBuyRequested;
        private Action<MarketItem, int>? _onSellRequested;

        /// <summary>
        /// Satırı belirli bir pazar malı ile bağlar ve buton tıklamalarını dinler.
        /// </summary>
        public void Bind(
            MarketItem marketItem,
            Caravan caravan,
            int buyPrice,
            int sellPrice,
            Action<MarketItem, int> onBuyRequested,
            Action<MarketItem, int> onSellRequested,
            Sprite? fallbackIcon = null)
        {
            BoundMarketItem = marketItem;
            _onBuyRequested = onBuyRequested;
            _onSellRequested = onSellRequested;

            if (_itemIcon != null && fallbackIcon != null)
            {
                _itemIcon.sprite = fallbackIcon;
            }

            if (_nameText != null) _nameText.text = marketItem.Item.Name;
            if (_categoryText != null) _categoryText.text = $"{marketItem.Item.Category} ({marketItem.Item.WeightKg:F1} kg)";
            if (_buyPriceText != null) _buyPriceText.text = $"Alış: {buyPrice} Akçe";
            if (_sellPriceText != null) _sellPriceText.text = $"Satış: {sellPrice} Akçe";
            if (_marketStockText != null) _marketStockText.text = $"Pazar: {marketItem.Stock} adet";

            int caravanStock = caravan.Cargo.GetQuantity(marketItem.Item.Id);
            if (_caravanAmountText != null) _caravanAmountText.text = $"Kervan: {caravanStock} adet";

            // Buton Olaylarını Bağla
            SetupButtons(caravan, buyPrice, caravanStock);
        }

        private void SetupButtons(Caravan caravan, int unitBuyPrice, int caravanStock)
        {
            if (BoundMarketItem == null) return;

            // 1. Alış Kontrolleri: Para yetiyor mu? Kapasite var mı? Pazar stoğu var mı?
            float singleWeight = BoundMarketItem.Item.WeightKg;
            bool canBuyOne = BoundMarketItem.Stock >= 1
                && caravan.Akce >= unitBuyPrice
                && caravan.Cargo.CanCarry(singleWeight);

            bool canBuyFive = BoundMarketItem.Stock >= 5
                && caravan.Akce >= (unitBuyPrice * 5)
                && caravan.Cargo.CanCarry(singleWeight * 5);

            if (_buyOneButton != null)
            {
                _buyOneButton.interactable = canBuyOne;
                _buyOneButton.onClick.RemoveAllListeners();
                _buyOneButton.onClick.AddListener(() => _onBuyRequested?.Invoke(BoundMarketItem, 1));
            }

            if (_buyFiveButton != null)
            {
                _buyFiveButton.interactable = canBuyFive;
                _buyFiveButton.onClick.RemoveAllListeners();
                _buyFiveButton.onClick.AddListener(() => _onBuyRequested?.Invoke(BoundMarketItem, 5));
            }

            // 2. Satış Kontrolleri: Kervanda bu maldan var mı?
            bool canSellOne = caravanStock >= 1;
            bool canSellAll = caravanStock > 0;

            if (_sellOneButton != null)
            {
                _sellOneButton.interactable = canSellOne;
                _sellOneButton.onClick.RemoveAllListeners();
                _sellOneButton.onClick.AddListener(() => _onSellRequested?.Invoke(BoundMarketItem, 1));
            }

            if (_sellAllButton != null)
            {
                _sellAllButton.interactable = canSellAll;
                _sellAllButton.onClick.RemoveAllListeners();
                _sellAllButton.onClick.AddListener(() => _onSellRequested?.Invoke(BoundMarketItem, caravanStock));
            }
        }
    }
}
