using UnityEngine;
using UnityEngine.EventSystems;

namespace RCCom.UI
{
    /// <summary>
    /// 버튼 등에 붙여서 마우스 호버 시 부드럽게 살짝 커지게 하는 범용 컴포넌트. Unity Button의
    /// 기본 Transition(Color Tint/Sprite Swap/Animation)엔 스케일 전환이 없어서 직접 구현.
    ///
    /// Time.unscaledDeltaTime을 쓴다 — 게임오버 화면처럼 Time.timeScale=0(일시정지)인 상태에서
    /// 뜨는 버튼에도 붙을 수 있는데, 일반 deltaTime을 쓰면 그동안 애니메이션이 아예 안 움직인다.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class UIHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float hoverScale = 1.1f;
        [SerializeField] private float transitionSpeed = 12f;

        private Vector3 _baseScale;
        private Vector3 _targetScale;

        private void Awake()
        {
            _baseScale = transform.localScale;
            _targetScale = _baseScale;
        }

        private void Update()
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, Time.unscaledDeltaTime * transitionSpeed);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _targetScale = _baseScale * hoverScale;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _targetScale = _baseScale;
        }
    }
}
