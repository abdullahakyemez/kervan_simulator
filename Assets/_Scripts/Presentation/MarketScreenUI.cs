using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Kervan.Domain;
using Kervan.Services;
using Kervan.Data;

namespace Kervan.Presentation
{
    /// <summary>
    /// Şehir pazarının ana arayüz ekranını yöneten sunum kontrolcüsü.
    /// Şehirdeki tüm ticaret mallarını dinamik olarak listeler ve kervan alım-satım işlemlerini yürütür.
    /// </summary>
    public class MarketScreenUI : MonoBehaviour
    {
        [Header("UI Referansları")]
        [SerializeField] private TextMeshProUGUI? _bazaarHeaderTitle;
        [SerializeField] private TextMeshProUGUI? _bazaarSubtitle;
        [SerializeField] private Transform? _itemsContainer;
        [SerializeField] private MarketItemEntryUI? _itemEntryPrefab;

        [Header("Veritabanı İkon Eşleme (İsteğe Bağlı)")]
        [SerializeField] private ItemDatabaseSO? _itemDatabase;

        // Havuzda tutulan satır bileşenleri (Object Pooling mantığıyla gereksiz Instantiate önlenir)
        private readonly List<MarketItemEntryUI> _activeEntries = new List<MarketItemEntryUI>();

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnCityChanged += HandleCityChanged;
                // İlk açılışta mevcut şehri yükle
                RefreshMarketView();
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnCityChanged -= HandleCityChanged;
            }
        }

        private void OnEnable()
        {
            RefreshMarketView();
        }

        private void HandleCityChanged(City newCity)
        {
            RefreshMarketView();
        }

        /// <summary>
        /// O an bulunulan şehrin pazarındaki tüm malları arayüze döker.
        /// </summary>
        public void RefreshMarketView()
        {
            if (GameManager.Instance == null || GameManager.Instance.CurrentCity == null)
                return;

            var city = GameManager.Instance.CurrentCity;
            var caravan = GameManager.Instance.PlayerCaravan;

            if (_bazaarHeaderTitle != null)
                _bazaarHeaderTitle.text = $"{city.Name} Bedesteni & Çarşısı";

            if (_bazaarSubtitle != null)
                _bazaarSubtitle.text = $"{city.RulingFaction} Hükmü Altında • Vergi Oranı: %{city.LocalTaxRate * 100:F0}";

            if (_itemsContainer == null || _itemEntryPrefab == null)
                return;

            var goods = city.LocalMarket.GetAllGoods();
            int requiredCount = goods.Count;

            // 1. Gerekirse yeni prefab satırları üret
            while (_activeEntries.Count < requiredCount)
            {
                var newEntry = Instantiate(_itemEntryPrefab, _itemsContainer);
                _activeEntries.Add(newEntry);
            }

            // 2. Fazla satırları gizle
            for (int i = 0; i < _activeEntries.Count; i++)
            {
                _activeEntries[i].gameObject.SetActive(i < requiredCount);
            }

            // 3. Satırları verilerle bağla
            int index = 0;
            foreach (var marketItem in goods)
            {
                var entryUI = _activeEntries[index];
                int buyPrice = city.LocalMarket.GetBuyPrice(marketItem.Item.Id, city.LocalTaxRate);
                int sellPrice = city.LocalMarket.GetSellPrice(marketItem.Item.Id);

                Sprite? itemIcon = _itemDatabase?.GetItemById(marketItem.Item.Id)?.Icon;

                entryUI.Bind(
                    marketItem,
                    caravan,
                    buyPrice,
                    sellPrice,
                    onBuyRequested: HandleBuyRequest,
                    onSellRequested: HandleSellRequest,
                    fallbackIcon: itemIcon
                );

                index++;
            }
        }

        private void HandleBuyRequest(MarketItem marketItem, int quantity)
        {
            if (GameManager.Instance == null) return;

            var city = GameManager.Instance.CurrentCity;
            var caravan = GameManager.Instance.PlayerCaravan;

            bool success = city.LocalMarket.TryBuy(caravan, marketItem.Item.Id, quantity, city.LocalTaxRate);

            if (success)
            {
                RefreshMarketView();
            }
        }

        private void HandleSellRequest(MarketItem marketItem, int quantity)
        {
            if (GameManager.Instance == null) return;

            var city = GameManager.Instance.CurrentCity;
            var caravan = GameManager.Instance.PlayerCaravan;

            bool success = city.LocalMarket.TrySell(caravan, marketItem.Item.Id, quantity);

            if (success)
            {
                RefreshMarketView();
            }
        }
    }
}
