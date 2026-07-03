using System.Collections.Generic;
using RCCom.Data;
using RCCom.Runtime;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RCCom.Managers
{
    /// <summary>
    /// 게임 흐름 단계 매니저 중 하나 (ARCHITECTURE.md 5단계 원칙: "오브젝트 타입"별 매니저는
    /// 안 만들지만 "게임 흐름 단계"별 매니저는 4개 — GameManager/MapManager/WaveManager/CardManager).
    ///
    /// 그리드 기반 타워 설치 슬롯과 적 이동 웨이포인트 경로를 관리한다. {{user}} 지침: 타워
    /// 설치는 그리드(Tilemap) 기준이지만 적/플레이어 이동은 그리드와 무관해야 하므로, 이 둘을
    /// 서로 다른 데이터로 분리해서 갖는다 — 설치 슬롯은 slotTilemap의 셀 좌표, 웨이포인트는
    /// 그냥 Transform 배열(자유 좌표, 그리드 셀과 무관).
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        [Header("타워 설치 그리드 (슬롯이 칠해진 타일맵 레이어)")]
        [SerializeField] private Tilemap slotTilemap;

        [Header("적 이동 경로 (그리드 무관, 자유 좌표 — 씬에 배치한 오브젝트 순서대로)")]
        [SerializeField] private Transform[] waypoints;

        [Header("타워 종류별 설치 슬롯 제한 (GDD 기본값, 카드로 확장 가능)")]
        [SerializeField] private int maxAttackTowers = 3;
        [SerializeField] private int maxSkillTowers = 1;
        [SerializeField] private int maxPowerTowers = 2;

        private readonly Dictionary<Vector3Int, TowerInstance> _occupiedSlots = new();
        private int _attackTowerCount;
        private int _skillTowerCount;
        private int _powerTowerCount;

        private Vector2[] _waypointPositions;

        /// <summary>
        /// 적 이동용 웨이포인트 목록 (스테이지당 고정이라 Awake에서 한 번만 캐싱).
        /// WaveManager가 EnemyInstance 스폰 시 이 목록을 넘겨줄 것.
        /// </summary>
        public IReadOnlyList<Vector2> Waypoints => _waypointPositions;

        private void Awake()
        {
            _waypointPositions = new Vector2[waypoints.Length];
            for (int i = 0; i < waypoints.Length; i++)
            {
                _waypointPositions[i] = waypoints[i].position;
            }
        }

        /// <summary>월드 좌표가 속한 셀을 반환한다. 슬롯 타일맵에 실제로 칠해진 셀이어야 유효하다.</summary>
        public bool TryGetSlotCell(Vector3 worldPosition, out Vector3Int cell)
        {
            cell = slotTilemap.WorldToCell(worldPosition);
            return slotTilemap.HasTile(cell);
        }

        public bool CanBuild(TowerKind kind, Vector3Int cell)
        {
            if (!slotTilemap.HasTile(cell) || _occupiedSlots.ContainsKey(cell))
            {
                return false;
            }

            return kind switch
            {
                TowerKind.Attack => _attackTowerCount < maxAttackTowers,
                TowerKind.Skill => _skillTowerCount < maxSkillTowers,
                TowerKind.Power => _powerTowerCount < maxPowerTowers,
                _ => false,
            };
        }

        /// <summary>
        /// prefab을 슬롯 셀 중심 위치에 건설한다. 호출 전 CanBuild로 가능 여부를 확인해야 한다
        /// (여기서는 재확인하지 않음 — 예: 타워 설치 UI가 버튼 활성/비활성을 CanBuild로 미리 걸러야 함).
        /// </summary>
        public TowerInstance Build(TowerInstance prefab, Vector3Int cell)
        {
            Vector3 worldPosition = slotTilemap.GetCellCenterWorld(cell);
            TowerInstance instance = Instantiate(prefab, worldPosition, Quaternion.identity);

            _occupiedSlots[cell] = instance;
            IncrementCount(instance.Data.kind);

            return instance;
        }

        /// <summary>업그레이드 카드(예: "공격 타워 증설")가 호출해 슬롯 한도를 늘린다.</summary>
        public void IncreaseMaxSlots(TowerKind kind, int amount)
        {
            switch (kind)
            {
                case TowerKind.Attack:
                    maxAttackTowers += amount;
                    break;
                case TowerKind.Skill:
                    maxSkillTowers += amount;
                    break;
                case TowerKind.Power:
                    maxPowerTowers += amount;
                    break;
            }
        }

        private void IncrementCount(TowerKind kind)
        {
            switch (kind)
            {
                case TowerKind.Attack:
                    _attackTowerCount++;
                    break;
                case TowerKind.Skill:
                    _skillTowerCount++;
                    break;
                case TowerKind.Power:
                    _powerTowerCount++;
                    break;
            }
        }
    }
}
