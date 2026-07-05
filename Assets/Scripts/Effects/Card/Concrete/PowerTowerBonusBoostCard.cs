using RCCom.Data;
using RCCom.Definitions.Tower;
using RCCom.Effects.Tower.Concrete;
using RCCom.Runtime;
using UnityEngine;

namespace RCCom.Effects.Card.Concrete
{
    /// <summary>
    /// 카드8 "축적된 힘": 파워 타워 글로벌 공격력 +0.5 누적. 증가폭은 PowerTowerData.upgradeIncrement
    /// (여러 파워 타워 종류가 있어도 첫 번째로 찾은 값 기준 — 지금은 종류가 하나뿐이라 문제 없음).
    /// </summary>
    [CreateAssetMenu(menuName = "RCCom/Card/Power Tower Bonus Boost Card")]
    public class PowerTowerBonusBoostCard : CardEffectBase
    {
        private void Reset()
        {
            displayName = "축적된 힘";
            description = "파워 타워 글로벌 공격력 +0.5 (누적)";
        }

        public override void Apply(CardContext ctx)
        {
            foreach (TowerDefinition definition in ctx.towerRoster.towers)
            {
                if (definition.Data is PowerTowerData data)
                {
                    GlobalBuffEffect.IncreaseAllGlobalBonuses(ctx.towerRoster, data.upgradeIncrement);
                    return;
                }
            }
        }
    }
}
