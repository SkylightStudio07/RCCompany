using RCCom.Definitions.Enemy;
using RCCom.Runtime;
using UnityEngine;

namespace RCCom.Debugging
{
    /// <summary>
    /// 임시 디버그용 — 실제 웨이브 진행은 이제 WaveManager가 담당하니 그쪽으로 테스트하는 게
    /// 낫지만, 맵/거점 세팅 없이 적 1체만 놓고 타워 효과 등을 격리 테스트하고 싶을 때 쓴다.
    /// 경로(path)를 null로 넘겨 제자리에 가만히 서 있게 만든다 — 확인 끝나면 지워도 된다.
    /// </summary>
    public class EnemySpawnerDebug : MonoBehaviour
    {
        public EnemyDefinition definition;
        public EnemyView viewPrefab;
        public Vector2 spawnPosition;

        private EnemyInstance _instance;

        private void Start()
        {
            _instance = new EnemyInstance
            {
                definition = definition,
                position = spawnPosition,
            };
            _instance.Spawn(path: null, goal: null);

            EnemyView view = Instantiate(viewPrefab, spawnPosition, Quaternion.identity);
            view.Bind(_instance);
        }

        private void Update()
        {
            _instance?.Tick(Time.deltaTime);
        }
    }
}
