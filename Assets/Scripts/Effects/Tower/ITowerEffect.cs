using RCCom.Runtime;

namespace RCCom.Effects.Tower
{
    /// <summary>
    /// 타워 특수능력 계약. 다른 시스템(매니저 등)은 TowerEffectBase가 아니라 이 인터페이스에만 의존해
    /// 독립성을 유지한다. 타워 종류별로 실제 쓰이는 시점이 다르다:
    /// 공격 타워=OnTick, 스킬 타워=OnAllyEnterRange/OnAllyExitRange, 파워 타워=OnBuild.
    /// </summary>
    public interface ITowerEffect
    {
        void OnBuild(TowerContext ctx);
        void OnTick(TowerContext ctx);
        void OnAllyEnterRange(TowerContext ctx, TowerInstance ally);
        void OnAllyExitRange(TowerContext ctx, TowerInstance ally);
    }
}
