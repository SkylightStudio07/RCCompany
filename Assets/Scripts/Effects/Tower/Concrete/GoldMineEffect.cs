using RCCom.Managers;
using RCCom.Runtime;
using UnityEngine;

namespace RCCom.Effects.Tower.Concrete
{
    /// <summary>
    /// 파워 타워 신규 효과("골드 채굴"): BaseRegenEffect와 완전히 같은 패턴으로, 건설되어 있는
    /// 동안 일정 주기마다 소량 골드를 자동 생산한다. 씬에 GameManager가 하나뿐임이 보장되므로
    /// 정적 싱글톤을 직접 참조한다 (BaseController.Instance와 동일한 근거).
    ///
    /// 주기(누적) 타이머는 효과 자산(SO) 자신이 아니라 ctx.self.cooldownRemaining(타워 인스턴스별
    /// 상태)에 저장한다 — 효과 SO는 여러 타워 인스턴스가 공유하는 상태 없는 자산이어야 하므로,
    /// SO에 직접 누적값을 두면 같은 정의를 쓰는 여러 골드 채굴 타워끼리 타이머가 섞이는 버그가 생긴다.
    /// </summary>
    [CreateAssetMenu(menuName = "RCCom/Tower/Effects/Gold Mine Effect")]
    public class GoldMineEffect : TowerEffectBase
    {
        [SerializeField] private int goldPerTick = 1;
        [SerializeField] private float tickInterval = 1f;

        public override void OnTick(TowerContext ctx)
        {
            ctx.self.cooldownRemaining -= ctx.deltaTime;
            if (ctx.self.cooldownRemaining > 0f)
            {
                return;
            }

            GameManager.Instance?.AddGold(goldPerTick);
            ctx.self.cooldownRemaining = tickInterval;
        }
    }
}
