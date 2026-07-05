using RCCom.Data;
using RCCom.Definitions.Tower;
using RCCom.Runtime;
using UnityEngine;

namespace RCCom.Effects.Card.Concrete
{
    /// <summary>
    /// 카드5 "사거리 확장": 공격 타워 사거리 +15%. 이미 지어진 타워의 콜라이더 반경도 즉시
    /// 갱신해야 실제로 사거리가 늘어난 게 반영된다 (TowerInstance.RefreshRangeCollider 참고).
    /// </summary>
    [CreateAssetMenu(menuName = "RCCom/Card/Attack Tower Range Boost Card")]
    public class AttackTowerRangeBoostCard : CardEffectBase
    {
        [SerializeField] private float multiplier = 1.15f;

        private void Reset()
        {
            displayName = "사거리 확장";
            description = "공격 타워 사거리 +15%";
        }

        public override void Apply(CardContext ctx)
        {
            foreach (TowerDefinition definition in ctx.towerRoster.towers)
            {
                if (definition.Data is AttackTowerData data)
                {
                    data.attackRange *= multiplier;
                }
            }

            foreach (TowerInstance instance in TowerInstance.All)
            {
                instance.RefreshRangeCollider();
            }
        }
    }
}
