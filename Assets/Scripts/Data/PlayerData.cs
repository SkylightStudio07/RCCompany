using System;
using UnityEngine;

namespace RCCom.Data
{
    /// <summary>
    /// 플레이어 스탯 원본 데이터. MonoBehaviour/ScriptableObject가 아닌 순수 데이터 컨테이너.
    /// 플레이어는 거점 체력과 별개로 자신의 체력을 가지며 적의 접촉 피해로 사망할 수 있다 ({{user}} 확인, 2026-07-04).
    /// 공격/스킬 세부 수치는 GDD상 미확정이므로 기본값을 강제하지 않고 인스펙터에서 조정한다.
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        [Header("생존")]
        public float maxHealth;
        public float moveSpeed;

        /// <summary>
        /// GDD "다이" 섹션: 동일 적에게 연속 피격 방지를 위해 피격 후 짧은 무적이 필요.
        /// </summary>
        public float hitInvulnerabilityDuration;

        [Header("기본 공격 (원거리, 가장 가까운 적 자동 타겟팅)")]
        public float attackDamage;
        public float attackRange;
        public float attackInterval;
        public float projectileSpeed;

        [Header("스킬 (세부 효과는 추후 확정)")]
        public float skillCooldown;
        public float skillRange;
        public float skillDamage;
    }
}
