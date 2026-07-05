using RCCom.Data;
using RCCom.Definitions.Tower;
using RCCom.Runtime;
using UnityEngine;

namespace RCCom.Effects.Card.Concrete
{
    /// <summary>카드4 "포격 강화": 공격 타워 데미지 +20% (현재+향후 모든 공격 타워 종류에 적용).</summary>
    [CreateAssetMenu(menuName = "RCCom/Card/Attack Tower Damage Boost Card")]
    public class AttackTowerDamageBoostCard : CardEffectBase
    {
        [SerializeField] private float multiplier = 1.20f;

        private void Reset()
        {
            displayName = "포격 강화";
            description = "공격 타워 데미지 +20%";
        }

        public override void Apply(CardContext ctx)
        {
            foreach (TowerDefinition definition in ctx.towerRoster.towers)
            {
                if (definition.Data is AttackTowerData data)
                {
                    data.damage *= multiplier;
                }
            }
        }
    }
}
