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

        /// <summary>
        /// 사거리 표시 UI(RangeIndicator)가 참조하는 값. 파워 타워처럼 사거리 개념이 없는
        /// 종류는 기본값 0을 그대로 써서 "표시할 사거리 없음"으로 처리된다. 기존
        /// TowerInstance.RefreshRangeCollider()의 콜라이더 반영 로직과는 별개 — 그쪽은
        /// -1 sentinel로 "건드리지 않음"을 구분해야 해서 그대로 두고, 이건 순수 표시 전용.
        /// </summary>
        public virtual float DisplayRange => 0f;
    }
}
