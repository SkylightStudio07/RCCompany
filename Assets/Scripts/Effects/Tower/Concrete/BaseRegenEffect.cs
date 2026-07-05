using RCCom.Runtime;
using UnityEngine;

namespace RCCom.Effects.Tower.Concrete
{
    /// <summary>
    /// 카드15 "재생의 축 해금" 전용 신규 파워 타워 효과. GlobalBuffEffect(1회성 OnBuild)와
    /// 달리 매 틱마다 거점 체력을 초당 소량 회복시킨다 (사거리 개념 없음, 건설되어 있는 동안
    /// 계속 적용). 씬에 거점이 하나뿐임이 보장되므로 BaseController.Instance를 직접 참조한다.
    /// </summary>
    [CreateAssetMenu(menuName = "RCCom/Tower/Effects/Base Regen Effect")]
    public class BaseRegenEffect : TowerEffectBase
    {
        [SerializeField] private float healPerSecond = 1f;

        public override void OnTick(TowerContext ctx)
        {
            BaseController.Instance?.Heal(healPerSecond * ctx.deltaTime);
        }
    }
}
