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

        public event Action<float> Damaged;
        public event Action Died;

        /// <summary>
        /// 경로 끝(거점)에 도달해서 제거되는 경우. Died(처치)와 구분한다 — 처치 보상은
        /// Died에서만 주고, 여기서는 주지 않는다.
        /// </summary>
        public event Action ReachedGoal;

        public EnemyData Data => definition.data;

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
            MoveAlongPath(deltaTime);

            EnemyContext ctx = MakeContext(deltaTime);
            foreach (IEnemyEffect effect in definition.effects)
            {
                effect.OnTick(ctx);
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
            float step = Data.moveSpeed * deltaTime;

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
