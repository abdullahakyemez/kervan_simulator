using System;

namespace Kervan.Domain
{
    /// <summary>
    /// 14. yüzyılda Osmanlı Beyliği (Orhan Gazi / Osman Gazi) adına Anadolu'da icra edilen
    /// gizli istihbarat, ferman ulaştırma ve casusluk görevlerini temsil eden model.
    /// </summary>
    public class EspionageMission
    {
        public string MissionId { get; }
        public string Title { get; }
        public string Description { get; }
        public string TargetCityId { get; }
        public string DeliveryCityId { get; } // Bilginin ulaştırılacağı Osmanlı merkezi (örn: Bursa veya Söğüt)
        public int RewardAkce { get; }
        public int OttomanReputationGain { get; } // Osmanlı nüfuz puanı artışı

        public bool IsIntelCollected { get; private set; }
        public bool IsCompleted { get; private set; }

        public EspionageMission(
            string missionId,
            string title,
            string description,
            string targetCityId,
            string deliveryCityId,
            int rewardAkce,
            int reputationGain = 10)
        {
            MissionId = missionId;
            Title = title;
            Description = description;
            TargetCityId = targetCityId;
            DeliveryCityId = deliveryCityId;
            RewardAkce = rewardAkce;
            OttomanReputationGain = reputationGain;
            IsIntelCollected = false;
            IsCompleted = false;
        }

        public void MarkIntelCollected()
        {
            IsIntelCollected = true;
        }

        public void CompleteMission()
        {
            IsCompleted = true;
        }
    }
}
