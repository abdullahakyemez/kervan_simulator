using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Kervan.Domain;

namespace Kervan.Services
{
    /// <summary>
    /// Osmanlı Beyliği adına yürütülen istihbarat, casusluk ve gizli ferman görevlerini
    /// yöneten servis.
    /// </summary>
    public class EspionageService : MonoBehaviour
    {
        public static EspionageService Instance { get; private set; } = null!;

        public int OttomanReputation { get; private set; } = 50; // Başlangıç nüfuz puanı

        private readonly List<EspionageMission> _activeMissions = new List<EspionageMission>();
        private readonly List<EspionageMission> _availableMissions = new List<EspionageMission>();

        public IReadOnlyList<EspionageMission> ActiveMissions => _activeMissions.AsReadOnly();
        public IReadOnlyList<EspionageMission> AvailableMissions => _availableMissions.AsReadOnly();

        public event Action<int>? OnReputationChanged;
        public event Action<EspionageMission>? OnMissionAccepted;
        public event Action<EspionageMission>? OnIntelCollected;
        public event Action<EspionageMission>? OnMissionCompleted;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            SeedStartingMissions();
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnCityChanged += CheckCityForMissions;
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnCityChanged -= CheckCityForMissions;
            }
        }

        private void SeedStartingMissions()
        {
            // 1. Görev: Konya'daki Karamanoğulları Askeri Düzenini İzle
            _availableMissions.Add(new EspionageMission(
                missionId: "karaman_recon_01",
                title: "Karamanoğlu Beyliği'nin Sınır Yığınağı",
                description: "Konya bedesteninde esnaf kılığına girerek Karamanoğlu Beyi'nin asker toplamaya başlayıp başlamadığını öğren ve haberi Bursa'ya getir.",
                targetCityId: "konya",
                deliveryCityId: "bursa",
                rewardAkce: 120,
                reputationGain: 20
            ));

            // 2. Görev: İznik Tekfuru'nun Gizli Mektubu
            _availableMissions.Add(new EspionageMission(
                missionId: "iznik_letter_02",
                title: "Bizans Tekfuru'nun Gizli İttifakı",
                description: "İznik pazarındaki Hristiyan tüccarlarla temas kurup Konstantinopolis'ten gelen gizli fermanı ele geçir ve Söğüt Beyliği divanına teslim et.",
                targetCityId: "iznik",
                deliveryCityId: "sogut",
                rewardAkce: 160,
                reputationGain: 30
            ));
        }

        public void AcceptMission(EspionageMission mission)
        {
            if (mission == null || _activeMissions.Contains(mission)) return;

            _availableMissions.Remove(mission);
            _activeMissions.Add(mission);

            OnMissionAccepted?.Invoke(mission);
        }

        private void CheckCityForMissions(City currentCity)
        {
            if (currentCity == null) return;

            // 1. Hedef şehre varıldıysa bilgiyi topla
            foreach (var mission in _activeMissions.Where(m => !m.IsIntelCollected && m.TargetCityId == currentCity.Id).ToList())
            {
                mission.MarkIntelCollected();
                OnIntelCollected?.Invoke(mission);
                GameManager.Instance.PlayerCaravan.SetMorale(GameManager.Instance.PlayerCaravan.Morale + 5);
            }

            // 2. Bilgi toplandıysa ve teslimat şehrine varıldıysa görevi tamamla
            foreach (var mission in _activeMissions.Where(m => m.IsIntelCollected && !m.IsCompleted && m.DeliveryCityId == currentCity.Id).ToList())
            {
                mission.CompleteMission();
                _activeMissions.Remove(mission);

                // Ödül ve Nüfuz
                GameManager.Instance.PlayerCaravan.AddGold(mission.RewardAkce);
                AddReputation(mission.OttomanReputationGain);

                OnMissionCompleted?.Invoke(mission);
            }
        }

        public void AddReputation(int amount)
        {
            OttomanReputation = Math.Max(0, OttomanReputation + amount);
            OnReputationChanged?.Invoke(OttomanReputation);
        }
    }
}
