using RCCom.Data;
using RCCom.Definitions.Tower;
using RCCom.Runtime;
using UnityEngine;

namespace RCCom.Effects.Card.Concrete
{
    /// <summary>
    /// 카드13~15 "OO 타워 해금" 공용 클래스. 인스펙터에 미리 만들어둔 신규 타워 Definition을
    /// 연결해두면, 선택 시 TowerRoster에 추가해 건설 가능하게 만든다 (같은 카드를 다시 뽑아도
    /// 중복 추가되지 않도록 방어). displayName/description은 카드마다 달라서 에셋 생성 시
    /// 직접 채울 것 (예: "관통 사격 타워 해금" / "신규 공격 타워: 일직선 적 관통 데미지").
    ///
    /// unlockDefinition은 각각 PierceDamageEffect/SlowAuraEffect/BaseRegenEffect를 사용하는
    /// AttackTowerDefinition/SkillTowerDefinition/PowerTowerDefinition 에셋을 미리 만들어서
    /// 연결해둘 것 (사용자 몫 — Editor에서 에셋 제작 필요, 프리팹은 공용 1개라 새로 안 만들어도 됨).
    /// </summary>
    [CreateAssetMenu(menuName = "RCCom/Card/Unlock Tower Card")]
    public class UnlockTowerCard : CardEffectBase
    {
        [SerializeField] private TowerDefinition unlockDefinition;

        /// <summary>해금할 타워 자체의 종류를 그대로 카드 프레임 판단 기준으로 사용 — 값을 두 군데 따로 관리하지 않음.</summary>
        public override TowerKind Category => unlockDefinition != null ? unlockDefinition.Data.kind : base.Category;

        /// <summary>
        /// 이미 해금된 타워라면 다시 뽑아도 의미가 없으니 후보에서 제외. towerRoster.towers는
        /// 세션 전용 복제본들을 담고 있으므로(TowerRoster.GetRuntimeInstance 참고), 원본
        /// unlockDefinition이 아니라 그 복제본과 비교해야 정확히 매칭된다.
        /// </summary>
        public override bool IsAvailable(CardContext ctx) => !ctx.towerRoster.towers.Contains(unlockDefinition.CreateRuntimeInstance());

        public override void Apply(CardContext ctx)
        {
            TowerDefinition runtimeDefinition = unlockDefinition.CreateRuntimeInstance();

            if (!ctx.towerRoster.towers.Contains(runtimeDefinition))
            {
                ctx.towerRoster.towers.Add(runtimeDefinition);
            }
        }

        /// <summary>재시작(Retry) 시 GameManager가 호출 — 해금 대상 Definition의 복제본 캐시도 비워야 함.</summary>
        public void ClearRuntimeCache()
        {
            if (unlockDefinition != null)
            {
                unlockDefinition.ClearRuntimeInstance();
            }
        }
    }
}
