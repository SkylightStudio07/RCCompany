using RCCom.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace RCCom.UI
{
    /// <summary>
    /// Configuration 화면의 Master/BGM/SFX 볼륨 슬라이더 배선. 실제 볼륨 반영/저장은
    /// SoundManager가 전담 — 이 스크립트는 슬라이더 값 ↔ SoundManager 메서드만 연결한다.
    /// 슬라이더를 움직이는 즉시 미리듣기가 되도록 SoundManager.SetXVolume이 바로 반영하고,
    /// 디스크 저장(PlayerPrefs.Save)은 APPLY 버튼을 눌렀을 때만 확정한다.
    /// </summary>
    public class AudioSettingsUI : MonoBehaviour
    {
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider bgmVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Button applyButton;

        private void OnEnable()
        {
            if (SoundManager.Instance == null)
            {
                return;
            }

            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.SetValueWithoutNotify(SoundManager.Instance.MasterVolume);
                masterVolumeSlider.onValueChanged.AddListener(SoundManager.Instance.SetMasterVolume);
            }

            if (bgmVolumeSlider != null)
            {
                bgmVolumeSlider.SetValueWithoutNotify(SoundManager.Instance.BgmVolume);
                bgmVolumeSlider.onValueChanged.AddListener(SoundManager.Instance.SetBgmVolume);
            }

            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.SetValueWithoutNotify(SoundManager.Instance.SfxVolume);
                sfxVolumeSlider.onValueChanged.AddListener(SoundManager.Instance.SetSfxVolume);
            }

            if (applyButton != null)
            {
                applyButton.onClick.AddListener(HandleApply);
            }
        }

        private void OnDisable()
        {
            if (SoundManager.Instance == null)
            {
                return;
            }

            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.onValueChanged.RemoveListener(SoundManager.Instance.SetMasterVolume);
            }

            if (bgmVolumeSlider != null)
            {
                bgmVolumeSlider.onValueChanged.RemoveListener(SoundManager.Instance.SetBgmVolume);
            }

            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.onValueChanged.RemoveListener(SoundManager.Instance.SetSfxVolume);
            }

            if (applyButton != null)
            {
                applyButton.onClick.RemoveListener(HandleApply);
            }
        }

        private void HandleApply()
        {
            if (SoundManager.Instance == null)
            {
                return;
            }

            SoundManager.Instance.PlaySettingsButtonClick();
            SoundManager.Instance.SaveVolumeSettings();
        }
    }
}
