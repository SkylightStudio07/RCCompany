using RCCom.Runtime;
using UnityEngine;

namespace RCCom.Effects.Card.Concrete
{
    /// <summary>카드3 "스킬 재사용 단축": 플레이어 스킬 쿨다운 -15%.</summary>
    [CreateAssetMenu(menuName = "RCCom/Card/Player Skill Cooldown Boost Card")]
    public class PlayerSkillCooldownBoostCard : CardEffectBase
    {
        [SerializeField] private float multiplier = 0.85f;

        private void Reset()
        {
            displayName = "스킬 재사용 단축";
            description = "플레이어 스킬 쿨다운 -15%";
        }

        public override void Apply(CardContext ctx)
        {
            ctx.player.data.skillCooldown *= multiplier;
        }
    }
}
