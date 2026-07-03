using System;

namespace RCCom.Data
{
    /// <summary>
    /// 모든 타워 종류가 공유하는 공통 데이터. MonoBehaviour/ScriptableObject가 아닌 순수 데이터 컨테이너.
    /// 타입별 세부 스탯은 하위 클래스(AttackTowerData 등, 각각 별도 파일)에 정의한다.
    /// </summary>
    [Serializable]
    public abstract class TowerData
    {
        public string towerId;
        public string displayName;
        public TowerKind kind;
        public int buildCost;
    }
}
