using RCCom.Managers;
using RCCom.Runtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RCCom.UI
{
    /// <summary>
    /// Day2 UI/HUD 체크리스트(플레이어/거점 체력바, 골드, 웨이브 번호)를 담당하는 화면
    /// 오버레이. 매 프레임 값을 다시 읽어서 갱신하는 가장 단순한 방식 — 이벤트 배선을 늘리는
    /// 대신 이 편이 빠르고, 최대체력이 카드로 바뀌는 경우도 자동으로 따라간다.
    /// </summary>
    public class HudController : MonoBehaviour
    {
        [SerializeField] private PlayerController player;
        [SerializeField] private BaseController baseController;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private WaveManager waveManager;
        [SerializeField] private MapManager mapManager;

        [Header("UI 참조")]
        [SerializeField] private Slider playerHealthBar;
        [SerializeField] private Slider baseHealthBar;
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private TextMeshProUGUI waveText;

        [Header("레벨/경험치")]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Slider expBar;
        [SerializeField] private TextMeshProUGUI expText;

        /// <summary>웨이브 사이 대기(빌드 페이즈) 중에만 보여줄 카운트다운. 스폰 진행 중엔 숨김.</summary>
        [SerializeField] private TextMeshProUGUI waveCountdownText;

        [Header("스킬: 오버드라이브 모드")]
        [SerializeField] private TextMeshProUGUI skillReadyText;
        [SerializeField] private TextMeshProUGUI overdriveText;
        [SerializeField] private Slider skillGaugeBar;

        [Header("타워 슬롯 (남은 설치 가능 수) — 1: 공격 / 2: 스킬 / 3: 파워")]
        [SerializeField] private TextMeshProUGUI slotText1;
        [SerializeField] private TextMeshProUGUI slotText2;
        [SerializeField] private TextMeshProUGUI slotText3;

        private void Update()
        {
            playerHealthBar.maxValue = player.data.maxHealth;
            playerHealthBar.value = player.CurrentHealth;

            baseHealthBar.maxValue = baseController.maxHealth;
            baseHealthBar.value = baseController.CurrentHealth;

            goldText.text = gameManager.Gold.ToString();
            waveText.text = $"{waveManager.CurrentWave}";

            UpdateWaveCountdown();
            UpdateExp();
            UpdateSkill();
            UpdateSlots();
        }

        private void UpdateSlots()
        {
            if (slotText1 != null)
            {
                slotText1.text = $"{mapManager.AttackSlotsRemaining}";
            }

            if (slotText2 != null)
            {
                slotText2.text = $"{mapManager.SkillSlotsRemaining}";
            }

            if (slotText3 != null)
            {
                slotText3.text = $"{mapManager.PowerSlotsRemaining}";
            }
        }

        private void UpdateSkill()
        {
            if (skillReadyText != null)
            {
                skillReadyText.gameObject.SetActive(player.IsSkillReady);
                if (player.IsSkillReady)
                {
                    skillReadyText.text = "준비";
                }
            }

            if (overdriveText != null)
            {
                overdriveText.gameObject.SetActive(player.IsSkillActive);
                if (player.IsSkillActive)
                {
                    overdriveText.text = "오버드라이브 모드";
                }
            }

            if (skillGaugeBar != null)
            {
                skillGaugeBar.maxValue = player.SkillCooldownDuration;
                skillGaugeBar.value = player.SkillCooldownDuration - player.SkillCooldownRemaining;
            }
        }

        private void UpdateExp()
        {
            int required = gameManager.ExpRequiredForNextLevel();

            levelText.text = $"{gameManager.Level:D2}";
            expBar.maxValue = required;
            expBar.value = gameManager.CurrentExp;
            expText.text = $"{gameManager.CurrentExp} / {required}";
        }

        private void UpdateWaveCountdown()
        {
            bool isWaiting = waveManager.IsWaitingForNextWave;
            waveCountdownText.gameObject.SetActive(isWaiting);

            if (isWaiting)
            {
                waveCountdownText.text = $"다음 웨이브까지 {waveManager.NextWaveCountdown:F1}초";
            }
        }
    }
}
