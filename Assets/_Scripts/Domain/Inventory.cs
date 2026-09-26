using System;
using System.Collections.Generic;
using System.Linq;

namespace Kervan.Domain
{
    /// <summary>
    /// Kervanın veya bir tüccarın envanter/yük sistemini yöneten saf C# sınıfı.
    /// Kapsülleme (Encapsulation) ve Olay Odaklı (Event-Driven) mimari ile tasarlanmıştır.
    /// </summary>
    public class Inventory
    {
        // Ana veri yapısı: itemId -> InventoryItem eşleşmesi (Hızlı O(1) erişim için)
        private readonly Dictionary<string, InventoryItem> _items = new Dictionary<string, InventoryItem>();

        public float MaxCapacityKg { get; private set; }
        public float CurrentWeightKg { get; private set; }

        public float RemainingCapacityKg => Math.Max(0, MaxCapacityKg - CurrentWeightKg);
        public bool IsFull => CurrentWeightKg >= MaxCapacityKg;

        // Olaylar (Events): Arayüz veya diğer sistemler buraya abone olur, Inventory'nin UI'dan haberi olmaz!
        public event Action<Item, int>? OnItemAdded;
        public event Action<Item, int>? OnItemRemoved;
        public event Action<float, float>? OnWeightChanged; // (currentWeight, maxCapacity)

        public Inventory(float maxCapacityKg)
        {
            if (maxCapacityKg <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxCapacityKg), "Kapasite 0 veya negatif olamaz.");

            MaxCapacityKg = maxCapacityKg;
            CurrentWeightKg = 0f;
        }

        /// <summary>
        /// Kervanın taşıma kapasitesini artırır veya azaltır (Örn: Yeni deve/katır alındığında).
        /// </summary>
        public void SetMaxCapacity(float newCapacityKg)
        {
            if (newCapacityKg <= 0)
                throw new ArgumentOutOfRangeException(nameof(newCapacityKg), "Yeni kapasite pozitif olmalıdır.");

            MaxCapacityKg = newCapacityKg;
            OnWeightChanged?.Invoke(CurrentWeightKg, MaxCapacityKg);
        }

        /// <summary>
        /// Envantere belirli bir ağırlık sığıp sığmayacağını kontrol eder.
        /// </summary>
        public bool CanCarry(float additionalWeightKg)
        {
            return (CurrentWeightKg + additionalWeightKg) <= MaxCapacityKg;
        }

        /// <summary>
        /// Envantere eşya eklemeyi dener. Kapasite yeterliyse true döner ve ekler.
        /// </summary>
        public bool TryAddItem(Item item, int quantity)
        {
            if (item == null || quantity <= 0)
                return false;

            float addedWeight = item.WeightKg * quantity;

            if (!CanCarry(addedWeight))
                return false; // Kapasite aşıldı, eklenemez!

            if (_items.TryGetValue(item.Id, out var existingItem))
            {
                existingItem.AddQuantity(quantity);
            }
            else
            {
                _items[item.Id] = new InventoryItem(item, quantity);
            }

            CurrentWeightKg += addedWeight;

            // Olayları tetikle
            OnItemAdded?.Invoke(item, quantity);
            OnWeightChanged?.Invoke(CurrentWeightKg, MaxCapacityKg);

            return true;
        }

        /// <summary>
        /// Envanterden eşya çıkarmayı/satmayı dener. Yeterli stok varsa true döner.
        /// </summary>
        public bool TryRemoveItem(string itemId, int quantity)
        {
            if (string.IsNullOrWhiteSpace(itemId) || quantity <= 0)
                return false;

            if (!_items.TryGetValue(itemId, out var existingItem) || existingItem.Quantity < quantity)
                return false; // Eşya yok veya miktar yetersiz

            existingItem.RemoveQuantity(quantity);
            float removedWeight = existingItem.Item.WeightKg * quantity;
            CurrentWeightKg = Math.Max(0, CurrentWeightKg - removedWeight);

            var itemReference = existingItem.Item;

            // Eğer miktar 0 olduysa sözlükten tamamen kaldır
            if (existingItem.Quantity == 0)
            {
                _items.Remove(itemId);
            }

            // Olayları tetikle
            OnItemRemoved?.Invoke(itemReference, quantity);
            OnWeightChanged?.Invoke(CurrentWeightKg, MaxCapacityKg);

            return true;
        }

        /// <summary>
        /// Belirli bir eşyanın kaç adet olduğunu döndürür.
        /// </summary>
        public int GetQuantity(string itemId)
        {
            return _items.TryGetValue(itemId, out var item) ? item.Quantity : 0;
        }

        /// <summary>
        /// Tüm envanter içeriğini salt okunur (read-only) liste olarak döndürür.
        /// Dışarıdaki kodlar listeyi doğrudan bozamaz.
        /// </summary>
        public IReadOnlyCollection<InventoryItem> GetAllItems()
        {
            return _items.Values.ToList().AsReadOnly();
        }

        /// <summary>
        /// Envanterdeki tüm malların toplam taban değerini hesaplar.
        /// </summary>
        public int GetTotalBaseValue()
        {
            return _items.Values.Sum(item => item.TotalBaseValue);
        }
    }
}
