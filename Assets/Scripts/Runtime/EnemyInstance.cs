using System;
using System.Collections.Generic;
using RCCom.Core;
using RCCom.Data;
using RCCom.Definitions.Enemy;
using RCCom.Effects.Enemy;
using UnityEngine;

namespace RCCom.Runtime
{
    /// <summary>
    /// 스폰된 적 1체의 런타임 상태. 매니저 아키텍처 원칙(ARCHITECTURE.md 5단계): 실시간 웨이브라
    /// 동시 개체 수가 많아질 수 있어 MonoBehaviour.Update() 오버헤드를 피하려고 순수 C# 클래스로
    /// 둔다 (MonoBehaviour 아님). 스스로 상태를 들고 Damaged/Died/ReachedGoal 이벤트로 외부
    /// (EnemyView, WaveManager)에 알린다 — 매니저는 이 인스턴스 리스트를 순회하며 Tick()만
    /// 불러주면 된다.
    /// </summary>
    public class EnemyInstance : IDamageable
    {
        public EnemyDefinition definition;
        public Vector2 position;
        public float currentHealth;

        private IReadOnlyList<Vector2> _path;
        private IDamageable _goal;
        private int _pathIndex;
        private bool _isDead;

        /// <summary>
        /// 빙결 오라 타워(SlowAuraEffect) 등이 적용하는 이동속도 배율. 지속시간 기반으로
        /// "사거리 안에 있는 동안 계속 갱신"되다가, 갱신이 끊기면(사거리 이탈) 자연히 만료된다
        /// (Tower의 OnAllyEnterRange/OnAllyExitRange 같은 진입/이탈 훅이 적에게는 없어서
        /// 이 방식으로 대체 — 매 프레임 OnTick에서 갱신되는 한 계속 유지됨).
        /// </summary>
        private float _speedMultiplier = 1f;
        private float _speedMultiplierRemaining;

        /// <summary>맹독 타워(PoisonDamageEffect)가 적용하는 지속피해(DoT). Slow와 동일한 갱신 방식.</summary>
        private float _poisonDamagePerSecond;
        private float _poisonRemaining;

        /// <summary>취약 오라(VulnerableAuraEffect)가 적용하는 피해 배율. Slow와 동일한 갱신 방식.</summary>
        private float _vulnerableMultiplier = 1f;
        private float _vulnerableRemaining;

        public event Action<float> Damaged;
        public event Action Died;

        /// <summary>
        /// 경로 끝(거점)에 도달해서 제거되는 경우. Died(처치)와 구분한다 — 처치 보상은
        /// Died에서만 주고, 여기서는 주지 않는다.
        /// </summary>
        public event Action ReachedGoal;

        public EnemyData Data => definition.data;

        /// <summary>
        /// 지금 향하고 있는 다음 웨이포인트 — EnemyView가 실제 이동 여부(프레임 간 위치 변화량)
        /// 대신 "의도된 목적지" 기준으로 방향을 계산할 때 쓴다. 스폰 직후나 웨이포인트에 정확히
        /// 스냅되는 프레임처럼 실이동량이 0에 가까운 순간에도 항상 올바른 방향을 주기 위함.
        /// 경로가 없거나 이미 끝에 도달했으면 null.
        /// </summary>
        public Vector2? CurrentTargetWaypoint =>
            _path != null && _pathIndex < _path.Count ? _path[_pathIndex] : (Vector2?)null;

        /// <summary>
        /// path: 이동할 웨이포인트 목록 (MapManager.Waypoints). goal: 경로 끝에 도달했을 때
        /// 접촉 피해를 받을 대상 (거점). 그리드와 무관한 자유 좌표 이동임에 유의.
        /// </summary>
        public void Spawn(IReadOnlyList<Vector2> path, IDamageable goal)
        {
            currentHealth = Data.maxHealth;
            _path = path;
            _goal = goal;
            _pathIndex = 0;

            EnemyContext ctx = MakeContext(0f);
            foreach (IEnemyEffect effect in definition.effects)
            {
                effect.OnSpawn(ctx);
            }
        }

