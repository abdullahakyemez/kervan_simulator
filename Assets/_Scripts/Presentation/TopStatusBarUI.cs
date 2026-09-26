using TMPro;
using UnityEngine;
using Kervan.Domain;
using Kervan.Services;

namespace Kervan.Presentation
{
    /// <summary>
    /// Ekranın üst kısmında yer alan, kervanın anlık durumunu (Akçe, Erzak, Yük, Moral, Tarih)
    /// gösteren kullanıcı arayüzü kontrolcüsü.
    /// Event-Driven: Kervan ve oyun yöneticisindeki C# olaylarını dinleyerek otomatik güncellenir.
    /// </summary>
    public class TopStatusBarUI : MonoBehaviour
    {
        [Header("Arayüz Metin Elemanları (TextMeshPro)")]
        [SerializeField] private TextMeshProUGUI? _goldText;
        [SerializeField] private TextMeshProUGUI? _foodText;
        [SerializeField] private TextMeshProUGUI? _weightText;
        [SerializeField] private TextMeshProUGUI? _moraleText;
        [SerializeField] private TextMeshProUGUI? _dateText;
        [SerializeField] private TextMeshProUGUI? _locationText;
        [SerializeField] private TextMeshProUGUI? _notificationBannerText;

        [Header("Moral Renk Teması")]
        [SerializeField] private Color _highMoraleColor = new Color(0.2f, 0.75f, 0.2f);   // Yeşil
        [SerializeField] private Color _mediumMoraleColor = new Color(0.85f, 0.65f, 0.1f); // Sarı / Amber
        [SerializeField] private Color _lowMoraleColor = new Color(0.85f, 0.2f, 0.2f);     // Kırmızı

        private void Start()
        {
            // GameManager ve Kervan hazır olduğunda olaylara abone ol
            SubscribeToEvents();
            RefreshAll();
        }

        private void OnDestroy()
        {
            // Bellek sızıntılarını (Memory Leak) önlemek için abonelikten çık
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            if (GameManager.Instance == null) return;

            var caravan = GameManager.Instance.PlayerCaravan;
            if (caravan != null)
            {
                caravan.OnGoldChanged += HandleGoldChanged;
                caravan.OnFoodChanged += HandleFoodChanged;
                caravan.OnMoraleChanged += HandleMoraleChanged;
                caravan.OnCaravanWarning += HandleNotification;
                caravan.Cargo.OnWeightChanged += HandleWeightChanged;
            }

            GameManager.Instance.OnCityChanged += HandleCityChanged;
            GameManager.Instance.OnDateChanged += HandleDateChanged;
            GameManager.Instance.OnNotificationSent += HandleNotification;
        }

        private void UnsubscribeFromEvents()
        {
            if (GameManager.Instance == null) return;

            var caravan = GameManager.Instance.PlayerCaravan;
            if (caravan != null)
            {
                caravan.OnGoldChanged -= HandleGoldChanged;
                caravan.OnFoodChanged -= HandleFoodChanged;
                caravan.OnMoraleChanged -= HandleMoraleChanged;
                caravan.OnCaravanWarning -= HandleNotification;
                caravan.Cargo.OnWeightChanged -= HandleWeightChanged;
            }

            GameManager.Instance.OnCityChanged -= HandleCityChanged;
            GameManager.Instance.OnDateChanged -= HandleDateChanged;
            GameManager.Instance.OnNotificationSent -= HandleNotification;
        }

        /// <summary>
        /// Tüm arayüz elemanlarını o anki verilerle anında tazeler.
        /// </summary>
        public void RefreshAll()
        {
            if (GameManager.Instance == null) return;

            var caravan = GameManager.Instance.PlayerCaravan;
            if (caravan != null)
            {
                HandleGoldChanged(caravan.Akce);
                HandleFoodChanged(caravan.FoodSupplyKg);
                HandleMoraleChanged(caravan.Morale);
                HandleWeightChanged(caravan.Cargo.CurrentWeightKg, caravan.Cargo.MaxCapacityKg);
            }

            if (GameManager.Instance.CurrentCity != null)
            {
                HandleCityChanged(GameManager.Instance.CurrentCity);
            }

            HandleDateChanged(GameManager.Instance.CurrentDay, GameManager.Instance.CurrentYear);
        }

        // --- OLAY İŞLEYİCİLERİ (EVENT HANDLERS) ---

        private void HandleGoldChanged(int newGold)
        {
            if (_goldText != null)
            {
                _goldText.text = $"{newGold:N0} Akçe";
            }
        }

        private void HandleFoodChanged(float newFoodKg)
        {
            if (_foodText != null)
            {
                _foodText.text = $"{newFoodKg:F1} kg Erzak";
            }
        }

        private void HandleWeightChanged(float currentWeight, float maxCapacity)
        {
            if (_weightText != null)
            {
                _weightText.text = $"{currentWeight:F0} / {maxCapacity:F0} kg";
                // Eğer kapasite %90'ın üzerindeyse kırmızımsı renge bürünsün
                _weightText.color = (currentWeight >= maxCapacity * 0.9f) ? _lowMoraleColor : Color.white;
            }
        }

        private void HandleMoraleChanged(int newMorale)
        {
            if (_moraleText != null)
            {
                _moraleText.text = $"Moral: %{newMorale}";

                if (newMorale >= 70) _moraleText.color = _highMoraleColor;
                else if (newMorale >= 40) _moraleText.color = _mediumMoraleColor;
                else _moraleText.color = _lowMoraleColor;
            }
        }

        private void HandleCityChanged(City newCity)
        {
            if (_locationText != null)
            {
                _locationText.text = $"{newCity.Name} ({newCity.RulingFaction})";
            }
        }

        private void HandleDateChanged(int day, int year)
        {
            if (_dateText != null)
            {
                _dateText.text = $"Gün {day} • {year} Miladi";
            }
        }

        private void HandleNotification(string message)
        {
            if (_notificationBannerText != null)
            {
                _notificationBannerText.text = message;
            }
        }
    }
}
