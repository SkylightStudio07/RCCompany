using System;

namespace RCCom.Data
{
    /// <summary>
    /// 공격 타워: 사거리 내 가장 가까운 적을 직접 공격.
    /// </summary>
    [Serializable]
    public class AttackTowerData : TowerData
    {
        public float damage;
        public float attackRange;
        public float attackInterval;

        public AttackTowerData()
        {
            kind = TowerKind.Attack;
        }
    }
}
