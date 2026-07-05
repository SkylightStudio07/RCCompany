using RCCom.Definitions.Tower;
using RCCom.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace RCCom.Runtime
{
    /// <summary>
    /// 건설 프리뷰: 현재 선택된 타워의 스프라이트를 반투명으로 커서(→가능하면 슬롯 셀 중심)에
    /// 띄우고, 설치 가능 여부에 따라 흰색/빨간색으로 물들인다. Player/CameraFollow와 같은
    /// 이유로 씬에 하나뿐인 개체라 매니저 없이 단일 컴포넌트로 처리.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class TowerBuildPreview : MonoBehaviour
    {
        [SerializeField] private TowerBuildController buildController;
        [SerializeField] private MapManager mapManager;

        [SerializeField] private Color validColor = new(1f, 1f, 1f, 0.5f);
        [SerializeField] private Color invalidColor = new(1f, 0f, 0f, 0.5f);

        [Tooltip("TowerInstance의 Target Visual Size와 동일한 값으로 맞춰야 프리뷰 크기 = 실제 건설 크기가 된다")]
        [SerializeField] private float targetVisualSize = 0.9f;

        [Tooltip("건설 전부터 사거리를 미리 보여줄 공용 인디케이터 (TowerBuildController와 같은 오브젝트를 공유해도 됨)")]
        [SerializeField] private RangeIndicator rangeIndicator;

        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            TowerDefinition definition = buildController.SelectedDefinition;

            // definition == null(건설 모드 자체가 꺼짐)일 땐 rangeIndicator를 아예 건드리지 않는다 —
            // 이 컴포넌트는 매 프레임 도는데, 여기서 Hide()를 매번 호출해버리면 TowerBuildController가
            // 이미 지어진 타워 클릭으로 보여준 사거리 표시를 같은/다음 프레임에 계속 덮어써서 지워버림
            // (두 스크립트가 같은 인디케이터를 두고 매 프레임 충돌하던 버그의 원인).
            if (definition == null)
            {
                _spriteRenderer.enabled = false;
                return;
            }

            // 카드 선택/게임오버/튜토리얼 등 게임을 멈추는 모든 상황이 공통적으로 Time.timeScale=0을
            // 쓰므로, 그 하나만 확인하면 프리뷰를 전부 숨길 수 있다 (TowerBuildController와 동일 원칙).
            if (definition.sprite == null || IsPointerOverUI() || Time.timeScale <= 0f)
            {
                _spriteRenderer.enabled = false;

                if (rangeIndicator != null)
                {
                    rangeIndicator.Hide();
                }

                return;
            }

            _spriteRenderer.enabled = true;
            _spriteRenderer.sprite = definition.sprite;

            float scale = SpriteFit.CalculateUniformScale(definition.sprite, targetVisualSize);
            transform.localScale = new Vector3(scale, scale, 1f);

            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            worldPosition.z = 0f;

            if (mapManager.TryGetSlotCell(worldPosition, out Vector3Int cell))
            {
                transform.position = mapManager.GetCellCenterWorld(cell);
                _spriteRenderer.color = mapManager.CanBuild(definition.Data.kind, cell) ? validColor : invalidColor;
            }
            else
            {
                transform.position = worldPosition;
                _spriteRenderer.color = invalidColor;
            }

            if (rangeIndicator != null)
            {
                rangeIndicator.Show(transform.position, definition.Data.DisplayRange);
            }
        }

        // 카드 선택 패널/빌드 메뉴 위에 마우스가 있을 때 프리뷰가 UI 위에 겹쳐 보이는 걸 방지.
        private static bool IsPointerOverUI() =>
            EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
