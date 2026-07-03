using System;

namespace RCCom.Data
{
    /// <summary>
    /// 파워 타워: 건설 즉시 전역(글로벌) 버프 적용. 사거리 개념 없음.
    /// </summary>
    [Serializable]
    public class PowerTowerData : TowerData
    {
        public float globalDamageBonus;
        public float upgradeIncrement;

        public PowerTowerData()
        {
            kind = TowerKind.Power;
        }
    }
}
