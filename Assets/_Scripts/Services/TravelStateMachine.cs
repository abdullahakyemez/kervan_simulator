using System;
using UnityEngine;
using Kervan.Domain;

namespace Kervan.Services
{
    /// <summary>
    /// Kervanın seyahat aşamalarını (Yolda olma, tehlike karşılaşması, varış)
    /// yöneten sonlu durum makinesi (Finite State Machine - FSM).
    /// </summary>
    public class TravelStateMachine : MonoBehaviour
    {
        public static TravelStateMachine Instance { get; private set; } = null!;

        public TravelState CurrentState { get; private set; } = TravelState.InCity;

        public MapRoute? ActiveRoute { get; private set; }
        public City? DestinationCity { get; private set; }
        public EncounterData? CurrentEncounter { get; private set; }

        public float TraveledDistanceKm { get; private set; }
        public float RemainingDistanceKm => ActiveRoute != null ? Math.Max(0, ActiveRoute.DistanceKm - TraveledDistanceKm) : 0f;

        private readonly System.Random _rng = new System.Random();

        // Olaylar (UI'ın dinleyeceği sinyaller)
        public event Action<TravelState>? OnStateChanged;
        public event Action<float, float>? OnTravelProgress; // (traveled, total)
        public event Action<EncounterData>? OnEncounterTriggered;
        public event Action<EncounterResolutionResult>? OnEncounterResolved;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        /// <summary>
        /// Belirli bir rota üzerinden hedef şehre seyahati başlatır.
        /// </summary>
        public bool StartJourney(MapRoute route, City destination)
        {
            if (CurrentState != TravelState.InCity || route == null || destination == null)
                return false;

            var caravan = GameManager.Instance.PlayerCaravan;

            // Varsa geçiş ücretini öde
            if (route.TollAkce > 0)
            {
                if (!caravan.TrySpendGold(route.TollAkce))
                {
                    return false; // Geçiş ücretine para yetmedi
                }
            }

            ActiveRoute = route;
            DestinationCity = destination;
            TraveledDistanceKm = 0f;

            ChangeState(TravelState.OnTheRoad);
            OnTravelProgress?.Invoke(TraveledDistanceKm, ActiveRoute.DistanceKm);
            return true;
        }

        /// <summary>
        /// Yolda 1 günlük seyahat adımını simüle eder (Oyuncu 'İlerle' butonuna bastığında veya otomatik).
        /// </summary>
        public void AdvanceTravelDay()
        {
            if (CurrentState != TravelState.OnTheRoad || ActiveRoute == null || DestinationCity == null)
                return;

            var caravan = GameManager.Instance.PlayerCaravan;

            // 1. Günlük tüketimi ve takvimi işlet
            GameManager.Instance.AdvanceDay();

            // 2. Kilometre ilerlemesi yap
            float dailySpeed = caravan.CalculateDailySpeedKm();
            TraveledDistanceKm += dailySpeed;
            OnTravelProgress?.Invoke(TraveledDistanceKm, ActiveRoute.DistanceKm);

            // 3. Karşılaşma/Pusu zar atışı kontrolü (Yol tehlike oranına göre)
            float encounterChance = ActiveRoute.DangerFactor * 0.45f;
            if (_rng.NextDouble() < encounterChance)
            {
                TriggerEncounter();
                return;
            }

            // 4. Hedefe varıldı mı?
            if (TraveledDistanceKm >= ActiveRoute.DistanceKm)
            {
                CompleteJourney();
            }
        }

        private void TriggerEncounter()
        {
            if (ActiveRoute == null) return;

            int dangerPercent = (int)(ActiveRoute.DangerFactor * 100);
            CurrentEncounter = (_rng.NextDouble() < 0.70)
                ? EncounterData.CreateBanditAmbush(dangerPercent, _rng)
                : EncounterData.CreateWolfPack(_rng);

            ChangeState(TravelState.InEncounter);
            OnEncounterTriggered?.Invoke(CurrentEncounter);
        }

        /// <summary>
        /// Karşılaşma anında oyuncunun seçtiği stratejiyi (Savaş, Rüşvet, Kaçış, İkna) uygular.
        /// </summary>
        public EncounterResolutionResult? ResolveEncounter(IEncounterStrategy strategy)
        {
            if (CurrentState != TravelState.InEncounter || CurrentEncounter == null)
                return null;

            var caravan = GameManager.Instance.PlayerCaravan;
            var result = strategy.Execute(caravan, CurrentEncounter, _rng);

            OnEncounterResolved?.Invoke(result);

            // Karşılaşma bittiğinde yola devam et
            CurrentEncounter = null;

            if (TraveledDistanceKm >= (ActiveRoute?.DistanceKm ?? 0f))
            {
                CompleteJourney();
            }
            else
            {
                ChangeState(TravelState.OnTheRoad);
            }

            return result;
        }

        private void CompleteJourney()
        {
            if (DestinationCity == null) return;

            ChangeState(TravelState.Arrived);
            GameManager.Instance.ArriveAtCity(DestinationCity);

            ActiveRoute = null;
            DestinationCity = null;
            ChangeState(TravelState.InCity);
        }

        private void ChangeState(TravelState newState)
        {
            CurrentState = newState;
            OnStateChanged?.Invoke(CurrentState);
        }
    }
}
