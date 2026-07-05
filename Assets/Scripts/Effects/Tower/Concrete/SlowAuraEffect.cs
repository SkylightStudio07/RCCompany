using RCCom.Runtime;
using UnityEngine;

namespace RCCom.Effects.Tower.Concrete
{
    /// <summary>
    /// 카드14 "빙결 오라 타워 해금" 전용 신규 스킬 타워 효과. AuraBuffEffect(아군 버프)와 달리
    /// 사거리 내 "적"의 이동속도를 감소시킨다. Tower는 아군 출입에만 OnAllyEnterRange/Exit
    /// 훅이 있고 적 출입 훅은 없어서, 대신 OnTick마다 사거리 안의 모든 적에게 짧은 지속시간의
    /// 슬로우를 계속 갱신하는 방식으로 구현 — 사거리를 벗어나면 갱신이 끊겨 곧 자연 만료된다.
    /// </summary>
    [CreateAssetMenu(menuName = "RCCom/Tower/Effects/Slow Aura Effect")]
    public class SlowAuraEffect : TowerEffectBase
    {
        [SerializeField] private float speedMultiplier = 0.5f;
        [SerializeField] private float refreshDuration = 0.5f;

        public override void OnTick(TowerContext ctx)
        {
            foreach (EnemyInstance enemy in ctx.activeEnemies)
            {
                enemy.ApplySpeedMultiplier(speedMultiplier, refreshDuration);
            }
        }
    }
}
