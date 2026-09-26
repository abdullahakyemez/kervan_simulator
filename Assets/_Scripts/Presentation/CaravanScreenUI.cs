using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Kervan.Domain;
using Kervan.Services;

namespace Kervan.Presentation
{
    /// <summary>
    /// Oyuncunun kervanını yönettiği (muhafız kiralama, binek hayvanı satın alma,
    /// erzak depolama ve envanter dökümünü inceleme) ana ekran kontrolcüsü.
    /// </summary>
    public class CaravanScreenUI : MonoBehaviour
    {
        [Header("İstatistik Metinleri")]
        [SerializeField] private TextMeshProUGUI? _crewStatsText;
        [SerializeField] private TextMeshProUGUI? _combatPowerText;
        [SerializeField] private TextMeshProUGUI? _dailyExpenseText;
        [SerializeField] private TextMeshProUGUI? _dailyFoodConsumptionText;
        [SerializeField] private TextMeshProUGUI? _mountsBreakdownText;
        [SerializeField] private TextMeshProUGUI? _cargoInventoryListText;

        [Header("Muhafız Yönetimi")]
        [SerializeField] private Button? _hireGuardButton;
        [SerializeField] private Button? _dismissGuardButton;

        [Header("Binek Hayvanı Satın Alma")]
        [SerializeField] private Button? _buyCamelButton; // Deve: 80 Akçe (+200kg)
        [SerializeField] private Button? _buyMuleButton;  // Katır: 45 Akçe (+120kg)
        [SerializeField] private Button? _buyHorseButton; // At: 60 Akçe (+80kg, hız bonusu)

        [Header("Erzak Satın Alma")]
        [SerializeField] private Button? _buyFood10KgButton; // 10 Akçe
        [SerializeField] private Button? _buyFood50KgButton; // 45 Akçe (Toptan indirimli)

        // Maliyet Sabitleri
        public const int GUARD_HIRE_ADVANCE_FEE = 15; // Muhafız tutma peşinatı (Akçe)
        public const int CAMEL_PRICE = 80;
        public const int MULE_PRICE = 45;
        public const int HORSE_PRICE = 60;
        public const int FOOD_10KG_PRICE = 10;
        public const int FOOD_50KG_PRICE = 45;

        private void OnEnable()
        {
            RefreshCaravanView();
        }

        private void Start()
        {
            SetupButtonListeners();
            RefreshCaravanView();
        }

        private void SetupButtonListeners()
        {
            _hireGuardButton?.onClick.AddListener(HandleHireGuard);
            _dismissGuardButton?.onClick.AddListener(HandleDismissGuard);

            _buyCamelButton?.onClick.AddListener(() => HandleBuyMount(MountType.Camel, CAMEL_PRICE));
            _buyMuleButton?.onClick.AddListener(() => HandleBuyMount(MountType.Mule, MULE_PRICE));
            _buyHorseButton?.onClick.AddListener(() => HandleBuyMount(MountType.Horse, HORSE_PRICE));

            _buyFood10KgButton?.onClick.AddListener(() => HandleBuyFood(10f, FOOD_10KG_PRICE));
            _buyFood50KgButton?.onClick.AddListener(() => HandleBuyFood(50f, FOOD_50KG_PRICE));
        }

        public void RefreshCaravanView()
        {
            if (GameManager.Instance == null || GameManager.Instance.PlayerCaravan == null)
                return;

            var caravan = GameManager.Instance.PlayerCaravan;

            // 1. İstatistik Metinlerini Güncelle
            if (_crewStatsText != null)
                _crewStatsText.text = $"Mürettebat: 1 Tüccar, {caravan.GuardCount} Muhafız, {caravan.LaborerCount} Kervancı (Toplam: {caravan.TotalPeople} Kişi)";

            if (_combatPowerText != null)
                _combatPowerText.text = $"Savaş Gücü: {caravan.TotalCombatPower} Puan (Tüccar Ustalığı: {caravan.MerchantCombatSkill}/10)";

            if (_dailyExpenseText != null)
                _dailyExpenseText.text = $"Günlük Ulufe Gideri: {caravan.DailyWageExpense} Akçe / Gün";

            if (_dailyFoodConsumptionText != null)
                _dailyFoodConsumptionText.text = $"Günlük Erzak Tüketimi: {caravan.DailyFoodConsumption:F1} kg / Gün";

            if (_mountsBreakdownText != null)
                _mountsBreakdownText.text = $"Binekler: {caravan.CamelCount} Deve, {caravan.MuleCount} Katır, {caravan.HorseCount} At (Toplam Kapasite: {caravan.Cargo.MaxCapacityKg:F0} kg)";

            // 2. Kervandaki Yüklerin Listesi
            if (_cargoInventoryListText != null)
            {
                var sb = new StringBuilder();
                var items = caravan.Cargo.GetAllItems();

                if (items.Count == 0)
                {
                    sb.Append("<i>Kervan heybeleri boş. Pazar sekmesinden ticaret malı satın alabilirsiniz.</i>");
                }
                else
                {
                    foreach (var stack in items)
                    {
                        sb.AppendLine($"• <b>{stack.Item.Name}</b>: {stack.Quantity} adet ({stack.TotalWeightKg:F1} kg) - Değer: ~{stack.TotalBaseValue} Akçe");
                    }
                }

                _cargoInventoryListText.text = sb.ToString();
            }

            // 3. Buton Etkinliklerini Güncelle
            UpdateButtonStates(caravan);
        }

        private void UpdateButtonStates(Caravan caravan)
        {
            if (_hireGuardButton != null)
                _hireGuardButton.interactable = caravan.Akce >= GUARD_HIRE_ADVANCE_FEE;

            if (_dismissGuardButton != null)
                _dismissGuardButton.interactable = caravan.GuardCount > 0;

            if (_buyCamelButton != null)
                _buyCamelButton.interactable = caravan.Akce >= CAMEL_PRICE;

            if (_buyMuleButton != null)
                _buyMuleButton.interactable = caravan.Akce >= MULE_PRICE;

            if (_buyHorseButton != null)
                _buyHorseButton.interactable = caravan.Akce >= HORSE_PRICE;

            if (_buyFood10KgButton != null)
                _buyFood10KgButton.interactable = caravan.Akce >= FOOD_10KG_PRICE;

            if (_buyFood50KgButton != null)
                _buyFood50KgButton.interactable = caravan.Akce >= FOOD_50KG_PRICE;
        }

        private void HandleHireGuard()
        {
            var caravan = GameManager.Instance.PlayerCaravan;
            if (caravan.TrySpendGold(GUARD_HIRE_ADVANCE_FEE))
            {
                caravan.HireGuard(1);
                RefreshCaravanView();
            }
        }

        private void HandleDismissGuard()
        {
            var caravan = GameManager.Instance.PlayerCaravan;
            if (caravan.DismissGuard(1))
            {
                RefreshCaravanView();
            }
        }

        private void HandleBuyMount(MountType type, int price)
        {
            var caravan = GameManager.Instance.PlayerCaravan;
            if (caravan.TrySpendGold(price))
            {
                caravan.AddMount(type, 1);
                RefreshCaravanView();
            }
        }

        private void HandleBuyFood(float amountKg, int price)
        {
            var caravan = GameManager.Instance.PlayerCaravan;
            if (caravan.TrySpendGold(price))
            {
                caravan.AddFood(amountKg);
                RefreshCaravanView();
            }
        }
    }
}
