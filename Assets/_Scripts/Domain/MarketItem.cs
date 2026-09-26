using System;

namespace Kervan.Domain
{
    /// <summary>
    /// Bir şehrin pazarında yer alan belirli bir malın stok adedini ve o şehre özel arz-talep katsayısını tutar.
    /// </summary>
    public class MarketItem
    {
        public Item Item { get; }
        public int Stock { get; private set; }
        
        /// <summary>
        /// Arz-Talep Çarpanı:
        /// < 1.0 : Şehir bu malı üretiyor / bolluk var (Örn: Bursa'da İpek 0.6 -> %40 ucuz)
        /// > 1.0 : Şehirde kıtlık / yüksek talep var (Örn: Konya'da Deniz Tuzu 1.8 -> %80 pahalı)
        /// </summary>
        public float Multiplier { get; private set; }

        public MarketItem(Item item, int initialStock, float initialMultiplier = 1.0f)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));
            Stock = Math.Max(0, initialStock);
            Multiplier = Math.Clamp(initialMultiplier, 0.3f, 3.5f);
        }

        public void AddStock(int quantity)
        {
            if (quantity <= 0) return;
            Stock += quantity;
        }

        public bool TryDeductStock(int quantity)
        {
            if (quantity <= 0 || Stock < quantity) return false;
            Stock -= quantity;
            return true;
        }

        public void SetMultiplier(float newMultiplier)
        {
            Multiplier = Math.Clamp(newMultiplier, 0.3f, 3.5f);
        }
    }
}
