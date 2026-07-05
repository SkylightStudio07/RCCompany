using RCCom.Definitions.Tower;
using RCCom.Managers;

namespace RCCom.Runtime
{
    /// <summary>
    /// ICardEffect.Apply에 전달되는 컨텍스트. 카드가 건드릴 수 있는 시스템들(플레이어, 타워
    /// 목록, 그리드 슬롯, 거점)에 대한 참조를 모아둔다. CardManager가 카드 선택 시점에 채워 넘긴다.
    /// </summary>
    public class CardContext
    {
        public PlayerController player;
        public TowerRoster towerRoster;
        public MapManager mapManager;
        public BaseController baseController;
        public GameManager gameManager;
    }
}
