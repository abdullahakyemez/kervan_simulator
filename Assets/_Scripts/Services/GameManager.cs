using System;
using UnityEngine;
using Kervan.Domain;
using Kervan.Data;

namespace Kervan.Services
{
    /// <summary>
    /// Oyunun ana oturumunu, oyuncu kervanını, bulunulan şehri ve zaman akışını yöneten merkezi servis.
    /// Singleton deseniyle oluşturulmuştur; servis katmanı Domain ile Presentation (UI) arasındaki köprüdür.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; } = null!;

        [Header("Başlangıç Veritabanları")]
        [SerializeField] private ItemDatabaseSO? _itemDatabase;
        [SerializeField] private CityDatabaseSO? _cityDatabase;
        [SerializeField] private MapGraphDataSO? _mapGraphData;

        [Header("Başlangıç Ayarları")]
        [SerializeField] private string _startingCityId = "bursa";
        [SerializeField] private int _startingAkce = 150;
        [SerializeField] private float _startingFoodKg = 40f;
        [SerializeField] private int _startYear = 1326; // Bursa'nın fethi yılı

        // Canlı Domain Nesneleri
        public Caravan PlayerCaravan { get; private set; } = null!;
        public City CurrentCity { get; private set; } = null!;
        public MapGraph WorldMapGraph { get; private set; } = null!;

        // Zaman Takibi
        public int CurrentDay { get; private set; } = 1;
        public int CurrentYear { get; private set; } = 1326;

        // Olaylar (UI'ın dinleyeceği sinyaller)
        public event Action<City>? OnCityChanged;
        public event Action<int, int>? OnDateChanged; // (day, year)
        public event Action<string>? OnNotificationSent;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeGameSession();
        }

        private void InitializeGameSession()
        {
            CurrentYear = _startYear;
            CurrentDay = 1;

            // 1. Kervanı Oluştur (150 Akçe, 40 kg erzak, orta savaş meziyeti)
            PlayerCaravan = new Caravan(_startingAkce, _startingFoodKg, initialMerchantSkill: 3);

            // Başlangıç için kervana 1 katır ve 1 muhafız ekle
            PlayerCaravan.AddMount(MountType.Mule, 1);
            PlayerCaravan.HireGuard(1);

            // 2. Harita Grafını İnşa Et
            if (_mapGraphData != null)
            {
                WorldMapGraph = _mapGraphData.BuildDomainGraph();
            }
            else
            {
                WorldMapGraph = new MapGraph();
            }

            // 3. Başlangıç Şehrini Belirle
            if (_cityDatabase != null)
            {
                var startingCitySO = _cityDatabase.GetCityById(_startingCityId);
                if (startingCitySO != null)
                {
                    CurrentCity = startingCitySO.ToDomain();
                }
                else
                {
                    CurrentCity = new City(_startingCityId, "Bursa", "Osmanlı Beyliği", 100f, 200f);
                }
            }
            else
            {
                CurrentCity = new City("bursa", "Bursa", "Osmanlı Beyliği", 100f, 200f);
            }

            // Eğer pazarda henüz mal yoksa varsayılan 14. yy Bursa bedesteni mallarını tohumla (Seed)
            if (CurrentCity.LocalMarket.GetAllGoods().Count == 0)
            {
                var silk = new Item("silk", "Bursa İpeği", ItemCategory.Textile, basePrice: 20, weightKg: 2f);
                var wheat = new Item("wheat", "Anadolu Buğdayı", ItemCategory.Food, basePrice: 5, weightKg: 5f, isPerishable: true, shelfLifeDays: 90);
                var steel = new Item("damascus_steel", "Şam Çeliği", ItemCategory.Metal, basePrice: 45, weightKg: 8f);
                var oliveOil = new Item("olive_oil", "Zeytinyağı", ItemCategory.Food, basePrice: 12, weightKg: 3f);

                // Bursa ipeğin merkezidir: İpek ucuz (0.6x), Şam çeliği ise uzaktan gelir primli (1.4x)
                CurrentCity.LocalMarket.AddOrUpdateGood(silk, stock: 40, multiplier: 0.6f);
                CurrentCity.LocalMarket.AddOrUpdateGood(wheat, stock: 100, multiplier: 1.0f);
                CurrentCity.LocalMarket.AddOrUpdateGood(steel, stock: 12, multiplier: 1.4f);
                CurrentCity.LocalMarket.AddOrUpdateGood(oliveOil, stock: 35, multiplier: 0.8f);
            }
        }

        private void Start()
        {
            // İlk başlangıç durumunu UI'a bildir
            OnCityChanged?.Invoke(CurrentCity);
            OnDateChanged?.Invoke(CurrentDay, CurrentYear);
            OnNotificationSent?.Invoke($"{CurrentCity.Name} şehrine hoş geldiniz. Ticaret için pazar kuruldu.");
        }

        /// <summary>
        /// Oyunda 1 gün ilerletir (Erzak tüketimi, muhafız maaşları vb.).
        /// </summary>
        public void AdvanceDay()
        {
            CurrentDay++;
            if (CurrentDay > 360) // 1 yıl = 360 gün
            {
                CurrentDay = 1;
                CurrentYear++;
            }

            PlayerCaravan.AdvanceOneDay();
            OnDateChanged?.Invoke(CurrentDay, CurrentYear);
        }

        /// <summary>
        /// Kervanın yeni bir şehre varışını işler.
        /// </summary>
        public void ArriveAtCity(City newCity)
        {
            if (newCity == null) return;

            CurrentCity = newCity;
            OnCityChanged?.Invoke(CurrentCity);
            OnNotificationSent?.Invoke($"{newCity.Name} kapılarından içeri girdiniz ({newCity.RulingFaction}).");
        }
    }
}
