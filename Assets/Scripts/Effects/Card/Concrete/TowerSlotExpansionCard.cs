using RCCom.Data;
using RCCom.Runtime;
using UnityEngine;

namespace RCCom.Effects.Card.Concrete
{
    /// <summary>
    /// 카드9~11 "OO 타워 증설" 공용 클래스. kind만 다르게 설정한 SO 에셋을 3개 만들어서
    /// (공격/스킬/파워) 각각 카드로 등록한다 — 로직이 완전히 같아서 클래스를 3개 두지 않음.
    /// displayName/description은 카드마다 달라서 Reset() 기본값을 안 두고, 에셋 생성 시
    /// 직접 채울 것 (예: "공격 타워 증설" / "공격 타워 최대 설치 수 +1").
    /// </summary>
    [CreateAssetMenu(menuName = "RCCom/Card/Tower Slot Expansion Card")]
    public class TowerSlotExpansionCard : CardEffectBase
    {
        [SerializeField] private TowerKind kind;
        [SerializeField] private int amount = 1;

        /// <summary>이미 있는 kind 필드를 그대로 카드 프레임 판단 기준으로도 사용 — 값을 두 군데 따로 관리하지 않음.</summary>
        public override TowerKind Category => kind;

        public override void Apply(CardContext ctx)
        {
            ctx.mapManager.IncreaseMaxSlots(kind, amount);
        }
    }
}
