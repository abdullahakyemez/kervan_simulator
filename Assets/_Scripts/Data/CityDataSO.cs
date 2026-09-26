using System;
using System.Collections.Generic;
using UnityEngine;
using Kervan.Domain;

namespace Kervan.Data
{
    /// <summary>
    /// Bir şehrin pazarında başlangıçta yer alacak malları, stokları ve arz-talep çarpanlarını tanımlayan yapı.
    /// [Serializable]: Unity Inspector penceresinde liste elemanı olarak düzenlenebilmesini sağlar.
    /// </summary>
    [Serializable]
    public class CityMarketSetupEntry
    {
        [Tooltip("Pazarda satılacak ticaret malı veri kartı")]
        [SerializeField] private ItemDataSO? _itemData;

        [Tooltip("Şehir pazarındaki başlangıç stok adedi")]
        [Min(0)]
        [SerializeField] private int _initialStock = 20;

        [Tooltip("Arz-Talep Çarpanı (< 1.0: Şehir üretiyor/ucuz, > 1.0: Şehir talep ediyor/pahalı)")]
        [Range(0.4f, 2.5f)]
        [SerializeField] private float _supplyDemandMultiplier = 1.0f;

        public ItemDataSO? ItemData => _itemData;
        public int InitialStock => _initialStock;
        public float SupplyDemandMultiplier => _supplyDemandMultiplier;

        public CityMarketSetupEntry() { }

        public CityMarketSetupEntry(ItemDataSO itemData, int initialStock, float multiplier)
        {
            _itemData = itemData;
            _initialStock = initialStock;
            _supplyDemandMultiplier = multiplier;
        }
    }

    /// <summary>
    /// 14. yüzyıl Anadolu şehirlerinin Unity Editörü üzerinden tanımlanmasını sağlayan ScriptableObject veri kartı.
    /// </summary>
    [CreateAssetMenu(fileName = "NewCityData", menuName = "Kervan/Data/City Data", order = 3)]
    public class CityDataSO : ScriptableObject
    {
        [Header("Şehir Kimliği")]
        [Tooltip("Benzersiz şehir kimliği (örn: bursa, iznik, sogut, konya, trabzon)")]
        [SerializeField] private string _id = string.Empty;

        [Tooltip("Oyunda gösterilecek tarihi şehir adı")]
        [SerializeField] private string _cityName = string.Empty;

        [Tooltip("Şehri yöneten beylik veya devlet (örn: Osmanlı Beyliği, Karamanoğulları, Bizans Tekfurluğu)")]
        [SerializeField] private string _rulingFaction = "Osmanlı Beyliği";

        [Header("Coğrafya ve Vergi")]
        [Tooltip("2D Harita üzerindeki X ve Y koordinatları (Seyahat mesafesi hesabı için)")]
        [SerializeField] private Vector2 _mapCoordinates = Vector2.zero;

        [Tooltip("Yerel pazar alışveriş vergisi oranı (%5 = 0.05)")]
        [Range(0f, 0.30f)]
        [SerializeField] private float _localTaxRate = 0.05f;

        [Header("Pazar Malları")]
        [Tooltip("Bu şehrin pazarında yer alan mallar ve arz-talep dengeleri")]
        [SerializeField] private List<CityMarketSetupEntry> _marketGoods = new List<CityMarketSetupEntry>();

        [Header("Görsel ve Tarihi Bilgi")]
        [Tooltip("Şehir ekranında gösterilecek 2D arka plan / minyatür görseli")]
        [SerializeField] private Sprite? _cityIllustration;

        [Tooltip("Şehri yöneten beyliğin sancağı / arması")]
        [SerializeField] private Sprite? _factionBanner;

        [Tooltip("14. yüzyılda bu şehrin siyasi ve ticari konumunu özetleyen tarihi metin")]
        [TextArea(3, 6)]
        [SerializeField] private string _historicalLore = string.Empty;

        // Public Properties
        public string Id => _id;
        public string CityName => _cityName;
        public string RulingFaction => _rulingFaction;
        public Vector2 MapCoordinates => _mapCoordinates;
        public float LocalTaxRate => _localTaxRate;
        public IReadOnlyList<CityMarketSetupEntry> MarketGoods => _marketGoods.AsReadOnly();
        public Sprite? CityIllustration => _cityIllustration;
        public Sprite? FactionBanner => _factionBanner;
        public string HistoricalLore => _historicalLore;

        /// <summary>
        /// Unity ScriptableObject verisini saf C# Domain City nesnesine ve pazar stoğuna dönüştürür.
        /// </summary>
        public City ToDomain()
        {
            var city = new City(
                id: _id,
                name: _cityName,
                rulingFaction: _rulingFaction,
                coordX: _mapCoordinates.x,
                coordY: _mapCoordinates.y,
                localTaxRate: _localTaxRate
            );

            // Pazar mallarını ve stoklarını doldur
            foreach (var entry in _marketGoods)
            {
                if (entry?.ItemData != null)
                {
                    city.LocalMarket.AddOrUpdateGood(
                        entry.ItemData.ToDomain(),
                        entry.InitialStock,
                        entry.SupplyDemandMultiplier
                    );
                }
            }

            return city;
        }
    }
}
