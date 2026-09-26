using System;

namespace Kervan.Domain
{
    /// <summary>
    /// Envanterdeki belirli bir eşyayı ve onun adet/stok miktarını tutan veri modeli.
    /// </summary>
    public class InventoryItem
    {
        public Item Item { get; }
        public int Quantity { get; private set; }

        public float TotalWeightKg => Item.WeightKg * Quantity;
        public int TotalBaseValue => Item.BasePrice * Quantity;

        public InventoryItem(Item item, int initialQuantity)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));

            if (initialQuantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(initialQuantity), "Başlangıç miktarı pozitif olmalıdır.");

            Quantity = initialQuantity;
        }

        public void AddQuantity(int amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Eklenecek miktar 0'dan büyük olmalıdır.");

            Quantity += amount;
        }

        public void RemoveQuantity(int amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Çıkarılacak miktar 0'dan büyük olmalıdır.");

            if (amount > Quantity)
                throw new InvalidOperationException($"Yetersiz stok. Mevcut: {Quantity}, İstenen: {amount}");

            Quantity -= amount;
        }
    }
}
