using UnityEngine;

namespace Kervan.Presentation
{
    /// <summary>
    /// Mobil oyundaki ana paneller (Pazar, Kervan Yönetimi, Harita, İstihbarat)
    /// arasındaki ekran geçişlerini yöneten sunum kontrolcüsü.
    /// </summary>
    public class ScreenNavigationManager : MonoBehaviour
    {
        public static ScreenNavigationManager Instance { get; private set; } = null!;

        [Header("Ekran Panelleri (Canvas Groups / GameObjects)")]
        [SerializeField] private GameObject? _cityBazaarPanel;
        [SerializeField] private GameObject? _caravanManagementPanel;
        [SerializeField] private GameObject? _travelMapPanel;
        [SerializeField] private GameObject? _espionagePanel;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            // Oyun başladığında varsayılan olarak Şehir Pazarı ekranını aç
            OpenCityBazaar();
        }

        public void OpenCityBazaar()
        {
            ActivateOnly(_cityBazaarPanel);
        }

        public void OpenCaravanManagement()
        {
            ActivateOnly(_caravanManagementPanel);
        }

        public void OpenTravelMap()
        {
            ActivateOnly(_travelMapPanel);
        }

        public void OpenEspionage()
        {
            ActivateOnly(_espionagePanel);
        }

        private void ActivateOnly(GameObject? targetPanel)
        {
            if (_cityBazaarPanel != null) _cityBazaarPanel.SetActive(_cityBazaarPanel == targetPanel);
            if (_caravanManagementPanel != null) _caravanManagementPanel.SetActive(_caravanManagementPanel == targetPanel);
            if (_travelMapPanel != null) _travelMapPanel.SetActive(_travelMapPanel == targetPanel);
            if (_espionagePanel != null) _espionagePanel.SetActive(_espionagePanel == targetPanel);
        }
    }
}
