using System;

namespace Kervan.Domain
{
    /// <summary>
    /// Oyundaki bir ticaret malının temel özelliklerini temsil eden saf C# (POCO) sınıfı.
    /// Değişmezlik (Immutability) ilkesi uygulanmıştır: Nesne oluşturulduktan sonra özellikleri dışarıdan değiştirilemez.
    /// </summary>
    public class Item
    {
        public string Id { get; }
        public string Name { get; }
        public ItemCategory Category { get; }
        public int BasePrice { get; }          // Akçe cinsinden taban değer
        public float WeightKg { get; }         // Birim başına ağırlık (kg)
        public bool IsPerishable { get; }      // Bozulabilir mi? (Gıda vb.)
        public int ShelfLifeDays { get; }      // Raf ömrü (Gün cinsinden, bozulmayan mallarda 0)

        public Item(
            string id,
            string name,
            ItemCategory category,
            int basePrice,
            float weightKg,
            bool isPerishable = false,
            int shelfLifeDays = 0)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Eşya ID'si boş olamaz.", nameof(id));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Eşya adı boş olamaz.", nameof(name));

            if (basePrice < 0)
                throw new ArgumentOutOfRangeException(nameof(basePrice), "Taban fiyat negatif olamaz.");

            if (weightKg <= 0)
                throw new ArgumentOutOfRangeException(nameof(weightKg), "Ağırlık sıfır veya negatif olamaz.");

            Id = id;
            Name = name;
            Category = category;
            BasePrice = basePrice;
            WeightKg = weightKg;
            IsPerishable = isPerishable;
            ShelfLifeDays = shelfLifeDays;
        }

        public override string ToString()
        {
            return $"{Name} ({Category}) - {BasePrice} Akçe, {WeightKg} kg";
        }
    }
}
