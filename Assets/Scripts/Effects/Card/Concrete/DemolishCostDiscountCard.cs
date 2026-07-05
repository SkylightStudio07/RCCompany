using RCCom.Runtime;
using UnityEngine;

namespace RCCom.Effects.Card.Concrete
{
    /// <summary>타워 철거 비용(건설비 대비 배율)을 낮춰주는 카드.</summary>
    [CreateAssetMenu(menuName = "RCCom/Card/Demolish Cost Discount Card")]
    public class DemolishCostDiscountCard : CardEffectBase
    {
        [SerializeField] private float discountAmount = 0.25f;

        private void Reset()
        {
            displayName = "철거 비용 절감";
            description = "타워 철거 비용 25%p 할인";
        }

        public override void Apply(CardContext ctx)
        {
            ctx.gameManager.ReduceDemolishCostRatio(discountAmount);
        }
    }
}
