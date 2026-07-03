using System;
using UnityEngine;

namespace RCCom.Runtime
{
    /// <summary>
    /// 자식 오브젝트의 트리거 콜라이더 이벤트를 부모 컴포넌트로 중계하는 최소 헬퍼.
    /// 플레이어처럼 한 오브젝트에 서로 다른 반경의 콜라이더 2개(몸통 히트박스 vs 원거리
    /// 사거리)가 필요할 때, Unity가 OnTriggerEnter2D에서 "내 콜라이더 중 어느 것"인지
    /// 구분해주지 않아서 큰 쪽(사거리)을 자식 오브젝트로 분리하고 이 릴레이로 이어붙인다.
    /// 이 오브젝트의 Collider2D 반경은 사용할 스탯(사거리 등)에 맞춰 인스펙터에서 수동으로
    /// 설정할 것.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class AttackRangeTrigger : MonoBehaviour
    {
        public event Action<Collider2D> EnteredRange;
        public event Action<Collider2D> ExitedRange;

        private void OnTriggerEnter2D(Collider2D other) => EnteredRange?.Invoke(other);

        private void OnTriggerExit2D(Collider2D other) => ExitedRange?.Invoke(other);
    }
}
