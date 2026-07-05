using RCCom.Data;
using RCCom.Definitions.Tower;
using RCCom.Effects.Tower;
using RCCom.Runtime;
using UnityEngine;

namespace RCCom.Effects.Tower.Concrete
{
    /// <summary>
    /// 파워 타워 기본 효과: 건설 즉시 전체 공격 타워에게 고정치 데미지 버프를 전역 적용한다.
    /// 사거리 개념이 없으므로 OnAllyEnterRange/OnAllyExitRange 대신 OnBuild 1회만 사용하고,
    /// 스킬 타워와 동일한 ITowerAura 파이프라인(GlobalTowerAuraRegistry)에 등록해 데미지 계산
    /// 로직을 하나로 통일한다.
    /// </summary>
    [CreateAssetMenu(menuName = "RCCom/Tower/Effects/Global Buff Effect")]
    public class GlobalBuffEffect : TowerEffectBase
    {
        public override void OnBuild(TowerContext ctx)
        {
            if (ctx.self.Data is not PowerTowerData data)
            {
                return;
            }

            GlobalTowerAuraRegistry.Auras.Add(new GlobalDamageAura(data.globalDamageBonus));
        }

        /// <summary>
        /// 업그레이드 카드(축적된 힘)가 호출: 이미 지어진 파워 타워들이 등록해둔 오라의 실제
        /// 기여치를 늘리고(즉시 반영), 앞으로 새로 지어질 파워 타워도 늘어난 값으로 시작하도록
        /// Definition 쪽 기본값도 함께 늘린다.
        /// </summary>
        public static void IncreaseAllGlobalBonuses(TowerRoster roster, float amount)
        {
            foreach (ITowerAura aura in GlobalTowerAuraRegistry.Auras)
            {
                if (aura is GlobalDamageAura globalAura)
                {
                    globalAura.bonus += amount;
                }
            }

            foreach (TowerDefinition definition in roster.towers)
            {
                if (definition.Data is PowerTowerData data)
                {
                    data.globalDamageBonus += amount;
                }
            }
        }

        private class GlobalDamageAura : ITowerAura
        {
            public float bonus;

            public GlobalDamageAura(float bonus)
            {
                this.bonus = bonus;
            }

            public float ModifyOutgoingDamage(float baseDamage) => baseDamage + bonus;

            public float ModifyAttackInterval(float baseInterval) => baseInterval;
        }
    }
}
