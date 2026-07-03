using System.Collections.Generic;
using RCCom.Definitions.Enemy;
using RCCom.Runtime;
using UnityEngine;

namespace RCCom.Managers
{
    /// <summary>
    /// 게임 흐름 단계 매니저 중 하나 (ARCHITECTURE.md 5단계 원칙) — 적 스폰과
    /// List&lt;EnemyInstance&gt; 순회(Tick)를 담당한다. 별도 EnemyManager 없이 이 매니저가
    /// 직접 리스트를 들고 자기 Update()에서 Tick()만 호출 — 각 EnemyInstance가 스스로
    /// 이동/효과를 처리한다.
    ///
    /// Day1 범위: 고정 간격 순차 스폰 베타버전 (예산 기반 절차적 생성은 Day2에 이 매니저를 확장).
    /// </summary>
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private MapManager mapManager;
        [SerializeField] private BaseController baseController;
        [SerializeField] private EnemyDefinition[] spawnPool;
        [SerializeField] private EnemyView viewPrefab;
        [SerializeField] private float spawnInterval = 2f;

        [Header("난수 (독립성·재현성 보장 — UnityEngine.Random 대신 전용 System.Random 사용)")]
        [SerializeField] private int randomSeed = 12345;

        private readonly List<EnemyInstance> _aliveEnemies = new();
        private System.Random _rng;
        private float _spawnCooldown;

        /// <summary>Day1 임시 보상 누계. GameManager 도입 시 그쪽으로 이관할 것.</summary>
        private int _totalGold;
        private int _totalExp;

        private void Awake()
        {
            _rng = new System.Random(randomSeed);
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            TickSpawning(deltaTime);
            TickEnemies(deltaTime);
        }

        private void TickSpawning(float deltaTime)
        {
            _spawnCooldown -= deltaTime;
            if (_spawnCooldown > 0f || spawnPool.Length == 0)
            {
                return;
            }

            EnemyDefinition definition = spawnPool[_rng.Next(spawnPool.Length)];
            SpawnOne(definition);
            _spawnCooldown = spawnInterval;
        }

        private void SpawnOne(EnemyDefinition definition)
        {
            IReadOnlyList<Vector2> path = mapManager.Waypoints;
            if (path.Count == 0)
            {
                return;
            }

            var instance = new EnemyInstance
            {
                definition = definition,
                position = path[0],
            };
            instance.Spawn(path, baseController);

            instance.Died += () =>
            {
                _aliveEnemies.Remove(instance);
                GrantReward(instance.Data.goldReward, instance.Data.expReward);
            };
            instance.ReachedGoal += () => _aliveEnemies.Remove(instance);

            EnemyView view = Instantiate(viewPrefab, path[0], Quaternion.identity);
            view.Bind(instance);

            _aliveEnemies.Add(instance);
        }

        private void TickEnemies(float deltaTime)
        {
            // Tick 도중 Died/ReachedGoal로 리스트에서 빠질 수 있어 역순으로 순회한다.
            for (int i = _aliveEnemies.Count - 1; i >= 0; i--)
            {
                _aliveEnemies[i].Tick(deltaTime);
            }
        }

        /// <summary>
        /// Day1 임시 처리 — GameManager가 생기기 전까지 여기서 누계만 로그로 남긴다.
        /// GDD Day1 체크리스트 "수치는 가짜값 OK" 허용 범위.
        /// </summary>
        private void GrantReward(int gold, int exp)
        {
            _totalGold += gold;
            _totalExp += exp;
            Debug.Log($"[Wave] 처치 보상 +{gold}G/+{exp}EXP (누계 {_totalGold}G/{_totalExp}EXP)");
        }
    }
}
