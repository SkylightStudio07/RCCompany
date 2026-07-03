using RCCom.Core;
using RCCom.Runtime;

namespace RCCom.Effects.Enemy
{
    /// <summary>
    /// 적 특수능력 계약. Tower의 ITowerEffect/TowerEffectBase와 동일한 패턴(계약+편의 기본구현).
    /// 훅 시점은 Tower와 이름은 다르지만 성격이 대응된다:
    /// OnSpawn(≈OnBuild, 스폰 시 1회) / OnTick(반복) /
    /// OnDealContactDamage(≈아군 사거리 출입에 대응하는 "적 고유 상호작용 시점" — 접촉 피해를 주는 순간) /
    /// OnDeath(사망 시 1회, 정리·보상 트리거용으로 추가).
    /// </summary>
    public interface IEnemyEffect
    {
        void OnSpawn(EnemyContext ctx);
        void OnTick(EnemyContext ctx);
        void OnDealContactDamage(EnemyContext ctx, IDamageable target);
        void OnDeath(EnemyContext ctx);
    }
}
