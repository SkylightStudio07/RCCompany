using System.Collections.Generic;
using UnityEngine;

namespace RCCom.Runtime
{
    /// <summary>
    /// 공격 명중 시 잠깐 나타났다 사라지는 레이저 느낌의 라인 시각 효과. 로직 없는 순수
    /// 렌더러 프리팹(4계층 중 3번) — 데미지 계산과 완전히 분리돼 있어서, 이 이펙트가
    /// 늦게 나가거나 아예 프리팹이 비어있어도 게임 판정에는 영향이 없다.
    ///
    /// 웨이브가 진행될수록 타워 수·공격속도가 계속 늘어나(카드 강화 포함) 초당 발생량이
    /// MotionTrailGhost 못지않게 커질 수 있어, 같은 이유로 풀링 적용 — Destroy 대신 prefab별
    /// 정적 대기열(_availablePool)에 반납해 재사용한다. 여러 종류의 공격 이펙트(DamageEffect/
    /// PierceDamageEffect/PoisonDamageEffect/SplashDamageEffect/PlayerController)가 서로 다른
    /// prefab을 물려뒀을 수도 있어 prefab별로 풀을 분리해서 관리한다. 정적 API(Spawn)는 그대로라
    /// 호출부(각 효과 클래스) 코드는 전혀 손댈 필요가 없다.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class AttackFlash : MonoBehaviour
    {
        [SerializeField] private float lifetime = 0.08f;

        private static readonly Dictionary<GameObject, Queue<AttackFlash>> _availablePool = new();

        private LineRenderer _line;
        private GameObject _sourcePrefab;
        private float _remainingLifetime;
        private bool _isActive;

        private void Awake()
        {
            _line = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            if (!_isActive)
            {
                return;
            }

            _remainingLifetime -= Time.deltaTime;
            if (_remainingLifetime <= 0f)
            {
                Deactivate();
            }
        }

        private void Play(Vector3 from, Vector3 to)
        {
            _line.positionCount = 2;
            _line.SetPosition(0, from);
            _line.SetPosition(1, to);
            _line.enabled = true;

            _remainingLifetime = lifetime;
            _isActive = true;
        }

        private void Deactivate()
        {
            _isActive = false;
            _line.enabled = false;

            if (_sourcePrefab == null)
            {
                return;
            }

            if (!_availablePool.TryGetValue(_sourcePrefab, out Queue<AttackFlash> queue))
            {
                queue = new Queue<AttackFlash>();
                _availablePool[_sourcePrefab] = queue;
            }

            queue.Enqueue(this);
        }

        /// <summary>prefab을 아직 안 만들었으면 조용히 무시 — 시각 효과 미배치가 공격 로직을 막으면 안 된다.</summary>
        public static void Spawn(GameObject prefab, Vector3 from, Vector3 to)
        {
            if (prefab == null)
            {
                return;
            }

            GetOrCreate(prefab).Play(from, to);
        }

        private static AttackFlash GetOrCreate(GameObject prefab)
        {
            if (_availablePool.TryGetValue(prefab, out Queue<AttackFlash> queue) && queue.Count > 0)
            {
                return queue.Dequeue();
            }

            GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            AttackFlash flash = instance.GetComponent<AttackFlash>();
            flash._sourcePrefab = prefab;
            return flash;
        }

        /// <summary>
        /// 재시작(Retry, SceneManager.LoadScene) 시 GameManager가 호출 — 씬 재로드로 풀 안의
        /// 인스턴스들은 파괴되는데, 이 정적 대기열은 그대로 남아 죽은 참조를 들고 있게 되므로
        /// (다음 Spawn에서 Dequeue하면 MissingReferenceException) 명시적으로 비워야 한다.
        /// </summary>
        public static void ClearPool()
        {
            _availablePool.Clear();
        }
    }
}
