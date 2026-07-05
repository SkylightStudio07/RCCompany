using RCCom.Runtime;
using UnityEngine;

namespace RCCom.Effects.Tower.Concrete
{
    /// <summary>
    /// 파워 타워 신규 효과("전역 가속"): 건설 즉시 전체 공격 타워의 공격속도를 전역으로
    /// 올린다. GlobalBuffEffect(전역 데미지 버프)와 완전히 같은 패턴 — GlobalTowerAuraRegistry에
    /// ITowerAura를 등록만 하면, TowerDamageMath.CalculateAttackInterval이 이미 전역 오라를
    /// 조회하도록 되어 있어 별도 배선 없이 그대로 반영된다.
    /// </summary>
    [CreateAssetMenu(menuName = "RCCom/Tower/Effects/Global Attack Speed Effect")]
    public class GlobalAttackSpeedEffect : TowerEffectBase
    {
        [SerializeField] private float attackSpeedMultiplier = 1.2f;

        public override void OnBuild(TowerContext ctx)
        {
            GlobalTowerAuraRegistry.Auras.Add(new GlobalSpeedAura(attackSpeedMultiplier));
        }

        private class GlobalSpeedAura : ITowerAura
        {
            private readonly float _attackSpeedMultiplier;

            public GlobalSpeedAura(float attackSpeedMultiplier)
            {
                _attackSpeedMultiplier = attackSpeedMultiplier;
            }

            public float ModifyOutgoingDamage(float baseDamage) => baseDamage;

            public float ModifyAttackInterval(float baseInterval) => baseInterval / _attackSpeedMultiplier;
        }
    }
}
