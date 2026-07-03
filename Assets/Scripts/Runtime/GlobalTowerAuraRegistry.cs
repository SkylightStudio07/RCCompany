using System.Collections.Generic;
using RCCom.Effects.Tower;

namespace RCCom.Runtime
{
    /// <summary>
    /// 파워 타워처럼 사거리 개념 없이 전역으로 적용되는 버프를 모아두는 최소 레지스트리.
    /// 매니저 아키텍처 원칙(ARCHITECTURE.md 5단계)상 TowerManager는 만들지 않기로 했으므로,
    /// GameManager(아직 없음)의 인스턴스 필드로 옮길 것. 그 전까지 임시로 static 컨테이너로 둔다.
    /// 매니저 도입 시 씬/플레이 세션 시작 시 Clear()를 호출하도록 바꿀 것 (에디터 도메인
    /// 리로드로 static 값이 남아있을 수 있음).
    /// </summary>
    public static class GlobalTowerAuraRegistry
    {
        public static readonly List<ITowerAura> Auras = new();
    }
}
