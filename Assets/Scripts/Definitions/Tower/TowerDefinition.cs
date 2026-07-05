using System.Collections.Generic;
using RCCom.Data;
using RCCom.Effects.Tower;
using UnityEngine;

namespace RCCom.Definitions.Tower
{
    /// <summary>
    /// 타워 한 "종류"(예: 관통 사격 타워)를 인스펙터에서 조립하는 SO. TowerData(스탯) + 효과 목록을 묶는다.
    /// TowerData가 타입별로 필드가 달라 상속으로 나뉜 것과 마찬가지로, 이 정의도 타입별 concrete 클래스로 나눈다
    /// (다형 직렬화 문제 회피). 매니저 등 다른 시스템은 이 추상 베이스의 Data/effects만 보고 동작한다.
    ///
    /// concrete 파생 클래스(AttackTowerDefinition 등)는 각각 별도 파일에 정의한다 — ScriptableObject를
    /// 여러 개 한 파일에 몰아두면, 나중에 그중 하나 이름을 바꿀 때 이미 만든 .asset이 스크립트를
    /// 못 찾는(Missing Script) 문제가 생길 수 있어서 반드시 분리한다.
    /// </summary>
    public abstract class TowerDefinition : ScriptableObject
    {
        public List<TowerEffectBase> effects = new();

        /// <summary>
        /// 표시용 스프라이트. Enemy와 같은 이유로 타워도 프리팹을 종류별로 따로 만들지 않고
        /// 1개만 재사용한다 — TowerInstance.Build()가 이 값을 자기 SpriteRenderer에 반영한다.
        /// </summary>
        public Sprite sprite;

        public abstract TowerData Data { get; }

        [System.NonSerialized] private TowerDefinition _runtimeInstance;

        /// <summary>
        /// 업그레이드 카드(포격 강화 등)가 Data 필드를 직접 수정하는데, 이 SO는 프로젝트 에셋
        /// 그 자체라 Play 모드를 꺼도 그 변경이 자동으로 원복되지 않는다(씬 오브젝트와 달리
        /// 에셋은 스냅샷 복원 대상이 아님) — 그래서 플레이할 때마다 원본이 조금씩 오염된다.
        /// 이 메서드는 원본 대신 세션당 1회만 만들어지는 복제본을 돌려줘서, 실행 중 모든 변경이
        /// 복제본에서만 일어나고 원본 .asset은 항상 그대로 유지되게 한다. 캐시를 원본(this) 쪽에
        /// 두기 때문에, TowerRoster를 통해 얻든 UnlockTowerCard가 직접 얻든 항상 같은 복제본이
        /// 반환된다 — 여러 시스템이 서로 다른 복제본을 들고 값이 어긋나는 일이 없다.
        /// </summary>
        public TowerDefinition CreateRuntimeInstance()
        {
            if (_runtimeInstance == null)
            {
                _runtimeInstance = Instantiate(this);
            }

            return _runtimeInstance;
        }

        /// <summary>
        /// 재시작(Retry, SceneManager.LoadScene) 시 호출 — Editor Play 재시작과 달리 도메인
        /// 리로드가 없어 [NonSerialized] 캐시가 저절로 안 비워지므로, 새 세션 시작 시 명시적으로
        /// 캐시를 비워 다음 CreateRuntimeInstance() 호출이 새 복제본을 만들게 한다.
        /// </summary>
        public void ClearRuntimeInstance()
        {
            _runtimeInstance = null;
        }
    }
}
