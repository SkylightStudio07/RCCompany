namespace RCCom.Effects.Tower
{
    /// <summary>
    /// 스킬/파워 타워가 공격 타워에게 주는 버프의 조회 계약. 스탯을 직접 가감하지 않고,
    /// 데미지/공격주기 계산 시점에 파이프라인으로 조회해 사거리 이탈 시 자동으로 사라지게 한다.
    /// </summary>
    public interface ITowerAura
    {
        float ModifyOutgoingDamage(float baseDamage);
        float ModifyAttackInterval(float baseInterval);
    }
}
