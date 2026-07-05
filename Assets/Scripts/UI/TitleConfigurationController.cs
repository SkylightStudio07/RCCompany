using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RCCom.UI
{
    public class TitleConfigurationController : MonoBehaviour
    {
        private const string MasterVolumeKey = "Settings.MasterVolume";
        private const string BgmVolumeKey = "Settings.BGMVolume";
        private const string SfxVolumeKey = "Settings.SFXVolume";
        private const string ScreenModeKey = "Settings.ScreenMode";
        private const string VSyncKey = "Settings.VSync";

        private enum ScreenMode
        {
            Windowed,
            Fullscreen
        }

        [Header("Panels")]
        [SerializeField] private GameObject configurationBackground;
        [SerializeField] private GameObject mainMenuBackground;

        [Header("Audio")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider bgmVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        [Header("Display Text")]
        [SerializeField] private TextMeshProUGUI screenModeText;
        [SerializeField] private TextMeshProUGUI resolutionText;
        [SerializeField] private TextMeshProUGUI vSyncText;

        [Header("Transition")]
        [SerializeField] private float transitionDuration = 0.45f;
        [SerializeField] private Vector2 menuExitOffset = new(-90f, 0f);
        [SerializeField] private Vector2 configurationEnterOffset = new(90f, 0f);
        [SerializeField] private float scaleBump = 0.025f;

        private ScreenMode pendingScreenMode;
        private bool pendingVSync;
        private bool initialized;
        private bool isConfigurationOpen;
        private bool isTransitioning;
        private bool isOpeningFromInactive;
        private RectTransform configurationRect;
        private RectTransform mainMenuRect;
        private CanvasGroup configurationGroup;
        private CanvasGroup mainMenuGroup;
        private Vector2 configurationStartPosition;
        private Vector2 mainMenuStartPosition;
        private Vector3 configurationStartScale;
        private Vector3 mainMenuStartScale;

        private void Awake()
        {
            Initialize();

            if (!isOpeningFromInactive)
            {
                HideImmediately();
            }
        }

        private void Initialize()
        {
            if (initialized)
            {
                return;
            }

            if (configurationBackground == null)
            {
                configurationBackground = gameObject;
            }

            configurationRect = configurationBackground != null ? configurationBackground.GetComponent<RectTransform>() : null;
            mainMenuRect = mainMenuBackground != null ? mainMenuBackground.GetComponent<RectTransform>() : null;
            configurationGroup = GetOrAddCanvasGroup(configurationBackground);
            mainMenuGroup = GetOrAddCanvasGroup(mainMenuBackground);

            configurationStartPosition = configurationRect != null ? configurationRect.anchoredPosition : Vector2.zero;
            mainMenuStartPosition = mainMenuRect != null ? mainMenuRect.anchoredPosition : Vector2.zero;
            configurationStartScale = configurationRect != null ? configurationRect.localScale : Vector3.one;
            mainMenuStartScale = mainMenuRect != null ? mainMenuRect.localScale : Vector3.one;

            LoadSettings();
            initialized = true;
        }

        public void Open()
        {
            Initialize();

            if (isConfigurationOpen || isTransitioning)
            {
                return;
            }

            RefreshLabels();

            if (configurationBackground != null)
            {
                isOpeningFromInactive = !configurationBackground.activeInHierarchy;
                configurationBackground.SetActive(true);
                isOpeningFromInactive = false;
            }

            StartCoroutine(PlayOpenTransition());
        }

        public void Close()
        {
            Initialize();

            if (!isConfigurationOpen || isTransitioning)
            {
                return;
            }

            if (mainMenuBackground != null)
            {
                mainMenuBackground.SetActive(true);
            }

            StartCoroutine(PlayCloseTransition());
        }

        public void Apply()
        {
            Initialize();

            float masterVolume = masterVolumeSlider != null ? masterVolumeSlider.value : AudioListener.volume;
            float bgmVolume = bgmVolumeSlider != null ? bgmVolumeSlider.value : PlayerPrefs.GetFloat(BgmVolumeKey, 1f);
            float sfxVolume = sfxVolumeSlider != null ? sfxVolumeSlider.value : PlayerPrefs.GetFloat(SfxVolumeKey, 1f);

            AudioListener.volume = masterVolume;
            QualitySettings.vSyncCount = pendingVSync ? 1 : 0;
            Screen.fullScreenMode = pendingScreenMode == ScreenMode.Fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
            Screen.fullScreen = pendingScreenMode == ScreenMode.Fullscreen;

            PlayerPrefs.SetFloat(MasterVolumeKey, masterVolume);
            PlayerPrefs.SetFloat(BgmVolumeKey, bgmVolume);
            PlayerPrefs.SetFloat(SfxVolumeKey, sfxVolume);
            PlayerPrefs.SetInt(ScreenModeKey, (int)pendingScreenMode);
            PlayerPrefs.SetInt(VSyncKey, pendingVSync ? 1 : 0);
            PlayerPrefs.Save();

            RefreshLabels();
        }

        public void PreviousScreenMode()
        {
            ToggleScreenMode();
        }

        public void NextScreenMode()
        {
            ToggleScreenMode();
        }

        public void PreviousVSync()
        {
            ToggleVSync();
        }

        public void NextVSync()
        {
            ToggleVSync();
        }

        public void PreviousResolution()
        {
            RefreshLabels();
        }

        public void NextResolution()
        {
            RefreshLabels();
        }

        private void LoadSettings()
        {
            float masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, AudioListener.volume);
            float bgmVolume = PlayerPrefs.GetFloat(BgmVolumeKey, 1f);
            float sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);

            SetSliderValue(masterVolumeSlider, masterVolume);
            SetSliderValue(bgmVolumeSlider, bgmVolume);
            SetSliderValue(sfxVolumeSlider, sfxVolume);

            pendingScreenMode = (ScreenMode)PlayerPrefs.GetInt(ScreenModeKey, Screen.fullScreen ? (int)ScreenMode.Fullscreen : (int)ScreenMode.Windowed);
            pendingVSync = PlayerPrefs.GetInt(VSyncKey, QualitySettings.vSyncCount > 0 ? 1 : 0) == 1;

            AudioListener.volume = masterVolume;
            RefreshLabels();
        }

        private void ToggleScreenMode()
        {
            pendingScreenMode = pendingScreenMode == ScreenMode.Windowed ? ScreenMode.Fullscreen : ScreenMode.Windowed;
            RefreshLabels();
        }

        private void ToggleVSync()
        {
            pendingVSync = !pendingVSync;
            RefreshLabels();
        }

        private void RefreshLabels()
        {
            if (screenModeText != null)
            {
                screenModeText.text = pendingScreenMode == ScreenMode.Fullscreen ? "Fullscreen" : "Windowed";
            }

            if (resolutionText != null)
            {
                resolutionText.text = $"{Screen.width} x {Screen.height}";
            }

            if (vSyncText != null)
            {
                vSyncText.text = pendingVSync ? "ON" : "OFF";
            }
        }

        private void HideImmediately()
        {
            isConfigurationOpen = false;
            isTransitioning = false;

            SetCanvasGroup(configurationGroup, 0f, false);
            SetCanvasGroup(mainMenuGroup, 1f, true);

            if (configurationRect != null)
            {
                configurationRect.anchoredPosition = configurationStartPosition;
                configurationRect.localScale = configurationStartScale;
            }

            if (mainMenuRect != null)
            {
                mainMenuRect.anchoredPosition = mainMenuStartPosition;
                mainMenuRect.localScale = mainMenuStartScale;
            }

            if (configurationBackground != null)
            {
                configurationBackground.SetActive(false);
            }

            if (mainMenuBackground != null)
            {
                mainMenuBackground.SetActive(true);
            }
        }

        private IEnumerator PlayOpenTransition()
        {
            isTransitioning = true;

            if (mainMenuBackground != null)
            {
                mainMenuBackground.SetActive(true);
            }

            SetCanvasGroup(mainMenuGroup, 1f, false);
            SetCanvasGroup(configurationGroup, 0f, false);

            if (mainMenuRect != null)
            {
                mainMenuRect.anchoredPosition = mainMenuStartPosition;
                mainMenuRect.localScale = mainMenuStartScale;
            }

            if (configurationRect != null)
            {
                configurationRect.anchoredPosition = configurationStartPosition + configurationEnterOffset;
                configurationRect.localScale = configurationStartScale * (1f + scaleBump);
            }

            float elapsed = 0f;
            while (elapsed < transitionDuration)
            {
                float t = transitionDuration > 0f ? Mathf.Clamp01(elapsed / transitionDuration) : 1f;
                ApplyTransition(EaseOutCubic(t));
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            ApplyTransition(1f);
            SetCanvasGroup(mainMenuGroup, 0f, false);
            SetCanvasGroup(configurationGroup, 1f, true);

            if (mainMenuBackground != null)
            {
                mainMenuBackground.SetActive(false);
            }

            isConfigurationOpen = true;
            isTransitioning = false;
        }

        private IEnumerator PlayCloseTransition()
        {
            isTransitioning = true;

            if (mainMenuBackground != null)
            {
                mainMenuBackground.SetActive(true);
            }

            SetCanvasGroup(mainMenuGroup, 0f, false);
            SetCanvasGroup(configurationGroup, 1f, false);

            if (mainMenuRect != null)
            {
                mainMenuRect.anchoredPosition = mainMenuStartPosition + menuExitOffset;
                mainMenuRect.localScale = mainMenuStartScale * (1f + scaleBump);
            }

            if (configurationRect != null)
            {
                configurationRect.anchoredPosition = configurationStartPosition;
                configurationRect.localScale = configurationStartScale;
            }

            float elapsed = 0f;
            while (elapsed < transitionDuration)
            {
                float t = transitionDuration > 0f ? Mathf.Clamp01(elapsed / transitionDuration) : 1f;
                ApplyTransition(1f - EaseOutCubic(t));
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            ApplyTransition(0f);
            SetCanvasGroup(mainMenuGroup, 1f, true);
            SetCanvasGroup(configurationGroup, 0f, false);

            if (configurationBackground != null)
            {
                configurationBackground.SetActive(false);
            }

            isConfigurationOpen = false;
            isTransitioning = false;
        }

        private void ApplyTransition(float t)
        {
            if (mainMenuRect != null)
            {
                mainMenuRect.anchoredPosition = Vector2.LerpUnclamped(mainMenuStartPosition, mainMenuStartPosition + menuExitOffset, t);
                mainMenuRect.localScale = Vector3.LerpUnclamped(mainMenuStartScale, mainMenuStartScale * (1f + scaleBump), t);
            }

            if (configurationRect != null)
            {
                configurationRect.anchoredPosition = Vector2.LerpUnclamped(configurationStartPosition + configurationEnterOffset, configurationStartPosition, t);
                configurationRect.localScale = Vector3.LerpUnclamped(configurationStartScale * (1f + scaleBump), configurationStartScale, t);
            }

            SetCanvasGroup(mainMenuGroup, 1f - t, false);
            SetCanvasGroup(configurationGroup, t, false);
        }

        private static CanvasGroup GetOrAddCanvasGroup(GameObject target)
        {
            if (target == null)
            {
                return null;
            }

            if (!target.TryGetComponent(out CanvasGroup group))
            {
                group = target.AddComponent<CanvasGroup>();
            }

            return group;
        }

        private static void SetCanvasGroup(CanvasGroup group, float alpha, bool interactive)
        {
            if (group == null)
            {
                return;
            }

            group.alpha = alpha;
            group.interactable = interactive;
            group.blocksRaycasts = interactive;
        }

        private static float EaseOutCubic(float t)
        {
            float inverse = 1f - Mathf.Clamp01(t);
            return 1f - inverse * inverse * inverse;
        }

        private static void SetSliderValue(Slider slider, float value)
        {
            if (slider == null)
            {
                return;
            }

            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = Mathf.Clamp01(value);
        }
    }
}
