using System.Collections.Generic;
using RCCom.Core;
using RCCom.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RCCom.Runtime
{
    /// <summary>
    /// 플레이어 컨트롤러. 그리드와 무관한 자유 이동, 가장 가까운 적 자동 원거리 공격,
    /// 쿨다운 기반 스킬, 피격/무적 처리를 담당한다. 개체가 하나뿐이라 Tower처럼 그냥
    /// MonoBehaviour로 둔다 (매니저 아키텍처 원칙상 PlayerManager도 만들지 않음).
    ///
    /// 인스펙터 연결 필요:
    /// - data: PlayerData
    /// - moveAction: InputSystem_Actions의 Player/Move
    /// - skillAction: InputSystem_Actions의 Player/Attack을 스킬 트리거로 재사용
    ///   (기본 공격은 자동 발동이라 이 버튼이 필요 없고, GDD가 스킬 조작 방식을 별도로
    ///   정하지 않아 기존 액션을 재사용함 — 세부는 추후 논의 대상)
    /// - attackRangeTrigger: 자식 오브젝트(원거리 사거리용 큰 Collider2D)에 붙은 AttackRangeTrigger
    ///
    /// 이 오브젝트 자신의 Collider2D는 몸통 히트박스(적 접촉 피해 판정용, EnemyView 쪽에서 감지)이고,
    /// attackRangeTrigger는 별도 자식 오브젝트의 더 큰 콜라이더다 — 한 오브젝트에 반경이 다른
    /// 콜라이더 2개를 두면 OnTriggerEnter2D에서 어느 쪽인지 구분이 안 돼 분리했다.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class PlayerController : MonoBehaviour, IDamageable
    {
        public PlayerData data;
        public InputActionReference moveAction;
        public InputActionReference skillAction;
        public AttackRangeTrigger attackRangeTrigger;

        public float CurrentHealth { get; private set; }

        private float _invulnerabilityRemaining;
        private float _attackCooldownRemaining;
        private float _skillCooldownRemaining;

        private readonly List<EnemyInstance> _enemiesInRange = new();

        private void Awake()
        {
            CurrentHealth = data.maxHealth;
        }

        private void OnEnable()
        {
            moveAction.action.Enable();
            skillAction.action.Enable();
            attackRangeTrigger.EnteredRange += HandleEnemyEnteredRange;
            attackRangeTrigger.ExitedRange += HandleEnemyExitedRange;
        }

        private void OnDisable()
        {
            moveAction.action.Disable();
            skillAction.action.Disable();
            attackRangeTrigger.EnteredRange -= HandleEnemyEnteredRange;
            attackRangeTrigger.ExitedRange -= HandleEnemyExitedRange;
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            Move(deltaTime);
            TickTimers(deltaTime);
            TryAutoAttack();
            TrySkill();
        }

        private void Move(float deltaTime)
        {
            Vector2 input = moveAction.action.ReadValue<Vector2>();
            transform.position += (Vector3)(input.normalized * data.moveSpeed * deltaTime);
        }

        private void TickTimers(float deltaTime)
        {
            _invulnerabilityRemaining -= deltaTime;
            _attackCooldownRemaining -= deltaTime;
            _skillCooldownRemaining -= deltaTime;
        }

        private void TryAutoAttack()
        {
            if (_attackCooldownRemaining > 0f)
            {
                return;
            }

            EnemyInstance target = EnemyTargeting.FindNearestInRange(_enemiesInRange, transform.position, data.attackRange);
            if (target == null)
            {
                return;
            }

            target.TakeDamage(data.attackDamage);
            _attackCooldownRemaining = data.attackInterval;
            // 투사체 등 시각 표현은 렌더러 프리팹 단계에서 별도 처리
        }

        private void TrySkill()
        {
            if (_skillCooldownRemaining > 0f || !skillAction.action.WasPerformedThisFrame())
            {
                return;
            }

            float sqrSkillRange = data.skillRange * data.skillRange;
            foreach (EnemyInstance enemy in _enemiesInRange)
            {
                if ((enemy.position - (Vector2)transform.position).sqrMagnitude <= sqrSkillRange)
                {
                    enemy.TakeDamage(data.skillDamage);
                }
            }

            _skillCooldownRemaining = data.skillCooldown;
        }

        private void HandleEnemyEnteredRange(Collider2D other)
        {
            if (other.TryGetComponent(out EnemyView enemyView))
            {
                _enemiesInRange.Add(enemyView.Instance);
            }
        }

        private void HandleEnemyExitedRange(Collider2D other)
        {
            if (other.TryGetComponent(out EnemyView enemyView))
            {
                _enemiesInRange.Remove(enemyView.Instance);
            }
        }

        public void TakeDamage(float amount)
        {
            if (_invulnerabilityRemaining > 0f)
            {
                return;
            }

            CurrentHealth -= amount;
            _invulnerabilityRemaining = data.hitInvulnerabilityDuration;
        }
    }
}
