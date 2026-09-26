using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Kervan.Services;

namespace Kervan.Presentation
{
    /// <summary>
    /// Mobil arayüze profesyonel his (Game Feel / Polish) katan animasyon ve etkileşim bileşeni.
    /// Butonlara tıklandığında hafifçe esneme (Punch/Bounce) animasyonu ve parşömen tıklama sesi verir.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class UIPolishAndAnimations : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Tooltip("Tıklama esnasındaki ölçek küçülme oranı")]
        [SerializeField] private float _pressScale = 0.93f;

        [Tooltip("Animasyon hızı")]
        [SerializeField] private float _animationSpeed = 12f;

        private Vector3 _originalScale;
        private Coroutine? _scaleCoroutine;

        private void Awake()
        {
            _originalScale = transform.localScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            var btn = GetComponent<Button>();
            if (btn != null && !btn.interactable) return;

            // Ses Çal
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClick();
            }

            // Küçülme animasyonu
            StartScaleAnimation(_originalScale * _pressScale);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // Orijinal boyuta dönme
            StartScaleAnimation(_originalScale);
        }

        private void StartScaleAnimation(Vector3 targetScale)
        {
            if (_scaleCoroutine != null)
            {
                StopCoroutine(_scaleCoroutine);
            }

            _scaleCoroutine = StartCoroutine(ScaleRoutine(targetScale));
        }

        private IEnumerator ScaleRoutine(Vector3 target)
        {
            while (Vector3.Distance(transform.localScale, target) > 0.005f)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, target, Time.deltaTime * _animationSpeed);
                yield return null;
            }

            transform.localScale = target;
        }
    }
}
