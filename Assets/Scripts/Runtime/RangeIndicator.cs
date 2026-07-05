using UnityEngine;

namespace RCCom.Runtime
{
    /// <summary>
    /// 타워 사거리를 빨간 테두리 원(내부 투명)으로 표시하는 공용 인디케이터. LineRenderer로
    /// 원을 그려서 별도 링 모양 아트 없이도 어떤 반경이든 항상 일정한 선 두께로 대응된다.
    /// 씬에 하나만 두고 위치/반경만 바꿔가며 재사용 — TowerBuildPreview(건설 중 미리보기)와
    /// TowerBuildController(이미 지어진 타워 클릭 시 조회) 둘 다 같은 인스턴스를 참조해도 된다
    /// (동시에 두 사거리를 보여줄 필요는 없어서 하나만으로 충분).
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class RangeIndicator : MonoBehaviour
    {
        [SerializeField] private int segmentCount = 48;
        [SerializeField] private Color color = Color.red;

        private LineRenderer _line;

        private void Awake()
        {
            _line = GetComponent<LineRenderer>();
            _line.loop = true;
            _line.useWorldSpace = false;
            _line.startColor = color;
            _line.endColor = color;
            Hide();
        }

        public void Show(Vector3 center, float radius)
        {
            if (radius <= 0f)
            {
                Hide();
                return;
            }

            transform.position = center;
            _line.positionCount = segmentCount;

            for (int i = 0; i < segmentCount; i++)
            {
                float angle = i * Mathf.PI * 2f / segmentCount;
                _line.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f));
            }

            _line.enabled = true;
        }

        public void Hide()
        {
            _line.enabled = false;
        }
    }
}
