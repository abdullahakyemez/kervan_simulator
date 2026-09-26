using System;

namespace Kervan.Domain
{
    /// <summary>
    /// Strateji Tasarım Deseni (Strategy Pattern) arayüzü.
    /// Farklı karşılaşma çözüm yolları (Savaş, Rüşvet, Kaçış, İkna) bu arayüzü uygular.
    /// </summary>
    public interface IEncounterStrategy
    {
        string StrategyName { get; }
        EncounterResolutionResult Execute(Caravan caravan, EncounterData encounter, System.Random rng);
    }

    /// <summary>
    /// Savaş Stratejisi: Kervanın muhafız ve tüccar gücünü tehdit gücüyle karşılaştırır.
    /// </summary>
    public class CombatStrategy : IEncounterStrategy
    {
        public string StrategyName => "Kılıç Çekip Savaş";

        public EncounterResolutionResult Execute(Caravan caravan, EncounterData encounter, System.Random rng)
        {
            if (caravan == null || encounter == null)
                return new EncounterResolutionResult(false, "Geçersiz parametre.");

            // Zar faktörü (Zar: 0.85 - 1.25 arası)
            float diceRoll = 0.85f + (float)rng.NextDouble() * 0.40f;
            float totalPlayerPower = caravan.TotalCombatPower * diceRoll;

            if (totalPlayerPower >= encounter.ThreatPower)
            {
                // Zafer!
                int loot = encounter.PotentialLootAkce;
                caravan.AddGold(loot);
                caravan.SetMorale(caravan.Morale + 10);

                return new EncounterResolutionResult(
                    isSuccess: true,
                    message: $"Düşmanı püskürttünüz! Cesaretiniz kervanın moralini yükseltti. {loot} Akçe ganimet ele geçirildi.",
                    goldChange: loot,
                    moraleChange: +10,
                    lostGuardsCount: 0
                );
            }
            else
            {
                // Yenilgi / Hasar
                int lostGuard = 0;
                if (caravan.GuardCount > 0 && rng.NextDouble() < 0.60)
                {
                    caravan.DismissGuard(1);
                    lostGuard = 1;
                }

                int moraleLoss = -25;
                caravan.SetMorale(caravan.Morale + moraleLoss);

                return new EncounterResolutionResult(
                    isSuccess: false,
                    message: $"Ağır bir çatışma yaşandı. Kervan geri çekilmek zorunda kaldı, moral çöktü{(lostGuard > 0 ? " ve 1 muhafızınız şehit düştü" : "")}!",
                    goldChange: 0,
                    moraleChange: moraleLoss,
                    lostGuardsCount: lostGuard
                );
            }
        }
    }

    /// <summary>
    /// Rüşvet / Haraç Verme Stratejisi: Çatışmadan kaçınıp canı kurtarır ama para kaybettirir.
    /// </summary>
    public class BribeStrategy : IEncounterStrategy
    {
        public string StrategyName => "Haraç Verip Anlaş";

        public EncounterResolutionResult Execute(Caravan caravan, EncounterData encounter, System.Random rng)
        {
            int demand = encounter.DemandedBribeAkce;

            if (caravan.Akce >= demand)
            {
                caravan.TrySpendGold(demand);
                caravan.SetMorale(caravan.Morale - 5); // Hafif gurur/moral kaybı

                return new EncounterResolutionResult(
                    isSuccess: true,
                    message: $"{demand} Akçe haraç ödenerek haydutların yolu açması sağlandı. Kimsenin canı yanmadı.",
                    goldChange: -demand,
                    moraleChange: -5
                );
            }
            else
            {
                // Parası yetmeyen tüccar haraç veremez, savaşmak zorunda kalır!
                return new EncounterResolutionResult(
                    isSuccess: false,
                    message: "Haydutların istediği haracı ödeyecek yeterli akçeniz yok! Çatışma kaçınılmaz hale geldi.",
                    goldChange: 0,
                    moraleChange: -10
                );
            }
        }
    }

    /// <summary>
    /// Kaçış Stratejisi: Kervanın hafifliğine ve at sayısına göre başarı şansı hesaplar.
    /// </summary>
    public class FleeStrategy : IEncounterStrategy
    {
        public string StrategyName => "Hızla Kaç";

        public EncounterResolutionResult Execute(Caravan caravan, EncounterData encounter, System.Random rng)
        {
            float speed = caravan.CalculateDailySpeedKm();
            // Yüksek hız ve at sayısı kaçış şansını artırır (Temel şans %45)
            float fleeChance = 0.45f + (caravan.HorseCount * 0.10f) + (speed > 22f ? 0.15f : 0f);

            if (rng.NextDouble() < fleeChance)
            {
                caravan.SetMorale(caravan.Morale - 5);
                return new EncounterResolutionResult(
                    isSuccess: true,
                    message: "Atların ve yük hayvanlarının dizginlerine asılarak pusudan hasarsız sıyrıldınız!",
                    goldChange: 0,
                    moraleChange: -5
                );
            }
            else
            {
                // Kaçarken yakalanma: Yük hasarı ve moral çöküşü
                caravan.SetMorale(caravan.Morale - 15);
                return new EncounterResolutionResult(
                    isSuccess: false,
                    message: "Kaçamadınız! Haydutlar kervanın önünü kesti, panik ve kargaşa yaşandı.",
                    goldChange: 0,
                    moraleChange: -15
                );
            }
        }
    }

    /// <summary>
    /// İkna ve Nüfuz Stratejisi: Tüccarın tecrübesine ve Osmanlı mensubiyetine güvenerek ikna etmeyi dener.
    /// </summary>
    public class NegotiateStrategy : IEncounterStrategy
    {
        public string StrategyName => "Osmanlı Gazisi Kimliğiyle İkna Et";

        public EncounterResolutionResult Execute(Caravan caravan, EncounterData encounter, System.Random rng)
        {
            // Tüccarın savaş meziyeti karizma ve caydırıcılık da sağlar
            float negotiateChance = 0.25f + (caravan.MerchantCombatSkill * 0.07f);

            if (rng.NextDouble() < negotiateChance)
            {
                caravan.SetMorale(caravan.Morale + 5);
                return new EncounterResolutionResult(
                    isSuccess: true,
                    message: "Tüccarın vakur duruşu ve Osmanlı Beyliği'nin fermanını göstermesi üzerine haydut reisi geri adım attı ve yolu açtı.",
                    goldChange: 0,
                    moraleChange: +5
                );
            }
            else
            {
                return new EncounterResolutionResult(
                    isSuccess: false,
                    message: "Haydutlar söz dinlemedi ve alay etti! İkna çabası boşa çıktı.",
                    goldChange: 0,
                    moraleChange: -5
                );
            }
        }
    }
}
