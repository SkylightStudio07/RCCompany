using RCCom.Runtime;

namespace RCCom.Effects.Card
{
    /// <summary>
    /// 업그레이드 카드 효과 계약. GDD 기술 노트: Project Vertex의 CardEffect는 "선택되는
    /// 순간 1회 실행되고 끝"이라 Tower/Enemy처럼 여러 훅(OnTick 등)이 필요 없다 — 그 패턴을
    /// 그대로 재사용해 단일 진입점(Apply)만 둔다.
    /// </summary>
    public interface ICardEffect
    {
        void Apply(CardContext ctx);
    }
}
