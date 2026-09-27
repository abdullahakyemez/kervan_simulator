using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Kervan.Domain;
using Kervan.Services;

namespace Kervan.Presentation
{
    /// <summary>
    /// 14. yüzyıl Anadolu seyahat ve rota seçim haritasını yöneten sunum kontrolcüsü.
    /// Oyuncuya alternatif rotalar sunar (Taş Yol vs Dağ Geçidi), seyahat günlerini ve pusu riskini gösterir,
    /// kervanın yoldaki ilerleyişini simüle eder.
    /// </summary>
    public class TravelMapScreenUI : MonoBehaviour
    {
        [Header("Rota Seçim Paneli")]
        [SerializeField] private GameObject? _routeSelectionSubPanel;
        [SerializeField] private TextMeshProUGUI? _currentLocationText;
        [SerializeField] private TextMeshProUGUI? _routeInfoTitleText;
        [SerializeField] private TextMeshProUGUI? _routeDetailsText;
        [SerializeField] private Button? _selectPavedRoadButton;
        [SerializeField] private Button? _selectMountainPassButton;
        [SerializeField] private Button? _startJourneyButton;

        [Header("Seyahat İlerleme Paneli")]
        [SerializeField] private GameObject? _travelProgressSubPanel;
        [SerializeField] private TextMeshProUGUI? _travelStatusText;
        [SerializeField] private Slider? _travelProgressBar;
        [SerializeField] private Button? _advanceDayButton;

        private MapRoute? _selectedRoute;
        private City? _targetCity;

        private void Start()
        {
            if (TravelStateMachine.Instance != null)
            {
                TravelStateMachine.Instance.OnTravelProgress += HandleTravelProgress;
                TravelStateMachine.Instance.OnStateChanged += HandleTravelStateChanged;
            }

            SetupButtons();
            RefreshMapView();
        }

        private void OnDestroy()
        {
            if (TravelStateMachine.Instance != null)
            {
                TravelStateMachine.Instance.OnTravelProgress -= HandleTravelProgress;
                TravelStateMachine.Instance.OnStateChanged -= HandleTravelStateChanged;
            }
        }

        private void OnEnable()
        {
            RefreshMapView();
        }

        private void SetupButtons()
        {
            _selectPavedRoadButton?.onClick.AddListener(SelectPavedRoad);
            _selectMountainPassButton?.onClick.AddListener(SelectMountainPass);
            _startJourneyButton?.onClick.AddListener(HandleStartJourney);
            _advanceDayButton?.onClick.AddListener(HandleAdvanceDay);
        }

        public void RefreshMapView()
        {
            if (GameManager.Instance == null || GameManager.Instance.CurrentCity == null)
                return;

            var currentCity = GameManager.Instance.CurrentCity;
            if (_currentLocationText != null)
                _currentLocationText.text = $"Mevcut Şehir: <color=#F7C844>{currentCity.Name}</color> ({currentCity.RulingFaction})";

            // Eğer kervan şu an yoldaysa ilerleme panelini göster
            bool isOnTheRoad = TravelStateMachine.Instance != null && TravelStateMachine.Instance.CurrentState == TravelState.OnTheRoad;

            if (_routeSelectionSubPanel != null) _routeSelectionSubPanel.SetActive(!isOnTheRoad);
            if (_travelProgressSubPanel != null) _travelProgressSubPanel.SetActive(isOnTheRoad);

            if (!isOnTheRoad)
            {
                // Varsayılan olarak İznik rotasını seçili getir
                SelectPavedRoad();
            }
        }

        private void SelectPavedRoad()
        {
            // Rota 1: Güvenli Taş Kervan Yolu (Bursa -> İznik: 90 km, %5 pusu tehlikesi)
            _targetCity = new City("iznik", "İznik", "Bizans Tekfurluğu / Sınır", 180f, 220f, 0.08f);
            _selectedRoute = new MapRoute(
                "bursa_iznik_highway",
                "Bursa - İznik Kral Yolu (Taş Döşeli)",
                "bursa",
                "iznik",
                TerrainType.PavedRoad,
                distanceKm: 90f,
                dangerFactor: 0.05f,
                tollAkce: 5
            );

            UpdateRouteDetailsUI();
        }

