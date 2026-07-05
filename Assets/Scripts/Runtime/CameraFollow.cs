using UnityEngine;

namespace RCCom.Runtime
{
    /// <summary>
    /// 플레이어를 부드럽게 추적하되, bounds로 지정한 영역(배경 크기와 맞춘 BoxCollider2D) 밖으로는
    /// 카메라 시야가 벗어나지 않도록 클램프한다. 매니저가 필요 없는 카메라 전용 단일 컴포넌트라
    /// Player/BaseController처럼 그냥 MonoBehaviour 하나로 처리.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float smoothTime = 0.15f;

        [Tooltip("카메라가 벗어나면 안 되는 영역. 배경 크기와 맞춘 BoxCollider2D를 드래그 (Is Trigger 체크해서 물리 충돌은 안 나게 할 것)")]
        [SerializeField] private Collider2D bounds;

        private Camera _camera;
        private Vector3 _velocity;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            Vector3 desired = new(target.position.x, target.position.y, transform.position.z);

            if (bounds != null)
            {
                float halfHeight = _camera.orthographicSize;
                float halfWidth = halfHeight * _camera.aspect;

                Bounds b = bounds.bounds;
                float minX = b.min.x + halfWidth;
                float maxX = b.max.x - halfWidth;
                float minY = b.min.y + halfHeight;
                float maxY = b.max.y - halfHeight;

                // 배경이 카메라 시야보다 작은 축(예: 세로로 좁은 배경)은 클램프 대신 중앙 고정.
                desired.x = minX <= maxX ? Mathf.Clamp(desired.x, minX, maxX) : b.center.x;
                desired.y = minY <= maxY ? Mathf.Clamp(desired.y, minY, maxY) : b.center.y;
            }

            transform.position = Vector3.SmoothDamp(transform.position, desired, ref _velocity, smoothTime);
        }
    }
}
