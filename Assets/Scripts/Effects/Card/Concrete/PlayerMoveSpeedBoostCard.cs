using RCCom.Runtime;
using UnityEngine;

namespace RCCom.Effects.Card.Concrete
{
    /// <summary>카드2 "신속한 발걸음": 플레이어 이동속도 +10%.</summary>
    [CreateAssetMenu(menuName = "RCCom/Card/Player Move Speed Boost Card")]
    public class PlayerMoveSpeedBoostCard : CardEffectBase
    {
        [SerializeField] private float multiplier = 1.10f;

        private void Reset()
        {
            displayName = "신속한 발걸음";
            description = "플레이어 이동속도 +10%";
        }

        public override void Apply(CardContext ctx)
        {
            ctx.player.data.moveSpeed *= multiplier;
        }
    }
}
