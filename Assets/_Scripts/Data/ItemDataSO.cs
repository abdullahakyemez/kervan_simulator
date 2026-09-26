using UnityEngine;
using Kervan.Domain;

namespace Kervan.Data
{
    /// <summary>
    /// Unity Editörü üzerinden yeni ticaret malları tanımlamamızı sağlayan ScriptableObject veri şablonu.
    /// Kod yazmadan Unity arayüzünden (Create -> Kervan -> Item Data) yeni eşyalar eklenebilir.
    /// </summary>
    [CreateAssetMenu(fileName = "NewItemData", menuName = "Kervan/Data/Item Data", order = 1)]
    public class ItemDataSO : ScriptableObject
    {
        [Header("Temel Bilgiler")]
        [Tooltip("Kod içerisinde kullanılacak benzersiz kimlik (örn: silk, wheat, damascus_steel)")]
        [SerializeField] private string _id = string.Empty;

        [Tooltip("Oyunda görüntülenecek tarihi ad (örn: Bursa İpeği, Şam Çeliği)")]
        [SerializeField] private string _itemName = string.Empty;

        [Tooltip("Ticarete konu olan malın kategorisi")]
        [SerializeField] private ItemCategory _category = ItemCategory.Food;

        [Header("Ekonomi ve Ağırlık")]
        [Tooltip("Akçe cinsinden taban pazar değeri")]
        [Min(1)]
        [SerializeField] private int _basePrice = 10;

        [Tooltip("Birim başına ağırlık (kg)")]
        [Min(0.1f)]
        [SerializeField] private float _weightKg = 1.0f;

        [Header("Bozulma ve Dayanıklılık")]
        [SerializeField] private bool _isPerishable = false;

        [Tooltip("Raf ömrü (Gün). Bozulmayan mallarda 0 kalmalıdır.")]
        [Min(0)]
        [SerializeField] private int _shelfLifeDays = 0;

        [Header("Görsel ve Anlatı (Lore)")]
        [Tooltip("2D Envanter ve Pazar arayüzünde gösterilecek ikon")]
        [SerializeField] private Sprite? _icon;

        [Tooltip("14. yüzyıl Anadolu'sundaki tarihi yeri ve önemini anlatan kısa açıklama")]
        [TextArea(3, 6)]
        [SerializeField] private string _description = string.Empty;

        // Public Properties
        public string Id => _id;
        public string ItemName => _itemName;
        public ItemCategory Category => _category;
        public int BasePrice => _basePrice;
        public float WeightKg => _weightKg;
        public bool IsPerishable => _isPerishable;
        public int ShelfLifeDays => _shelfLifeDays;
        public Sprite? Icon => _icon;
        public string Description => _description;

        /// <summary>
        /// Unity ScriptableObject verisini saf C# Domain Item nesnesine dönüştürür (Köprü Metot).
        /// </summary>
        public Item ToDomain()
        {
            return new Item(
                id: _id,
                name: _itemName,
                category: _category,
                basePrice: _basePrice,
                weightKg: _weightKg,
                isPerishable: _isPerishable,
                shelfLifeDays: _shelfLifeDays
            );
        }
    }
}
