using RCCom.Data;
using UnityEngine;

namespace RCCom.Definitions.Tower
{
    [CreateAssetMenu(menuName = "RCCom/Tower/Power Tower Definition")]
    public class PowerTowerDefinition : TowerDefinition
    {
        public PowerTowerData data = new();
        public override TowerData Data => data;
    }
}
