using RCCom.Runtime;
using UnityEngine;

namespace RCCom.Effects.Card.Concrete
{
    /// <summary>카드12 "거점 보강": 거점 최대체력 +20%.</summary>
    [CreateAssetMenu(menuName = "RCCom/Card/Base Max Health Boost Card")]
    public class BaseMaxHealthBoostCard : CardEffectBase
    {
        [SerializeField] private float multiplier = 1.20f;

        private void Reset()
        {
            displayName = "거점 보강";
            description = "거점 최대체력 +20%";
        }

        public override void Apply(CardContext ctx)
        {
            ctx.baseController.IncreaseMaxHealth(multiplier);
        }
    }
}
