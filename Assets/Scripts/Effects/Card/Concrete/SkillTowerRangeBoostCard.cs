using RCCom.Data;
using RCCom.Definitions.Tower;
using RCCom.Runtime;
using UnityEngine;

namespace RCCom.Effects.Card.Concrete
{
    /// <summary>카드7 "오라 확장": 스킬 타워 사거리 +20%. 콜라이더 반경도 즉시 갱신.</summary>
    [CreateAssetMenu(menuName = "RCCom/Card/Skill Tower Range Boost Card")]
    public class SkillTowerRangeBoostCard : CardEffectBase
    {
        [SerializeField] private float multiplier = 1.20f;

        private void Reset()
        {
            displayName = "오라 확장";
            description = "스킬 타워 사거리 +20%";
        }

        public override void Apply(CardContext ctx)
        {
            foreach (TowerDefinition definition in ctx.towerRoster.towers)
            {
                if (definition.Data is SkillTowerData data)
                {
                    data.buffRange *= multiplier;
                }
            }

            foreach (TowerInstance instance in TowerInstance.All)
            {
                instance.RefreshRangeCollider();
            }
        }
    }
}
