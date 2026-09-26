using System;

namespace Kervan.Domain
{
    /// <summary>
    /// Kervanı temsil eden ana model.
    /// Bileşim (Composition) prensibiyle Inventory sınıfını bünyesinde barındırır.
    /// Mürettebat, binek hayvanları, maliye, erzak ve moral dinamiklerini yönetir.
    /// </summary>
    public class Caravan
    {
        // 1. Temel Bileşenler (Composition)
        public Inventory Cargo { get; }

        // 2. Maliye ve Kaynaklar
        public int Akce { get; private set; }
        public float FoodSupplyKg { get; private set; }
        public int Morale { get; private set; } // 0 - 100 arası

        // 3. Mürettebat
        public int MerchantCombatSkill { get; private set; } // Tüccarın savaşçılık meziyeti (1 - 10)
        public int GuardCount { get; private set; }          // Muhafız sayısı
        public int LaborerCount { get; private set; }        // Seyis & Kervancı sayısı

        // 4. Hayvan Sayıları
        public int CamelCount { get; private set; }
        public int MuleCount { get; private set; }
        public int HorseCount { get; private set; }

        // 5. Olaylar (Events)
        public event Action<int>? OnGoldChanged;
        public event Action<int>? OnMoraleChanged;
        public event Action<float>? OnFoodChanged;
        public event Action<string>? OnCaravanWarning;

        // Sabit Değerler (Katsayılar)
        private const float BASE_HUMAN_CAPACITY_KG = 50f;
        private const float CAMEL_CAPACITY_KG = 200f;
        private const float MULE_CAPACITY_KG = 120f;
        private const float HORSE_CAPACITY_KG = 80f;

        private const int GUARD_DAILY_WAGE = 3;   // Muhafız günlük ulufe (Akçe)
        private const int LABORER_DAILY_WAGE = 1; // Kervancı günlük yevmiye (Akçe)

        private const float HUMAN_DAILY_FOOD_KG = 1.0f;
        private const float ANIMAL_DAILY_FOOD_KG = 2.0f;

        public Caravan(int initialAkce, float initialFoodKg, int initialMerchantSkill = 2)
        {
            if (initialAkce < 0)
                throw new ArgumentOutOfRangeException(nameof(initialAkce), "Başlangıç akçesi negatif olamaz.");

            Akce = initialAkce;
            FoodSupplyKg = Math.Max(0, initialFoodKg);
            Morale = 100; // Başlangıç morali tam
            MerchantCombatSkill = Math.Clamp(initialMerchantSkill, 1, 10);

            // Başlangıçta 1 binek katır ve tüccarın kendi sırt çantasıyla başlar
            MuleCount = 1;
            CamelCount = 0;
            HorseCount = 0;
            GuardCount = 0;
            LaborerCount = 0;

            Cargo = new Inventory(CalculateTotalCapacity());
        }

        // --- KAPASİTE VE HESAPLANAN ÖZELLİKLER ---

        public int TotalPeople => 1 + GuardCount + LaborerCount; // 1: Tüccarın kendisi
        public int TotalAnimals => CamelCount + MuleCount + HorseCount;

        /// <summary>
        /// Kervanın toplam taşıma kapasitesini hayvan ve insan sayısına göre hesaplar.
        /// </summary>
        public float CalculateTotalCapacity()
        {
            return BASE_HUMAN_CAPACITY_KG
                + (CamelCount * CAMEL_CAPACITY_KG)
                + (MuleCount * MULE_CAPACITY_KG)
                + (HorseCount * HORSE_CAPACITY_KG);
        }

        /// <summary>
        /// Günlük toplam erzak (un, su, kurutulmuş et, yem) tüketimi (kg).
        /// </summary>
        public float DailyFoodConsumption =>
            (TotalPeople * HUMAN_DAILY_FOOD_KG) + (TotalAnimals * ANIMAL_DAILY_FOOD_KG);

        /// <summary>
        /// Günlük toplam muhafız ve işçi maaş gideri (Akçe).
        /// </summary>
        public int DailyWageExpense =>
            (GuardCount * GUARD_DAILY_WAGE) + (LaborerCount * LABORER_DAILY_WAGE);

        /// <summary>
        /// Kervanın toplam savunma ve savaş gücü (Haydut karşılaşmalarında kullanılır).
        /// </summary>
        public int TotalCombatPower =>
            (MerchantCombatSkill * 10) + (GuardCount * 15);

