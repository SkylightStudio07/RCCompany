using RCCom.Core;
using RCCom.Runtime;
using UnityEngine;

namespace RCCom.Effects.Enemy
{
    /// <summary>
    /// IEnemyEffect의 편의용 기본 구현. 실제 효과는 이 클래스를 상속해 필요한 훅만 오버라이드한다.
    /// </summary>
    public abstract class EnemyEffectBase : ScriptableObject, IEnemyEffect
    {
        public virtual void OnSpawn(EnemyContext ctx) { }
        public virtual void OnTick(EnemyContext ctx) { }
        public virtual void OnDealContactDamage(EnemyContext ctx, IDamageable target) { }
        public virtual void OnDeath(EnemyContext ctx) { }
    }
}
