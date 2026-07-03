using RCCom.Core;
using RCCom.Data;
using RCCom.Effects.Tower;
using RCCom.Runtime;
using UnityEngine;

namespace RCCom.Effects.Tower.Concrete
{
    /// <summary>
    /// 공격 타워 기본 효과: 사거리 내 가장 가까운 적에게 주기적으로 데미지를 입힌다.
    /// 투사체 등 시각 표현은 여기서 다루지 않는다 (렌더러 프리팹/매니저 단계에서 별도 처리).
    /// </summary>
    [CreateAssetMenu(menuName = "RCCom/Tower/Effects/Damage Effect")]
    public class DamageEffect : TowerEffectBase
    {
        public override void OnTick(TowerContext ctx)
        {
            if (ctx.self.Data is not AttackTowerData data)
            {
                return;
            }

            ctx.self.cooldownRemaining -= ctx.deltaTime;
            if (ctx.self.cooldownRemaining > 0f)
            {
                return;
            }

            EnemyInstance target = EnemyTargeting.FindNearestInRange(ctx.activeEnemies, ctx.self.Position, data.attackRange);
            if (target == null)
            {
                return;
            }

            target.TakeDamage(CalculateDamage(ctx.self, data.damage));
            ctx.self.cooldownRemaining = CalculateAttackInterval(ctx.self, data.attackInterval);
        }

        private static float CalculateDamage(TowerInstance self, float baseDamage)
        {
            float damage = baseDamage;

            foreach (ITowerAura aura in self.activeAuras)
            {
                damage = aura.ModifyOutgoingDamage(damage);
            }

            foreach (ITowerAura aura in GlobalTowerAuraRegistry.Auras)
            {
                damage = aura.ModifyOutgoingDamage(damage);
            }

            return damage;
        }

        private static float CalculateAttackInterval(TowerInstance self, float baseInterval)
        {
            float interval = baseInterval;

            foreach (ITowerAura aura in self.activeAuras)
            {
                interval = aura.ModifyAttackInterval(interval);
            }

            foreach (ITowerAura aura in GlobalTowerAuraRegistry.Auras)
            {
                interval = aura.ModifyAttackInterval(interval);
            }

            return interval;
        }
    }
}
