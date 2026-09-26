using UnityEngine;

namespace Kervan.Presentation
{
    /// <summary>
    /// Mobil cihazlardaki çentik (notch), kamera deliği ve ana ekran çubuğu gibi
    /// donanımsal alanları algılayarak arayüzün güvenli alana (Safe Area) tam oturmasını sağlar.
    /// Modern Android ve iOS cihazlar için endüstri standardı bir çözümdür.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaHandler : MonoBehaviour
    {
        private RectTransform _rectTransform = null!;
        private Rect _lastSafeArea = Rect.zero;
        private Vector2Int _lastScreenSize = Vector2Int.zero;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            ApplySafeArea();
        }

        private void Update()
        {
            // Ekran döndürüldüğünde veya çözünürlük değiştiğinde güncelle
            if (_lastSafeArea != Screen.safeArea || _lastScreenSize.x != Screen.width || _lastScreenSize.y != Screen.height)
            {
                ApplySafeArea();
            }
        }

        private void ApplySafeArea()
        {
            if (_rectTransform == null) return;

            Rect safeArea = Screen.safeArea;
            _lastSafeArea = safeArea;
            _lastScreenSize = new Vector2Int(Screen.width, Screen.height);

            // Anchor min ve max değerlerini ekran oranına göre normalize et
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            _rectTransform.anchorMin = anchorMin;
            _rectTransform.anchorMax = anchorMax;
        }
    }
}
