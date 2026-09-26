using System;

namespace Kervan.Domain
{
    /// <summary>
    /// Ticaret ve fiyat hesaplama formüllerini içeren saf C# statik yardımcı sınıfı.
    /// static class: Bellekte tek bir örneği bulunmaz, new yapılamaz, doğrudan metotları çağrılır.
    /// </summary>
    public static class TradeCalculator
    {
        // Pazar kâr marjı (Spread): Tüccar satarken pazarın kestiği standart komisyon oranı (%15)
        public const float MARKET_SPREAD_MARGIN = 0.15f;

        /// <summary>
        /// Tüccarın pazardan mal SATIN ALIRKEN ödeyeceği birim fiyatı hesaplar.
        /// Fiyat = TabanFiyat * ArzTalepÇarpanı * (1 + VergiOranı)
        /// </summary>
        public static int CalculateBuyPrice(Item item, float supplyDemandMultiplier, float taxRate = 0.05f)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            float rawPrice = item.BasePrice * supplyDemandMultiplier * (1.0f + taxRate);

            // Fiyat en az 1 Akçe olmalıdır
            return Math.Max(1, (int)Math.Round(rawPrice));
        }

        /// <summary>
        /// Tüccarın pazara mal SATARKEN eline geçecek birim akçeyi hesaplar.
        /// Satış fiyatı pazar marjı kadar düşüktür ancak tüccarın karizması/itibarı bu marjı azaltabilir.
        /// </summary>
        public static int CalculateSellPrice(Item item, float supplyDemandMultiplier, float merchantDiscountRate = 0f)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            // Efektif marj: %15 pazar kesintisi tüccarın indirimiyle düşebilir
            float effectiveMargin = Math.Max(0.05f, MARKET_SPREAD_MARGIN - merchantDiscountRate);
            float rawPrice = item.BasePrice * supplyDemandMultiplier * (1.0f - effectiveMargin);

            return Math.Max(1, (int)Math.Round(rawPrice));
        }

        /// <summary>
        /// İki şehir koordinatı arasındaki kuş uçuşu mesafeyi (km) Pisagor teoremiyle hesaplar.
        /// </summary>
        public static float CalculateDistanceKm(float x1, float y1, float x2, float y2)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;
            return (float)Math.Sqrt((dx * dx) + (dy * dy));
        }
    }
}
