using System;
using RCCom.Definitions.Tower;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RCCom.UI
{
    /// <summary>
    /// 빌드 메뉴(스크롤 목록)에 동적으로 생성되는 버튼 1개. 종류(공격/스킬/파워)별 프레임
    /// 그림(이름이 이미 그려져 있는 템플릿) 위에 타워 아이콘/이름/비용을 얹어서 표시하고,
    /// 클릭 시 전달받은 콜백을 실행한다 — 어떤 인덱스를 선택하는지는 이 컴포넌트가 몰라도
    /// 되게 콜백으로 위임 (TowerBuildMenuUI가 인덱스를 캡처해서 넘겨줌).
    /// </summary>
    public class TowerBuildButton : MonoBehaviour
    {
        [SerializeField] private Image frame;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private Button button;

        public void Setup(TowerDefinition definition, Sprite frameSprite, Action onClick)
        {
            frame.sprite = frameSprite;
            icon.sprite = definition.sprite;
            nameText.text = definition.Data.displayName;
            costText.text = definition.Data.buildCost.ToString();

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick());
        }
    }
}
