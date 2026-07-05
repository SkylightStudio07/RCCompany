using RCCom.Data;
using RCCom.Definitions.Tower;
using RCCom.Runtime;
using UnityEngine;

namespace RCCom.Effects.Card.Concrete
{
    /// <summary>
    /// 카드6 "오라 증폭": 스킬 타워 버프 수치 +25%. damageBuffMultiplier/attackSpeedBuffMultiplier는
    /// "1.0 = 버프 없음" 기준이라, 배율 전체가 아니라 보너스 부분(배율-1)만 25% 늘린다
    /// (예: +20% 버프 → +25%, 즉 1.2 → 1.25).
    /// </summary>
    [CreateAssetMenu(menuName = "RCCom/Card/Skill Tower Buff Boost Card")]
    public class SkillTowerBuffBoostCard : CardEffectBase
    {
        [SerializeField] private float boostMultiplier = 1.25f;

        private void Reset()
        {
            displayName = "오라 증폭";
            description = "스킬 타워 버프 수치 +25%";
        }

        public override void Apply(CardContext ctx)
        {
            foreach (TowerDefinition definition in ctx.towerRoster.towers)
            {
                if (definition.Data is SkillTowerData data)
                {
                    data.damageBuffMultiplier = 1f + (data.damageBuffMultiplier - 1f) * boostMultiplier;
                    data.attackSpeedBuffMultiplier = 1f + (data.attackSpeedBuffMultiplier - 1f) * boostMultiplier;
                }
            }
        }
    }
}
