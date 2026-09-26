using System;
using System.IO;
using UnityEngine;
using Kervan.Domain;
using Kervan.Data;

namespace Kervan.Services
{
    /// <summary>
    /// Oyunun kaydedilmesini (Save) ve yüklenmesini (Load) yöneten dosya sistemi servisi.
    /// Mobil platformlarda (Android/iOS) ve PC'de güvenli Application.persistentDataPath dizinini kullanır.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; } = null!;

        private const string SAVE_FILE_NAME = "kervan_save.json";
        private string SaveFilePath => Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

        [SerializeField] private ItemDatabaseSO? _itemDatabase;
        [SerializeField] private CityDatabaseSO? _cityDatabase;

        public event Action? OnGameSaved;
        public event Action? OnGameLoaded;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public bool HasSaveFile()
        {
            return File.Exists(SaveFilePath);
        }

        /// <summary>
        /// Mevcut oyun durumunu JSON formatında diske yazar.
        /// </summary>
        public bool SaveGame()
        {
            if (GameManager.Instance == null) return false;

            try
            {
                var caravan = GameManager.Instance.PlayerCaravan;
                var saveData = new SaveData
                {
                    SaveDateTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Akce = caravan.Akce,
                    FoodSupplyKg = caravan.FoodSupplyKg,
                    Morale = caravan.Morale,
                    MerchantCombatSkill = caravan.MerchantCombatSkill,
                    GuardCount = caravan.GuardCount,
                    LaborerCount = caravan.LaborerCount,
                    CamelCount = caravan.CamelCount,
                    MuleCount = caravan.MuleCount,
                    HorseCount = caravan.HorseCount,
                    CurrentCityId = GameManager.Instance.CurrentCity.Id,
                    CurrentDay = GameManager.Instance.CurrentDay,
                    CurrentYear = GameManager.Instance.CurrentYear,
                    OttomanReputation = EspionageService.Instance != null ? EspionageService.Instance.OttomanReputation : 50
                };

                // Kervandaki eşyaları ekle
                foreach (var stack in caravan.Cargo.GetAllItems())
                {
                    saveData.CargoItems.Add(new SavedCargoItem
                    {
                        ItemId = stack.Item.Id,
                        Quantity = stack.Quantity
                    });
                }

                // JSON'a serileştir ve güvenli yaz (atomic write via temp file)
                string json = JsonUtility.ToJson(saveData, prettyPrint: true);
                string tempPath = SaveFilePath + ".tmp";

                File.WriteAllText(tempPath, json);
                if (File.Exists(SaveFilePath))
                {
                    File.Delete(SaveFilePath);
                }
                File.Move(tempPath, SaveFilePath);

                OnGameSaved?.Invoke();
                Debug.Log($"[SaveManager] Oyun başarıyla kaydedildi: {SaveFilePath}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveManager] Kaydetme hatası: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Kayıtlı oyunu diskten okuyup kervan ve dünya durumuna uygular.
        /// </summary>
        public bool LoadGame()
        {
            if (!HasSaveFile() || GameManager.Instance == null || _itemDatabase == null)
                return false;

            try
            {
                string json = File.ReadAllText(SaveFilePath);
                var saveData = JsonUtility.FromJson<SaveData>(json);

                if (saveData == null) return false;

                // 1. Kervanı Yeniden Kur
                var caravan = new Caravan(saveData.Akce, saveData.FoodSupplyKg, saveData.MerchantCombatSkill);
                caravan.SetMorale(saveData.Morale);

                // Binek ve muhafızları yükle
                caravan.AddMount(MountType.Camel, saveData.CamelCount);
                caravan.AddMount(MountType.Mule, saveData.MuleCount);
                caravan.AddMount(MountType.Horse, saveData.HorseCount);

                caravan.HireGuard(saveData.GuardCount);
                caravan.HireLaborer(saveData.LaborerCount);

                // Eşyaları envantere geri yükle
                foreach (var savedItem in saveData.CargoItems)
                {
                    var itemData = _itemDatabase.GetItemById(savedItem.ItemId);
                    if (itemData != null)
                    {
                        caravan.Cargo.TryAddItem(itemData.ToDomain(), savedItem.Quantity);
                    }
                }

                // 2. GameManager Durumunu Güncelle
                // Yeni şehri bul ve ayarla
                if (_cityDatabase != null)
                {
                    var cityData = _cityDatabase.GetCityById(saveData.CurrentCityId);
                    if (cityData != null)
                    {
                        GameManager.Instance.ArriveAtCity(cityData.ToDomain());
                    }
                }

                if (EspionageService.Instance != null)
                {
                    EspionageService.Instance.AddReputation(saveData.OttomanReputation - EspionageService.Instance.OttomanReputation);
                }

                OnGameLoaded?.Invoke();
                Debug.Log("[SaveManager] Kayıtlı oyun başarıyla yüklendi.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveManager] Yükleme hatası: {ex.Message}");
                return false;
            }
        }

        public void DeleteSaveFile()
        {
            if (File.Exists(SaveFilePath))
            {
                File.Delete(SaveFilePath);
            }
        }
    }
}
