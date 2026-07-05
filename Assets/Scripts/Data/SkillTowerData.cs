using System;

namespace RCCom.Data
{
    /// <summary>
    /// 스킬 타워: 자기 사거리 내 공격 타워에게 상시 버프 부여 (범위 이탈 시 상실).
    /// </summary>
    [Serializable]
    public class SkillTowerData : TowerData
    {
        public float buffRange;
        public float damageBuffMultiplier;
        public float attackSpeedBuffMultiplier;

        public override float DisplayRange => buffRange;

        public SkillTowerData()
        {
            kind = TowerKind.Skill;
        }
    }
}
