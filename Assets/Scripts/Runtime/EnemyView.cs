using RCCom.Core;
using UnityEngine;

namespace RCCom.Runtime
{
    /// <summary>
    /// EnemyInstance(순수 C#)의 시각 표현만 담당하는 MonoBehaviour. 로직은 갖지 않고,
    /// 매 프레임 EnemyInstance.position을 읽어 transform.position에 반영하고, Died/ReachedGoal
    /// 이벤트를 구독해 스스로를 파괴한다. WaveManager가 EnemyInstance를 스폰/Tick하고
    /// 이 View를 Bind해 붙여준다.
    ///
    /// 이 오브젝트의 Collider2D는 두 가지 용도로 쓰인다:
    /// 1) Tower의 Collider2D 트리거가 적을 감지하는 대상 (Tower 쪽에서 감지)
    /// 2) 자신이 플레이어/거점 등 IDamageable과 접촉했을 때 접촉 피해를 주는 판정 (아래 OnTriggerEnter2D)
    /// 트리거 감지엔 최소 한쪽에 Rigidbody2D도 필요 — 프리팹 설정 시 유의.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class EnemyView : MonoBehaviour
    {
        public EnemyInstance Instance { get; private set; }

        public void Bind(EnemyInstance instance)
        {
            Instance = instance;
            Instance.Died += HandleRemoved;
            Instance.ReachedGoal += HandleRemoved;
        }

        private void OnDestroy()
        {
            if (Instance != null)
            {
                Instance.Died -= HandleRemoved;
                Instance.ReachedGoal -= HandleRemoved;
            }
        }

        private void LateUpdate()
        {
            transform.position = Instance.position;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out IDamageable target))
            {
                Instance.DealContactDamageTo(target);
            }
        }

        private void HandleRemoved()
        {
            Destroy(gameObject);
        }
    }
}
