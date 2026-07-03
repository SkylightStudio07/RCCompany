using RCCom.Data;
using UnityEngine;

namespace RCCom.Definitions.Tower
{
    [CreateAssetMenu(menuName = "RCCom/Tower/Attack Tower Definition")]
    public class AttackTowerDefinition : TowerDefinition
    {
        public AttackTowerData data = new();
        public override TowerData Data => data;
    }
}
