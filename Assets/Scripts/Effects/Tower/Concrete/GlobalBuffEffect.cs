using RCCom.Data;
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
        /// 업그레이드 카드(축적된 힘, upgradeIncrement)로 값이 늘어날 수 있으므로 bonus를
        /// 불변이 아닌 가변 필드로 둔다. 카드 시스템 구현 시 이 인스턴스를 찾아 bonus를 더해준다
        /// (현재는 등록만 하고, 카드 연동은 이후 업그레이드 카드 시스템 단계에서 처리).
        /// </summary>
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
