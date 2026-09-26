using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kervan.Data
{
    /// <summary>
    /// Oyundaki tüm ticaret mallarının merkezi listesini tutan ScriptableObject veri tabanı.
    /// </summary>
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Kervan/Data/Item Database", order = 2)]
    public class ItemDatabaseSO : ScriptableObject
    {
        [SerializeField] private List<ItemDataSO> _allItems = new List<ItemDataSO>();

        public IReadOnlyList<ItemDataSO> AllItems => _allItems.AsReadOnly();

        public ItemDataSO? GetItemById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            return _allItems.FirstOrDefault(item => item != null && item.Id == id);
        }
    }
}
