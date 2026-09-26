using System;

namespace Kervan.Domain
{
    /// <summary>
    /// İki şehir arasındaki belirli bir seyahat rotasını temsil eden saf C# sınıfı.
    /// Oyuncuya "Güvenli ama uzun yol" veya "Kestirme ama tehlikeli patika" gibi stratejik tercihler sunar.
    /// </summary>
    public class MapRoute
    {
        public string RouteId { get; }
        public string RouteName { get; }
        public string FromCityId { get; }
        public string ToCityId { get; }
        public TerrainType Terrain { get; }
        public float DistanceKm { get; }
        public float DangerFactor { get; } // 0.0f (Tamamen güvenli) - 1.0f (Ölümcül tehlike)
        public int TollAkce { get; }       // Geçiş vergisi (Derbent / Köprü masrafı)

        public MapRoute(
            string routeId,
            string routeName,
            string fromCityId,
            string toCityId,
            TerrainType terrain,
            float distanceKm,
            float dangerFactor,
            int tollAkce = 0)
        {
            if (string.IsNullOrWhiteSpace(routeId)) throw new ArgumentException("Rota ID boş olamaz.", nameof(routeId));
            if (string.IsNullOrWhiteSpace(fromCityId)) throw new ArgumentException("Çıkış şehri boş olamaz.", nameof(fromCityId));
            if (string.IsNullOrWhiteSpace(toCityId)) throw new ArgumentException("Varış şehri boş olamaz.", nameof(toCityId));
            if (distanceKm <= 0) throw new ArgumentOutOfRangeException(nameof(distanceKm), "Mesafe 0 veya negatif olamaz.");

            RouteId = routeId;
            RouteName = routeName;
            FromCityId = fromCityId;
            ToCityId = toCityId;
            Terrain = terrain;
            DistanceKm = distanceKm;
            DangerFactor = Math.Clamp(dangerFactor, 0.0f, 1.0f);
            TollAkce = Math.Max(0, tollAkce);
        }

        /// <summary>
        /// Kervanın mevcut hızına ve arazi türüne göre bu rotanın kaç gün süreceğini hesaplar.
        /// </summary>
        public int CalculateEstimatedDays(Caravan caravan)
        {
            if (caravan == null) return 1;

            float baseSpeed = caravan.CalculateDailySpeedKm();

            // Araziye göre hız katsayısı
            float terrainSpeedModifier = Terrain switch
            {
                TerrainType.PavedRoad => 1.15f,     // Taş yolda %15 daha hızlı
                TerrainType.Steppe => 1.0f,         // Bozkırda standart
                TerrainType.Valley => 0.95f,        // Vadide hafif yavaş
                TerrainType.Forest => 0.85f,        // Ormanda yavaş
                TerrainType.MountainPass => 0.70f,  // Dağ geçidinde %30 daha yavaş
                _ => 1.0f
            };

            // Eğer kervanda dağda katır veya bozkırda deve varsa avantaj kazanır
            if (Terrain == TerrainType.MountainPass && caravan.MuleCount > 0)
            {
                terrainSpeedModifier += 0.10f; // Katırlar dağ hızını artırır
            }
            else if (Terrain == TerrainType.Steppe && caravan.CamelCount > 0)
            {
                terrainSpeedModifier += 0.10f; // Develer bozkır hızını artırır
            }

            float effectiveDailySpeed = Math.Max(5f, baseSpeed * terrainSpeedModifier);
            return (int)Math.Max(1, Math.Ceiling(DistanceKm / effectiveDailySpeed));
        }

        public override string ToString()
        {
            return $"{RouteName} ({FromCityId} -> {ToCityId}) [{Terrain}, {DistanceKm} km, Tehlike: %{DangerFactor * 100:F0}]";
        }
    }
}
