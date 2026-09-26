using System;
using System.Collections.Generic;
using UnityEngine;
using Kervan.Domain;

namespace Kervan.Data
{
    /// <summary>
    /// Bir tarihi olayın belirli bir mal veya kategori üzerindeki fiyat etkisini tanımlar.
    /// </summary>
    [Serializable]
    public class ItemPriceModifier
    {
        [Tooltip("Etkilenecek mal kategorisi (Eğer 'All' istenirse tüm mallar)")]
        [SerializeField] private ItemCategory _affectedCategory = ItemCategory.Food;

        [Tooltip("Fiyat çarpanı değişimi (+0.5f = %50 pahalanma, -0.3f = %30 ucuzlama)")]
        [Range(-0.8f, 3.0f)]
        [SerializeField] private float _multiplierChange = 0.5f;

        public ItemCategory AffectedCategory => _affectedCategory;
        public float MultiplierChange => _multiplierChange;

        public ItemPriceModifier() { }

        public ItemPriceModifier(ItemCategory category, float change)
        {
            _affectedCategory = category;
            _multiplierChange = change;
        }
    }

    /// <summary>
    /// 14. yüzyıl Anadolu'sundaki tarihi olayları, salgınları, fetihleri ve siyasi krizleri temsil eden ScriptableObject.
    /// </summary>
    [CreateAssetMenu(fileName = "NewHistoricalEvent", menuName = "Kervan/Data/Historical Event", order = 5)]
    public class HistoricalEventDataSO : ScriptableObject
    {
        [Header("Olay Tanımı")]
        [Tooltip("Benzersiz olay kimliği (örn: black_death_1346, ahi_assembly, byzantine_border_skirmish)")]
        [SerializeField] private string _id = string.Empty;

        [Tooltip("Tarihi olayın başlığı")]
        [SerializeField] private string _title = string.Empty;

        [Tooltip("Olayın tetikleneceği oyun yılı (14. yy dönemi: 1300 - 1399)")]
        [Range(1300, 1399)]
        [SerializeField] private int _triggerYear = 1326;

        [Tooltip("Olayın süreceği gün sayısı")]
        [Min(1)]
        [SerializeField] private int _durationDays = 30;

        [Header("Coğrafi Etki")]
        [Tooltip("Bu olaydan etkilenen şehir kimlikleri (Boş bırakılırsa tüm Anadolu etkilenir)")]
        [SerializeField] private List<string> _affectedCityIds = new List<string>();

        [Header("Ekonomik ve Güvenlik Etkileri")]
        [Tooltip("Pazardaki belirli malların fiyatlarını değiştiren çarpanlar")]
        [SerializeField] private List<ItemPriceModifier> _priceModifiers = new List<ItemPriceModifier>();

        [Tooltip("Yollardaki haydut ve pusu riski artışı (+0.20 = %20 daha fazla saldırı riski)")]
        [Range(-0.5f, 1.0f)]
        [SerializeField] private float _dangerRiskModifier = 0.0f;

        [Header("Osmanlı İstihbarat & Görev Fırsatı")]
        [Tooltip("Bu olay Osmanlı Beyliği adına gizli bir istihbarat / ulak görevi doğurur mu?")]
        [SerializeField] private bool _offersEspionageMission = false;

        [Tooltip("Görevi başarıyla tamamlayıp Söğüt veya Bursa'ya bilgi ulaştırıldığında kazanılacak Akçe")]
        [Min(0)]
        [SerializeField] private int _espionageRewardAkce = 100;

        [Header("Görsel ve Anlatı")]
        [Tooltip("Olay fermanında / bildiriminde gösterilecek minyatür çizim")]
        [SerializeField] private Sprite? _eventIllustration;

        [Tooltip("Olay patlak verdiğinde oyuncuya gösterilecek ferman veya ulak haberi metni")]
        [TextArea(4, 8)]
        [SerializeField] private string _chronicleText = string.Empty;

        // Public Properties
        public string Id => _id;
        public string Title => _title;
        public int TriggerYear => _triggerYear;
        public int DurationDays => _durationDays;
        public IReadOnlyList<string> AffectedCityIds => _affectedCityIds.AsReadOnly();
        public IReadOnlyList<ItemPriceModifier> PriceModifiers => _priceModifiers.AsReadOnly();
        public float DangerRiskModifier => _dangerRiskModifier;
        public bool OffersEspionageMission => _offersEspionageMission;
        public int EspionageRewardAkce => _espionageRewardAkce;
        public Sprite? EventIllustration => _eventIllustration;
        public string ChronicleText => _chronicleText;

        /// <summary>
        /// Belirli bir şehrin bu olaydan etkilenip etkilenmediğini kontrol eder.
        /// </summary>
        public bool AffectsCity(string cityId)
        {
            if (string.IsNullOrWhiteSpace(cityId)) return false;
            // Eğer liste boşsa olay geneldir, tüm şehirleri etkiler
            return _affectedCityIds.Count == 0 || _affectedCityIds.Contains(cityId);
        }
    }
}
