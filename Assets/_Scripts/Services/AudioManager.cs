using System.Collections;
using UnityEngine;

namespace Kervan.Services
{
    /// <summary>
    /// 14. yüzyıl atmosferini yansıtan arka plan müziklerini (Ney, kopuz, bendir)
    /// ve ses efektlerini (Akçe şıngırtısı, deve adımları, kılıç sesleri) yöneten ses servisi.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; } = null!;

        [Header("Ses Kaynakları (Audio Sources)")]
        [SerializeField] private AudioSource? _musicSource;
        [SerializeField] private AudioSource? _sfxSource;

        [Header("Müzik Klipleri")]
        [SerializeField] private AudioClip? _cityAmbientMusic;   // Şehir & Pazar huzurlu müzik
        [SerializeField] private AudioClip? _travelMusic;         // Yolda geçen seyahat ezgisi
        [SerializeField] private AudioClip? _combatTensionMusic;  // Haydut pususu gerilim müziği

        [Header("Ses Efektleri (SFX)")]
        [SerializeField] private AudioClip? _coinRattleSfx;       // Akçe / alışveriş sesi
        [SerializeField] private AudioClip? _caravanFootstepsSfx; // Yük hayvanı adımları
        [SerializeField] private AudioClip? _swordClashSfx;       // Kılıç çarpışması / çatışma
        [SerializeField] private AudioClip? _buttonClickSfx;      // Parşömen buton tıklaması
        [SerializeField] private AudioClip? _victoryFanfareSfx;   // Başarılı ticaret veya zafer

        [Header("Ses Seviyeleri (0.0 - 1.0)")]
        [Range(0f, 1f)] [SerializeField] private float _masterVolume = 1.0f;
        [Range(0f, 1f)] [SerializeField] private float _musicVolume = 0.7f;
        [Range(0f, 1f)] [SerializeField] private float _sfxVolume = 1.0f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            PlayCityMusic();
        }

        // --- MÜZİK KONTROLLERİ ---

        public void PlayCityMusic()
        {
            CrossFadeMusic(_cityAmbientMusic);
        }

        public void PlayTravelMusic()
        {
            CrossFadeMusic(_travelMusic);
        }

        public void PlayCombatMusic()
        {
            CrossFadeMusic(_combatTensionMusic);
        }

        private void CrossFadeMusic(AudioClip? newClip)
        {
            if (_musicSource == null || newClip == null || _musicSource.clip == newClip)
                return;

            StopAllCoroutines();
            StartCoroutine(FadeMusicRoutine(newClip));
        }

        private IEnumerator FadeMusicRoutine(AudioClip newClip)
        {
            float duration = 1.0f;
            float startVol = _musicSource!.volume;

            // Fade Out
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                _musicSource.volume = Mathf.Lerp(startVol, 0f, t / duration);
                yield return null;
            }

            _musicSource.clip = newClip;
            _musicSource.Play();

            // Fade In
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                _musicSource.volume = Mathf.Lerp(0f, _musicVolume * _masterVolume, t / duration);
                yield return null;
            }

            _musicSource.volume = _musicVolume * _masterVolume;
        }

        // --- SES EFEKTİ ÇALMA METOTLARI ---

        public void PlayCoinSound() => PlaySfx(_coinRattleSfx);
        public void PlayFootsteps() => PlaySfx(_caravanFootstepsSfx);
        public void PlaySwordClash() => PlaySfx(_swordClashSfx);
        public void PlayButtonClick() => PlaySfx(_buttonClickSfx);
        public void PlayVictory() => PlaySfx(_victoryFanfareSfx);

        public void PlaySfx(AudioClip? clip, float volumeScale = 1.0f)
        {
            if (_sfxSource == null || clip == null) return;
            _sfxSource.PlayOneShot(clip, volumeScale * _sfxVolume * _masterVolume);
        }

        // --- AYARLAR ---

        public void SetMasterVolume(float vol)
        {
            _masterVolume = Mathf.Clamp01(vol);
            if (_musicSource != null) _musicSource.volume = _musicVolume * _masterVolume;
        }

        public void SetMusicVolume(float vol)
        {
            _musicVolume = Mathf.Clamp01(vol);
            if (_musicSource != null) _musicSource.volume = _musicVolume * _masterVolume;
        }

        public void SetSfxVolume(float vol)
        {
            _sfxVolume = Mathf.Clamp01(vol);
        }
    }
}
