using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kervan.Data
{
    /// <summary>
    /// Oyundaki tüm 14. yüzyıl şehirlerinin listesini tutan merkezi ScriptableObject veri tabanı.
    /// </summary>
    [CreateAssetMenu(fileName = "CityDatabase", menuName = "Kervan/Data/City Database", order = 4)]
    public class CityDatabaseSO : ScriptableObject
    {
        [SerializeField] private List<CityDataSO> _allCities = new List<CityDataSO>();

        public IReadOnlyList<CityDataSO> AllCities => _allCities.AsReadOnly();

        public CityDataSO? GetCityById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            return _allCities.FirstOrDefault(c => c != null && c.Id == id);
        }
    }
}
