using System;

namespace Kervan.Domain
{
    /// <summary>
    /// 14. yüzyıl Anadolu'sundaki bir şehri, beylik hakimiyetini ve yerel pazarını temsil eden saf C# sınıfı.
    /// </summary>
    public class City
    {
        public string Id { get; }
        public string Name { get; }
        public string RulingFaction { get; private set; } // Örn: "Osmanlı Beyliği", "Bizans Tekfurluğu", "Karamanoğulları"
        public float CoordX { get; }
        public float CoordY { get; }
        public float LocalTaxRate { get; private set; }   // Yerel pazar vergisi oranı (Örn: 0.05 = %5)
        public Market LocalMarket { get; }

        // İstihbarat & Görev Sistemi Dinamikleri
        public bool HasEspionageIntel { get; private set; }
        public int IntelValueAkce { get; private set; }

        public City(
            string id,
            string name,
            string rulingFaction,
            float coordX,
            float coordY,
            float localTaxRate = 0.05f)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Şehir ID boş olamaz.", nameof(id));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Şehir adı boş olamaz.", nameof(name));

            Id = id;
            Name = name;
            RulingFaction = rulingFaction;
            CoordX = coordX;
            CoordY = coordY;
            LocalTaxRate = Math.Clamp(localTaxRate, 0f, 0.30f); // En fazla %30 vergi
            LocalMarket = new Market();
            HasEspionageIntel = false;
            IntelValueAkce = 0;
        }

        public void SetRulingFaction(string newFaction)
        {
            if (!string.IsNullOrWhiteSpace(newFaction))
                RulingFaction = newFaction;
        }

        public void SetTaxRate(float newRate)
        {
            LocalTaxRate = Math.Clamp(newRate, 0f, 0.30f);
        }

        /// <summary>
        /// Şehirde Osmanlı adına toplanabilecek gizli bir istihbarat oluşturur (Askeri yığınak, isyan planı vb.).
        /// </summary>
        public void PlantEspionageIntel(int valueAkce)
        {
            HasEspionageIntel = true;
            IntelValueAkce = valueAkce;
        }

        /// <summary>
        /// Tüccar şehirde bilgi topladığında istihbaratı teslim alır.
        /// </summary>
        public int CollectEspionageIntel()
        {
            if (!HasEspionageIntel) return 0;

            int collectedValue = IntelValueAkce;
            HasEspionageIntel = false;
            IntelValueAkce = 0;
            return collectedValue;
        }

        /// <summary>
        /// Başka bir şehre olan mesafeyi km cinsinden hesaplar.
        /// </summary>
        public float DistanceTo(City otherCity)
        {
            if (otherCity == null) return 0f;
            return TradeCalculator.CalculateDistanceKm(CoordX, CoordY, otherCity.CoordX, otherCity.CoordY);
        }

        public override string ToString()
        {
            return $"{Name} ({RulingFaction})";
        }
    }
}
