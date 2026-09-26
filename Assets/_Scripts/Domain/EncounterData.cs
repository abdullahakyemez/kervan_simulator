using System;

namespace Kervan.Domain
{
    /// <summary>
    /// Tetiklenen bir olayın veya tehdidin tüm sayısal ve anlatısal verisini taşır.
    /// </summary>
    public class EncounterData
    {
        public EncounterType Type { get; }
        public string Title { get; }
        public string Description { get; }
        public int ThreatPower { get; }      // Tehdit gücü (Muhafız gücüyle kıyaslanır, örn: 25 - 80)
        public int DemandedBribeAkce { get; } // Haydutların talep ettiği haraç / rüşvet
        public int PotentialLootAkce { get; } // Savaş kazanılırsa ele geçirilecek ganimet

        public EncounterData(
            EncounterType type,
            string title,
            string description,
            int threatPower,
            int demandedBribeAkce,
            int potentialLootAkce = 0)
        {
            Type = type;
            Title = title;
            Description = description;
            ThreatPower = threatPower;
            DemandedBribeAkce = demandedBribeAkce;
            PotentialLootAkce = potentialLootAkce;
        }

        public static EncounterData CreateBanditAmbush(int routeDangerPercent, System.Random rng)
        {
            int threat = 20 + (routeDangerPercent / 2) + rng.Next(5, 25);
            int bribe = Math.Max(15, threat * 2);
            int loot = threat * 3;

            return new EncounterData(
                EncounterType.BanditAmbush,
                "Aşiret Haydutları Pusu Kurdu!",
                "Dar bir vadi boğazında silahlı haydutlar yolu kesti. Reisleri haraç talep ediyor, aksi halde kervana saldıracaklar.",
                threat,
                bribe,
                loot
            );
        }

        public static EncounterData CreateWolfPack(System.Random rng)
        {
            int threat = rng.Next(15, 35);
            return new EncounterData(
                EncounterType.WolfPack,
                "Dağ Kurtları Sürüsü!",
                "Kış soğuğunda aç kalmış bir kurt sürüsü kervanın etrafını sardı, binek hayvanları huzursuz.",
                threat,
                demandedBribeAkce: 0,
                potentialLootAkce: 10 // Kurt postu
            );
        }
    }

    /// <summary>
    /// Bir karşılaşma stratejisi uygulandığında ortaya çıkan net sonucu temsil eder.
    /// </summary>
    public class EncounterResolutionResult
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        public int GoldChange { get; }
        public int MoraleChange { get; }
        public int LostGuardsCount { get; }

        public EncounterResolutionResult(
            bool isSuccess,
            string message,
            int goldChange = 0,
            int moraleChange = 0,
            int lostGuardsCount = 0)
        {
            IsSuccess = isSuccess;
            Message = message;
            GoldChange = goldChange;
            MoraleChange = moraleChange;
            LostGuardsCount = lostGuardsCount;
        }
    }
}
