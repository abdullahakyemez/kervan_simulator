using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Kervan.Domain;
using Kervan.Services;

namespace Kervan.Presentation
{
    /// <summary>
    /// Seyahat esnasında bir tehlike (haydut pususu, kurt sürüsü) patlak verdiğinde
    /// açılan interaktif karar ekranı. Oyuncuya 4 stratejik seçenek sunar (Strategy Pattern).
    /// </summary>
    public class EncounterDialogUI : MonoBehaviour
    {
        [Header("Dialog Paneli")]
        [SerializeField] private GameObject? _dialogRoot;
        [SerializeField] private TextMeshProUGUI? _titleText;
        [SerializeField] private TextMeshProUGUI? _descriptionText;
        [SerializeField] private TextMeshProUGUI? _threatComparisonText;
        [SerializeField] private TextMeshProUGUI? _resultOutcomeText;

        [Header("Karar Butonları")]
        [SerializeField] private Button? _combatButton;
        [SerializeField] private Button? _bribeButton;
        [SerializeField] private Button? _fleeButton;
        [SerializeField] private Button? _negotiateButton;
        [SerializeField] private Button? _continueJourneyButton;

        private void Start()
        {
            if (TravelStateMachine.Instance != null)
            {
                TravelStateMachine.Instance.OnEncounterTriggered += HandleEncounterTriggered;
                TravelStateMachine.Instance.OnEncounterResolved += HandleEncounterResolved;
            }

            SetupButtons();
            CloseDialog();
        }

        private void OnDestroy()
        {
            if (TravelStateMachine.Instance != null)
            {
                TravelStateMachine.Instance.OnEncounterTriggered -= HandleEncounterTriggered;
                TravelStateMachine.Instance.OnEncounterResolved -= HandleEncounterResolved;
            }
        }

        private void SetupButtons()
        {
            _combatButton?.onClick.AddListener(() =>
                TravelStateMachine.Instance.ResolveEncounter(new CombatStrategy()));

            _bribeButton?.onClick.AddListener(() =>
                TravelStateMachine.Instance.ResolveEncounter(new BribeStrategy()));

            _fleeButton?.onClick.AddListener(() =>
                TravelStateMachine.Instance.ResolveEncounter(new FleeStrategy()));

            _negotiateButton?.onClick.AddListener(() =>
                TravelStateMachine.Instance.ResolveEncounter(new NegotiateStrategy()));

            _continueJourneyButton?.onClick.AddListener(CloseDialog);
        }

        private void HandleEncounterTriggered(EncounterData encounter)
        {
            if (_dialogRoot == null) return;

            _dialogRoot.SetActive(true);

            if (_titleText != null) _titleText.text = encounter.Title;
            if (_descriptionText != null) _descriptionText.text = encounter.Description;

            var caravan = GameManager.Instance.PlayerCaravan;
            if (_threatComparisonText != null && caravan != null)
            {
                _threatComparisonText.text = $"Düşman Gücü: {encounter.ThreatPower} vs Kervan Savunma Gücü: {caravan.TotalCombatPower}";
            }

            // Sonuç alanını gizle, karar butonlarını aç
            SetActionButtonsActive(true);
            if (_continueJourneyButton != null) _continueJourneyButton.gameObject.SetActive(false);
            if (_resultOutcomeText != null) _resultOutcomeText.text = string.Empty;

            // Haraç butonunu akçe yetiyorsa göster
            if (_bribeButton != null)
            {
                bool canBribe = encounter.DemandedBribeAkce > 0 && caravan != null && caravan.Akce >= encounter.DemandedBribeAkce;
                _bribeButton.interactable = canBribe;
                var bribeText = _bribeButton.GetComponentInChildren<TextMeshProUGUI>();
                if (bribeText != null)
                {
                    bribeText.text = (encounter.DemandedBribeAkce > 0)
                        ? $"Haraç Ver ({encounter.DemandedBribeAkce} Akçe)"
                        : "Anlaşma Mümkün Değil";
                }
            }
        }

        private void HandleEncounterResolved(EncounterResolutionResult result)
        {
            SetActionButtonsActive(false);

            if (_resultOutcomeText != null)
            {
                _resultOutcomeText.text = (result.IsSuccess ? "<color=green>BAŞARILI!</color> " : "<color=red>KAYIP YAŞANDI!</color> ") + result.Message;
            }

            if (_continueJourneyButton != null)
            {
                _continueJourneyButton.gameObject.SetActive(true);
            }
        }

        private void SetActionButtonsActive(bool active)
        {
            if (_combatButton != null) _combatButton.gameObject.SetActive(active);
            if (_bribeButton != null) _bribeButton.gameObject.SetActive(active);
            if (_fleeButton != null) _fleeButton.gameObject.SetActive(active);
            if (_negotiateButton != null) _negotiateButton.gameObject.SetActive(active);
        }

        private void CloseDialog()
        {
            if (_dialogRoot != null)
                _dialogRoot.SetActive(false);
        }
    }
}
