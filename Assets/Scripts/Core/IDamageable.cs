namespace RCCom.Core
{
    /// <summary>
    /// 피해를 받을 수 있는 대상의 최소 계약. 적(EnemyInstance)이 구현하며, 플레이어/거점도
    /// 컨트롤러 단계에서 구현 예정.
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(float amount);
    }
}
