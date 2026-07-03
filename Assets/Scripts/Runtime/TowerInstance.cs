using System.Collections.Generic;
using RCCom.Data;
using RCCom.Definitions.Tower;
using RCCom.Effects.Tower;
using UnityEngine;

namespace RCCom.Runtime
{
    /// <summary>
    /// 건설된 타워 1기. 매니저 아키텍처 원칙(ARCHITECTURE.md 5단계): 타워는 설치 슬롯 제한으로
    /// 개체 수가 적어(공격3/스킬1/파워2, 카드로 확장돼도 6~8개 수준) Enemy처럼 순수 C#으로 뺄
    /// 필요 없이 일반 MonoBehaviour로 둔다. 별도 TowerManager 없이 자기 Awake/Update/트리거
    /// 콜백에서 직접 ITowerEffect 훅을 호출한다.
    ///
    /// definition은 프리팹에 인스펙터로 미리 연결해둘 것 (타워 종류별로 프리팹을 따로 둠).
    /// 아군 사거리 출입(스킬 타워)·적 사거리 진입(공격 타워) 감지는 Collider2D 트리거로 하므로,
    /// 이 프리팹과 EnemyView 양쪽에 Collider2D(+최소 한쪽에 Rigidbody2D)가 필요하다.
    /// </summary>
    public class TowerInstance : MonoBehaviour
    {
        public TowerDefinition definition;

        /// <summary>
        /// 공격 효과 등 주기적 효과가 자체적으로 쓰는 타이머. 효과(SO)는 여러 타워 인스턴스가
        /// 공유하는 상태 없는(stateless) 자산이라 인스턴스별 값은 반드시 여기(런타임 쪽)에 둔다.
        /// </summary>
        public float cooldownRemaining;

        /// <summary>
        /// 자신에게 걸린 스킬/파워 타워 버프 목록. 데미지 계산 시점에만 조회하고 스탯 자체는
        /// 건드리지 않는다. 스킬 타워가 OnAllyEnterRange/OnAllyExitRange에서 Add/Remove한다.
        /// </summary>
        public List<ITowerAura> activeAuras = new();

        private readonly List<EnemyInstance> _enemiesInRange = new();

        public TowerData Data => definition.Data;
        public Vector2 Position => transform.position;

        private void Awake()
        {
            TowerContext ctx = MakeContext();
            foreach (ITowerEffect effect in definition.effects)
            {
                effect.OnBuild(ctx);
            }
        }

        private void Update()
        {
            TowerContext ctx = MakeContext();
            foreach (ITowerEffect effect in definition.effects)
            {
                effect.OnTick(ctx);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out TowerInstance ally))
            {
                TowerContext ctx = MakeContext();
                foreach (ITowerEffect effect in definition.effects)
                {
                    effect.OnAllyEnterRange(ctx, ally);
                }
            }
            else if (other.TryGetComponent(out EnemyView enemyView))
            {
                _enemiesInRange.Add(enemyView.Instance);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent(out TowerInstance ally))
            {
                TowerContext ctx = MakeContext();
                foreach (ITowerEffect effect in definition.effects)
                {
                    effect.OnAllyExitRange(ctx, ally);
                }
            }
            else if (other.TryGetComponent(out EnemyView enemyView))
            {
                _enemiesInRange.Remove(enemyView.Instance);
            }
        }

        private TowerContext MakeContext() => new()
        {
            self = this,
            deltaTime = Time.deltaTime,
            activeEnemies = _enemiesInRange,
        };
    }
}
