using RCCom.Runtime;
using UnityEngine;

namespace RCCom.Effects.Card.Concrete
{
    /// <summary>카드1 "강화된 화력": 플레이어 공격력 +15%.</summary>
    [CreateAssetMenu(menuName = "RCCom/Card/Player Attack Damage Boost Card")]
    public class PlayerAttackDamageBoostCard : CardEffectBase
    {
        [SerializeField] private float multiplier = 1.15f;

        private void Reset()
        {
            displayName = "강화된 화력";
            description = "플레이어 공격력 +15%";
        }

        public override void Apply(CardContext ctx)
        {
            ctx.player.data.attackDamage *= multiplier;
        }
    }
}
