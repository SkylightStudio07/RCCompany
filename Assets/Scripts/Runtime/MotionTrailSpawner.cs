using UnityEngine;

namespace RCCom.Runtime
{
    /// <summary>
    /// 이동 중일 때만 잔상(MotionTrailGhost)을 주기적으로 남기는 트레일 효과. 순수 시각 효과라
    /// 이동 로직과 완전히 분리 — 자기 transform의 위치 변화량만 관찰하므로, 플레이어든 나중에
    /// 다른 오브젝트든 스프라이트를 가진 오브젝트에 붙이기만 하면 그대로 동작한다.
    /// PlayerController가 스프라이트를 루트가 아니라 자식(spriteTransform)에 둘 수도 있으므로,
    /// 이 컴포넌트는 실제 SpriteRenderer가 있는 오브젝트에 직접 붙일 것 (루트든 자식이든 무관).
    ///
    /// 고정 크기 풀 사용: 공격 이펙트(AttackFlash)처럼 쿨다운 걸린 저빈도 이벤트와 달리, 이동
    /// 중엔 초당 최대 1/spawnInterval개(기본 20개)를 지속적으로 만들어내는 고빈도 패턴이라
    /// Instantiate/Destroy를 반복하면 GC 부담이 누적된다. 그래서 Awake에서 poolSize개를 한 번만
    /// 만들어두고, 이후엔 그 풀을 순환하며 Play()로 재사용한다(파괴 없음).
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class MotionTrailSpawner : MonoBehaviour
    {
        [SerializeField] private MotionTrailGhost ghostPrefab;
        [SerializeField] private float spawnInterval = 0.05f;
        [SerializeField] private float fadeDuration = 0.3f;
        [SerializeField] private Color ghostColor = new(1f, 1f, 1f, 0.5f);
        [SerializeField] private float minMoveDistancePerSpawn = 0.05f;

        [Tooltip("동시에 화면에 떠 있을 수 있는 고스트 최대 개수 (대략 fadeDuration / spawnInterval에 여유를 더한 값이면 충분)")]
        [SerializeField] private int poolSize = 12;

        private SpriteRenderer _spriteRenderer;
        private Vector3 _lastSpawnPosition;
        private float _spawnCooldownRemaining;

        private MotionTrailGhost[] _pool;
        private int _nextPoolIndex;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _lastSpawnPosition = transform.position;

            if (ghostPrefab != null)
            {
                _pool = new MotionTrailGhost[poolSize];
                for (int i = 0; i < poolSize; i++)
                {
                    _pool[i] = Instantiate(ghostPrefab);
                }
            }
        }

        private void Update()
        {
            _spawnCooldownRemaining -= Time.deltaTime;

            if (_pool == null || _spawnCooldownRemaining > 0f)
            {
                return;
            }

            float movedDistance = Vector3.Distance(transform.position, _lastSpawnPosition);
            if (movedDistance < minMoveDistancePerSpawn)
            {
                return;
            }

            MotionTrailGhost ghost = _pool[_nextPoolIndex];
            _nextPoolIndex = (_nextPoolIndex + 1) % _pool.Length;

            ghost.Play(_spriteRenderer.sprite, transform.position, transform.rotation, ghostColor, fadeDuration);

            _lastSpawnPosition = transform.position;
            _spawnCooldownRemaining = spawnInterval;
        }
    }
}
