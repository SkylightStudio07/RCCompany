using System;
using RCCom.Core;
using UnityEngine;

namespace RCCom.Runtime
{
    /// <summary>
    /// 거점(수비 대상). Player와 마찬가지로 개체가 하나뿐이라 그냥 MonoBehaviour로 둔다
    /// (매니저 아키텍처 원칙상 별도 BaseManager도 만들지 않음).
    ///
    /// 적이 웨이포인트 경로 끝에 도달하면 EnemyInstance가 이 컴포넌트의 TakeDamage를 직접
    /// 호출한다 (물리 충돌이 아니라 "경로 완주" 판정 — WaveManager가 스폰 시 이 컴포넌트를
    /// 목표로 넘겨줌). 플레이어처럼 물리 접촉으로도 맞을 수 있게 하려면 Collider2D를 추가로
    /// 붙여도 되지만(EnemyView가 IDamageable을 범용으로 감지하므로 자동으로 동작함), 필수는 아님.
    /// </summary>
    public class BaseController : MonoBehaviour, IDamageable
    {
        public float maxHealth;

        public float CurrentHealth { get; private set; }
        public event Action Defeated;

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void TakeDamage(float amount)
        {
            if (CurrentHealth <= 0f)
            {
                return;
            }

            CurrentHealth -= amount;

            if (CurrentHealth <= 0f)
            {
                CurrentHealth = 0f;
                Defeated?.Invoke();
            }
        }
    }
}
