using System.Collections.Generic;
using RCCom.Runtime;
using UnityEngine;

namespace RCCom.Core
{
    /// <summary>
    /// "후보 목록에서 사거리 내 가장 가까운 적 찾기"는 공격 타워(DamageEffect)와 플레이어
    /// 기본 공격이 동일하게 필요로 해서 한 곳에 모아 공유한다.
    /// </summary>
    public static class EnemyTargeting
    {
        public static EnemyInstance FindNearestInRange(IReadOnlyList<EnemyInstance> candidates, Vector2 origin, float range)
        {
            EnemyInstance nearest = null;
            float nearestSqrDistance = range * range;

            foreach (EnemyInstance enemy in candidates)
            {
                float sqrDistance = (enemy.position - origin).sqrMagnitude;
                if (sqrDistance <= nearestSqrDistance)
                {
                    nearest = enemy;
                    nearestSqrDistance = sqrDistance;
                }
            }

            return nearest;
        }
    }
}
