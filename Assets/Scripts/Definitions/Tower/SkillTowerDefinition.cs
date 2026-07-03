using RCCom.Data;
using UnityEngine;

namespace RCCom.Definitions.Tower
{
    [CreateAssetMenu(menuName = "RCCom/Tower/Skill Tower Definition")]
    public class SkillTowerDefinition : TowerDefinition
    {
        public SkillTowerData data = new();
        public override TowerData Data => data;
    }
}
