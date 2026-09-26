using System;
using System.Collections.Generic;

namespace Kervan.Services
{
    /// <summary>
    /// Envanterdeki tek bir malın kayıt altına alınan verisi.
    /// </summary>
    [Serializable]
    public class SavedCargoItem
    {
        public string ItemId = string.Empty;
        public int Quantity = 0;
    }

    /// <summary>
    /// Oyunun tüm durumunu (Kervan, altın, erzak, binekler, şehir, tarih ve casusluk nüfuzu)
    /// dosyalara JSON olarak kaydetmek için kullanılan saf C# veri aktarım modeli (DTO).
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public int Version = 1;
        public string SaveDateTimestamp = string.Empty;

        // 1. Kervan Durumu
        public int Akce;
        public float FoodSupplyKg;
        public int Morale;
        public int MerchantCombatSkill;

        public int GuardCount;
        public int LaborerCount;

        public int CamelCount;
        public int MuleCount;
        public int HorseCount;

        public List<SavedCargoItem> CargoItems = new List<SavedCargoItem>();

        // 2. Dünya ve Zaman
        public string CurrentCityId = "bursa";
        public int CurrentDay = 1;
        public int CurrentYear = 1326;

        // 3. İstihbarat ve Nüfuz
        public int OttomanReputation = 50;
        public List<string> CompletedMissionIds = new List<string>();
    }
}