        private void SelectMountainPass()
        {
            // Rota 2: Kestirme Katırlı Dağ Patikası (Bursa -> İznik: 50 km, %40 haydut tehlikesi)
            _targetCity = new City("iznik", "İznik", "Bizans Tekfurluğu / Sınır", 180f, 220f, 0.08f);
            _selectedRoute = new MapRoute(
                "bursa_iznik_mountain",
                "Katırlı Dağ Patikası (Kestirme & Tehlikeli)",
                "bursa",
                "iznik",
                TerrainType.MountainPass,
                distanceKm: 50f,
                dangerFactor: 0.40f,
                tollAkce: 0
            );

            UpdateRouteDetailsUI();
        }

        private void UpdateRouteDetailsUI()
        {
            if (_selectedRoute == null || _targetCity == null || GameManager.Instance == null)
                return;

            var caravan = GameManager.Instance.PlayerCaravan;
            int estimatedDays = _selectedRoute.CalculateEstimatedDays(caravan);

            if (_routeInfoTitleText != null)
                _routeInfoTitleText.text = $"Hedef: <color=#F7C844>{_targetCity.Name}</color> ({_selectedRoute.RouteName})";

            if (_routeDetailsText != null)
            {
                string dangerColor = _selectedRoute.DangerFactor > 0.20f ? "#E55039" : "#78E08F";
                _routeDetailsText.text =
                    $"• <b>Mesafe:</b> {_selectedRoute.DistanceKm:F0} km\n" +
                    $"• <b>Arazi:</b> {_selectedRoute.Terrain}\n" +
                    $"• <b>Tahmini Süre:</b> {estimatedDays} Gün\n" +
                    $"• <b>Pusu / Haydut Riski:</b> <color={dangerColor}>%{_selectedRoute.DangerFactor * 100:F0}</color>\n" +
                    $"• <b>Derbent Vergisi:</b> {_selectedRoute.TollAkce} Akçe\n\n" +
                    $"<i>({_selectedRoute.Terrain} arazisinde kervan hızınız ve hayvanlarınız doğrudan etkilenecektir).</i>";
            }

            if (_startJourneyButton != null)
            {
                bool canAffordToll = caravan.Akce >= _selectedRoute.TollAkce;
                _startJourneyButton.interactable = canAffordToll;
            }
        }

        private void HandleStartJourney()
        {
            if (_selectedRoute == null || _targetCity == null || TravelStateMachine.Instance == null)
                return;

            bool started = TravelStateMachine.Instance.StartJourney(_selectedRoute, _targetCity);
            if (started)
            {
                if (_routeSelectionSubPanel != null) _routeSelectionSubPanel.SetActive(false);
                if (_travelProgressSubPanel != null) _travelProgressSubPanel.SetActive(true);

                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayTravelMusic();
            }
        }

        private void HandleAdvanceDay()
        {
            if (TravelStateMachine.Instance == null) return;
            TravelStateMachine.Instance.AdvanceTravelDay();
        }

        private void HandleTravelProgress(float traveledKm, float totalKm)
        {
            if (_travelProgressBar != null)
            {
                _travelProgressBar.maxValue = totalKm;
                _travelProgressBar.value = traveledKm;
            }

            if (_travelStatusText != null)
            {
                float percent = totalKm > 0 ? (traveledKm / totalKm) * 100f : 0f;
                _travelStatusText.text = $"Yoldasınız... {traveledKm:F0} / {totalKm:F0} km (%{percent:F0})";
            }
        }

        private void HandleTravelStateChanged(TravelState state)
        {
            if (state == TravelState.Arrived || state == TravelState.InCity)
            {
                RefreshMapView();
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayCityMusic();
            }
        }
    }
}
