namespace RCCom.Data
{
    /// <summary>
    /// 상태이상 종류. 이전 프로젝트(Project Vertex)의 개념을 이름만 계승하되,
    /// 원본 소스가 없어 이 프로젝트(타워 디펜스) 문맥에 맞게 의미를 새로 정의함 (2026-07-04).
    /// </summary>
    public enum StatusType
    {
        /// <summary>대상이 주는 피해 감소.</summary>
        Weak,

        /// <summary>대상이 받는 피해 증가.</summary>
        Vulnerable,

        /// <summary>매 틱 고정 피해 (DoT).</summary>
        Poison,

        /// <summary>이동속도 감소. 카드14 빙결 오라 타워가 사용.</summary>
        Slow,

        /// <summary>매 틱 고정 회복. 카드15 재생의 축이 거점에 부여.</summary>
        Regen,

        /// <summary>공격력 고정치 증가.</summary>
        Strength,

        /// <summary>예약값. 이 게임엔 블록/방어 스탯이 없어 현재 대응 효과 없음 — 필요 시 의미 재정의.</summary>
        Dexterity,
    }
}