        /// <summary>
        /// Günlük seyahat hızı (km/gün). Yük doluluk oranı arttıkça hız düşer.
        /// </summary>
        public float CalculateDailySpeedKm()
        {
            const float baseSpeedKm = 25.0f; // Standart kervan hızı: günde 25 km
            float loadRatio = Cargo.MaxCapacityKg > 0 ? (Cargo.CurrentWeightKg / Cargo.MaxCapacityKg) : 0f;

            // Yük %100 doluysa hız %25'e kadar yavaşlar
            float loadPenalty = loadRatio * 0.25f;

            // At oranı fazlaysa hız biraz artar
            float horseBonus = (TotalAnimals > 0) ? ((float)HorseCount / TotalAnimals) * 0.15f : 0f;

            return baseSpeedKm * (1.0f - loadPenalty + horseBonus);
        }

        // --- MALİ VE KAYNAK YÖNETİMİ ---

        public void AddGold(int amount)
        {
            if (amount <= 0) return;
            Akce += amount;
            OnGoldChanged?.Invoke(Akce);
        }

        public bool TrySpendGold(int amount)
        {
            if (amount <= 0) return false;
            if (Akce < amount) return false;

            Akce -= amount;
            OnGoldChanged?.Invoke(Akce);
            return true;
        }

        public void AddFood(float amountKg)
        {
            if (amountKg <= 0) return;
            FoodSupplyKg += amountKg;
            OnFoodChanged?.Invoke(FoodSupplyKg);
        }

        public void SetMorale(int newMorale)
        {
            Morale = Math.Clamp(newMorale, 0, 100);
            OnMoraleChanged?.Invoke(Morale);
        }

        // --- MÜRETTEBAT VE BİNEK YÖNETİMİ ---

        public void HireGuard(int count = 1)
        {
            if (count <= 0) return;
            GuardCount += count;
        }

        public bool DismissGuard(int count = 1)
        {
            if (count <= 0 || GuardCount < count) return false;
            GuardCount -= count;
            return true;
        }

        public void HireLaborer(int count = 1)
        {
            if (count <= 0) return;
            LaborerCount += count;
        }

        public void AddMount(MountType mountType, int count = 1)
        {
            if (count <= 0) return;

            switch (mountType)
            {
                case MountType.Camel: CamelCount += count; break;
                case MountType.Mule: MuleCount += count; break;
                case MountType.Horse: HorseCount += count; break;
            }

            // Kapasiteyi güncelle
            Cargo.SetMaxCapacity(CalculateTotalCapacity());
        }

        // --- SEYAHAT SİMÜLASYONU VE GÜNLÜK TÜKETİM ---

        /// <summary>
        /// Yolda geçen 1 günlük tüketimi, maaş ödemesini ve moral etkilerini simüle eder.
        /// </summary>
        public void AdvanceOneDay()
        {
            // 1. Erzak Tüketimi
            float consumedFood = DailyFoodConsumption;
            if (FoodSupplyKg >= consumedFood)
            {
                FoodSupplyKg -= consumedFood;
            }
            else
            {
                FoodSupplyKg = 0f;
                // Açlık durumunda moral hızla çöker (-20)
                SetMorale(Morale - 20);
                OnCaravanWarning?.Invoke("Erzak tükendi! Kervan halkı aç ve moral hızla düşüyor!");
            }
            OnFoodChanged?.Invoke(FoodSupplyKg);

            // 2. Maaş / Ulufe Ödemesi
            int wage = DailyWageExpense;
            if (wage > 0)
            {
                if (Akce >= wage)
                {
                    Akce -= wage;
                    OnGoldChanged?.Invoke(Akce);
                }
                else
                {
                    // Maaş ödenemezse muhafızlar isyan eder, moral düşer
                    Akce = 0;
                    OnGoldChanged?.Invoke(Akce);
                    SetMorale(Morale - 15);
                    OnCaravanWarning?.Invoke("Ulufe ödenemedi! Muhafızlar huzursuz ve kervanı terk edebilir.");
                }
            }

            // 3. Doğal Moral Dengelenmesi (Açlık veya iflas yoksa moral yavaşça 50'ye doğru toparlar)
            if (FoodSupplyKg > 0 && Morale < 50)
            {
                SetMorale(Morale + 2);
            }
        }
    }
}
