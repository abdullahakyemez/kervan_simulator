using System;
using System.Collections.Generic;
using System.Linq;

namespace Kervan.Domain
{
    /// <summary>
    /// Bir şehrin çarşısını / pazarını temsil eden sınıf.
    /// Kervan ile pazar arasındaki güvenli alım-satım işlemlerini denetler.
    /// </summary>
    public class Market
    {
        private readonly Dictionary<string, MarketItem> _items = new Dictionary<string, MarketItem>();

        public event Action<Item, int, int, bool>? OnTransactionCompleted; // (Item, quantity, totalGold, isBuy)

        public void AddOrUpdateGood(Item item, int stock, float multiplier)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            if (_items.TryGetValue(item.Id, out var existing))
            {
                existing.AddStock(stock);
                existing.SetMultiplier(multiplier);
            }
            else
            {
                _items[item.Id] = new MarketItem(item, stock, multiplier);
            }
        }

        public MarketItem? GetMarketItem(string itemId)
        {
            return _items.TryGetValue(itemId, out var item) ? item : null;
        }

        public IReadOnlyCollection<MarketItem> GetAllGoods()
        {
            return _items.Values.ToList().AsReadOnly();
        }

        /// <summary>
        /// Pazardan 1 adet mal satın alma fiyatını hesaplar.
        /// </summary>
        public int GetBuyPrice(string itemId, float taxRate = 0.05f)
        {
            if (!_items.TryGetValue(itemId, out var marketItem)) return 0;
            return TradeCalculator.CalculateBuyPrice(marketItem.Item, marketItem.Multiplier, taxRate);
        }

        /// <summary>
        /// Pazara 1 adet mal satma fiyatını hesaplar.
        /// </summary>
        public int GetSellPrice(string itemId, float merchantDiscount = 0f)
        {
            if (!_items.TryGetValue(itemId, out var marketItem)) return 0;
            return TradeCalculator.CalculateSellPrice(marketItem.Item, marketItem.Multiplier, merchantDiscount);
        }

        /// <summary>
        /// Kervanın pazardan mal satın almasını sağlar.
        /// Kontroller: Stok var mı? Para yetiyor mu? Kervanda taşıma kapasitesi var mı?
        /// </summary>
        public bool TryBuy(Caravan caravan, string itemId, int quantity, float taxRate = 0.05f)
        {
            if (caravan == null || string.IsNullOrWhiteSpace(itemId) || quantity <= 0)
                return false;

            if (!_items.TryGetValue(itemId, out var marketItem) || marketItem.Stock < quantity)
                return false; // Yetersiz pazar stoğu

            int unitPrice = GetBuyPrice(itemId, taxRate);
            int totalCost = unitPrice * quantity;

            // 1. Akçe kontrolü
            if (caravan.Akce < totalCost)
                return false;

            // 2. Kervan taşıma kapasitesi kontrolü
            float addedWeight = marketItem.Item.WeightKg * quantity;
            if (!caravan.Cargo.CanCarry(addedWeight))
                return false;

            // İşlemi gerçekleştir
            if (!caravan.TrySpendGold(totalCost))
                return false;

            if (!caravan.Cargo.TryAddItem(marketItem.Item, quantity))
            {
                // Bir aksilik olursa parayı iade et (Rollback)
                caravan.AddGold(totalCost);
                return false;
            }

            marketItem.TryDeductStock(quantity);

            OnTransactionCompleted?.Invoke(marketItem.Item, quantity, totalCost, true);
            return true;
        }

        /// <summary>
        /// Kervanın elindeki malı pazara satmasını sağlar.
        /// Kontroller: Kervanda bu maldan yeterli adet var mı?
        /// </summary>
        public bool TrySell(Caravan caravan, string itemId, int quantity, float merchantDiscount = 0f)
        {
            if (caravan == null || string.IsNullOrWhiteSpace(itemId) || quantity <= 0)
                return false;

            if (caravan.Cargo.GetQuantity(itemId) < quantity)
                return false; // Kervanda yeterli mal yok

            if (!_items.TryGetValue(itemId, out var marketItem))
                return false;

            int unitPrice = GetSellPrice(itemId, merchantDiscount);
            int totalEarning = unitPrice * quantity;

            // İşlemi gerçekleştir
            if (!caravan.Cargo.TryRemoveItem(itemId, quantity))
                return false;

            caravan.AddGold(totalEarning);
            marketItem.AddStock(quantity);

            OnTransactionCompleted?.Invoke(marketItem.Item, quantity, totalEarning, false);
            return true;
        }
    }
}