        public void Tick(float deltaTime)
        {
            TickSpeedMultiplier(deltaTime);
            TickPoison(deltaTime);
            TickVulnerable(deltaTime);
            MoveAlongPath(deltaTime);

            EnemyContext ctx = MakeContext(deltaTime);
            foreach (IEnemyEffect effect in definition.effects)
            {
                effect.OnTick(ctx);
            }
        }

        /// <summary>사거리 내에 있는 동안 SlowAuraEffect가 매 틱 갱신 호출한다.</summary>
        public void ApplySpeedMultiplier(float multiplier, float duration)
        {
            _speedMultiplier = multiplier;
            _speedMultiplierRemaining = duration;
        }

        /// <summary>맹독 타워(PoisonDamageEffect)가 명중 시 호출 — 계속 맞으면 지속시간이 갱신된다.</summary>
        public void ApplyPoison(float damagePerSecond, float duration)
        {
            _poisonDamagePerSecond = damagePerSecond;
            _poisonRemaining = duration;
        }

        /// <summary>취약 오라(VulnerableAuraEffect)가 사거리 내에 있는 동안 매 틱 갱신 호출한다.</summary>
        public void ApplyVulnerable(float damageTakenMultiplier, float duration)
        {
            _vulnerableMultiplier = damageTakenMultiplier;
            _vulnerableRemaining = duration;
        }

        private void TickSpeedMultiplier(float deltaTime)
        {
            if (_speedMultiplierRemaining <= 0f)
            {
                return;
            }

            _speedMultiplierRemaining -= deltaTime;
            if (_speedMultiplierRemaining <= 0f)
            {
                _speedMultiplier = 1f;
            }
        }

        private void TickPoison(float deltaTime)
        {
            if (_poisonRemaining <= 0f)
            {
                return;
            }

            _poisonRemaining -= deltaTime;
            TakeDamage(_poisonDamagePerSecond * deltaTime);
        }

        private void TickVulnerable(float deltaTime)
        {
            if (_vulnerableRemaining <= 0f)
            {
                return;
            }

            _vulnerableRemaining -= deltaTime;
            if (_vulnerableRemaining <= 0f)
            {
                _vulnerableMultiplier = 1f;
            }
        }

        private void MoveAlongPath(float deltaTime)
        {
            if (_path == null || _pathIndex >= _path.Count)
            {
                return;
            }

            Vector2 target = _path[_pathIndex];
            Vector2 toTarget = target - position;
            float step = Data.moveSpeed * _speedMultiplier * deltaTime;

            if (toTarget.sqrMagnitude <= step * step)
            {
                position = target;
                _pathIndex++;

                if (_pathIndex >= _path.Count)
                {
                    DealContactDamageTo(_goal);
                    ReachedGoal?.Invoke();
                }
            }
            else
            {
                position += toTarget.normalized * step;
            }
        }

        public void DealContactDamageTo(IDamageable target)
        {
            EnemyContext ctx = MakeContext(0f);
            foreach (IEnemyEffect effect in definition.effects)
            {
                effect.OnDealContactDamage(ctx, target);
            }
        }

        public void TakeDamage(float amount)
        {
            if (_isDead)
            {
                return;
            }

            amount *= _vulnerableMultiplier;
            currentHealth -= amount;
            Damaged?.Invoke(amount);
            Debug.Log($"[EnemyDebug] {Data.displayName} 피격 -{amount} (남은 체력 {currentHealth}/{Data.maxHealth})"); // TODO: 확인 끝나면 삭제

            if (currentHealth <= 0f)
            {
                _isDead = true;

                EnemyContext ctx = MakeContext(0f);
                foreach (IEnemyEffect effect in definition.effects)
                {
                    effect.OnDeath(ctx);
                }

                Died?.Invoke();
            }
        }

        private EnemyContext MakeContext(float deltaTime) => new()
        {
            self = this,
            deltaTime = deltaTime,
        };
    }
}
