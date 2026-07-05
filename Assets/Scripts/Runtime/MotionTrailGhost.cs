using UnityEngine;

namespace RCCom.Runtime
{
    /// <summary>
    /// 이동 잔상 트레일의 "고스트" 1개 — 스폰 시점의 스프라이트를 그대로 복사해 반투명하게
    /// 표시하다가 fadeDuration에 걸쳐 알파를 0으로 줄이며 사라진다. 로직 없는 순수 렌더러
    /// 프리팹(4계층 3번).
    ///
    /// MotionTrailSpawner가 이동 중 초당 수십 개를 만들어내는 고빈도 지속 패턴이라(공격
    /// 이펙트처럼 쿨다운 걸린 저빈도 이벤트와 다름) 자기 자신을 Destroy하지 않고, 다 사라진
    /// 뒤엔 알파 0 상태로 가만히 대기한다 — MotionTrailSpawner가 고정 크기 풀로 재사용
    /// (Play를 다시 호출)하며, Instantiate/Destroy 자체가 반복되지 않아 GC 부담이 없다.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class MotionTrailGhost : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private Color _startColor;
        private float _fadeDuration;
        private float _elapsed;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Play(Sprite sprite, Vector3 position, Quaternion rotation, Color color, float fadeDuration)
        {
            _spriteRenderer.sprite = sprite;
            transform.SetPositionAndRotation(position, rotation);

            _startColor = color;
            _spriteRenderer.color = color;
            _fadeDuration = fadeDuration;
            _elapsed = 0f;
        }

        private void Update()
        {
            if (_elapsed >= _fadeDuration)
            {
                return;
            }

            _elapsed += Time.deltaTime;
            float t = _fadeDuration > 0f ? Mathf.Clamp01(_elapsed / _fadeDuration) : 1f;

            Color color = _startColor;
            color.a = _startColor.a * (1f - t);
            _spriteRenderer.color = color;
        }
    }
}
