using RCCom.Runtime;
using UnityEngine;

namespace RCCom.Effects.Tower
{
    /// <summary>
    /// ITowerEffect의 편의용 기본 구현. 실제 효과는 이 클래스를 상속해 필요한 훅만 오버라이드한다.
    /// 예: 공격 효과는 OnTick만, 오라 버프 효과는 OnAllyEnterRange/OnAllyExitRange만 구현.
    /// </summary>
    public abstract class TowerEffectBase : ScriptableObject, ITowerEffect
    {
        public virtual void OnBuild(TowerContext ctx) { }
        public virtual void OnTick(TowerContext ctx) { }
        public virtual void OnAllyEnterRange(TowerContext ctx, TowerInstance ally) { }
        public virtual void OnAllyExitRange(TowerContext ctx, TowerInstance ally) { }
    }
}
