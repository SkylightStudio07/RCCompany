using RCCom.Runtime;
using UnityEngine;

namespace RCCom.Effects.Tower.Concrete
{
    /// <summary>
    /// 스킬 타워 신규 효과("취약 오라"): 아군 버프 대신 사거리 내 "적"이 받는 피해를 증가시킨다.
    /// SlowAuraEffect와 완전히 동일한 패턴 — Tower는 적 출입 훅이 없어서, OnTick마다 사거리
    /// 안의 적에게 짧은 지속시간의 취약 상태를 계속 갱신한다 (사거리 이탈 시 자연 만료).
    /// </summary>
    [CreateAssetMenu(menuName = "RCCom/Tower/Effects/Vulnerable Aura Effect")]
    public class VulnerableAuraEffect : TowerEffectBase
    {
        [SerializeField] private float damageTakenMultiplier = 1.3f;
        [SerializeField] private float refreshDuration = 0.5f;

        public override void OnTick(TowerContext ctx)
        {
            foreach (EnemyInstance enemy in ctx.activeEnemies)
            {
                enemy.ApplyVulnerable(damageTakenMultiplier, refreshDuration);
            }
        }
    }
}
