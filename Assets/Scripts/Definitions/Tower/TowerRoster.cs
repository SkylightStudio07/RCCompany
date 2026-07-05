using System.Collections.Generic;
using UnityEngine;

namespace RCCom.Definitions.Tower
{
    /// <summary>
    /// 지금 게임에 존재하는 모든 타워 종류(Definition)를 모아두는 프로젝트 창 에셋.
    /// WaveManager/TowerBuildController/CardManager 등 여러 매니저가 각자 배열을 따로
    /// 들고 있지 않고 이 하나를 참조하게 해서, 새 타워 종류가 추가될 때(카드로 신규 타워
    /// 해금 등) 한 곳만 고치면 되도록 한다. towerId로 조회도 가능.
    ///
    /// 타워는 프리팹이 1개뿐이라(TowerInstance.Build 참고) 여기엔 프리팹이 아니라
    /// Definition을 담는다 — MapManager가 건설 시 이 Definition을 공용 프리팹에 주입한다.
    /// </summary>
    [CreateAssetMenu(menuName = "RCCom/Tower/Tower Roster")]
    public class TowerRoster : ScriptableObject
    {
        public List<TowerDefinition> towers = new();

        [System.NonSerialized] private TowerRoster _runtimeInstance;

        /// <summary>
        /// UnlockTowerCard.Apply가 이 리스트에 직접 Add하는데, 이 SO 역시 프로젝트 에셋 그
        /// 자체라 Play 모드를 꺼도 추가한 항목이 원본 에셋에 그대로 남는다. TowerDefinition과
        /// 같은 이유로, 세션당 1회 복제본을 만들어 그 복제본의 리스트(+리스트에 담긴 각
        /// Definition도 TowerDefinition.CreateRuntimeInstance()로 복제)만 실행 중 변경되게 한다.
        /// TowerBuildController/CardManager/TowerBuildMenuUI가 각자 Awake에서 이 메서드를 호출해
        /// towerRoster 필드 자체를 복제본으로 바꿔치기한다.
        /// </summary>
        public TowerRoster GetRuntimeInstance()
        {
            if (_runtimeInstance != null)
            {
                return _runtimeInstance;
            }

            _runtimeInstance = Instantiate(this);
            _runtimeInstance.towers = new List<TowerDefinition>();

            foreach (TowerDefinition original in towers)
            {
                _runtimeInstance.towers.Add(original.CreateRuntimeInstance());
            }

            return _runtimeInstance;
        }

        /// <summary>
        /// 재시작(Retry) 시 GameManager가 호출 — 원본 towers 리스트에 담긴 각 Definition의
        /// 복제본 캐시까지 전부 비워야, 재시작한 세션이 이전 판의 카드 강화치를 이어받지 않는다.
        /// </summary>
        public void ClearRuntimeInstance()
        {
            foreach (TowerDefinition original in towers)
            {
                original.ClearRuntimeInstance();
            }

            _runtimeInstance = null;
        }

        public TowerDefinition FindById(string towerId)
        {
            foreach (TowerDefinition definition in towers)
            {
                if (definition.Data.towerId == towerId)
                {
                    return definition;
                }
            }

            return null;
        }
    }
}
