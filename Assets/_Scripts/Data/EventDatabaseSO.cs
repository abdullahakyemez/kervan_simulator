using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kervan.Data
{
    /// <summary>
    /// Oyundaki tüm tarihi olayları, fermanları ve krizleri tutan merkezi veri tabanı.
    /// </summary>
    [CreateAssetMenu(fileName = "EventDatabase", menuName = "Kervan/Data/Event Database", order = 6)]
    public class EventDatabaseSO : ScriptableObject
    {
        [SerializeField] private List<HistoricalEventDataSO> _allEvents = new List<HistoricalEventDataSO>();

        public IReadOnlyList<HistoricalEventDataSO> AllEvents => _allEvents.AsReadOnly();

        public HistoricalEventDataSO? GetEventById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            return _allEvents.FirstOrDefault(e => e != null && e.Id == id);
        }

        public List<HistoricalEventDataSO> GetEventsForYear(int year)
        {
            return _allEvents.Where(e => e != null && e.TriggerYear == year).ToList();
        }
    }
}
