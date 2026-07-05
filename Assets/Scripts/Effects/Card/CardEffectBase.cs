using RCCom.Data;
using RCCom.Runtime;
using UnityEngine;

namespace RCCom.Effects.Card
{
    /// <summary>
    /// 업그레이드 카드 1장. Tower/Enemy와 달리 카드마다 로직이 전부 달라서(플레이어 강화,
    /// 타워 강화, 슬롯 확장, 거점, 신규 타워 해금 등) 재사용되는 "Data 모양"이 없다 — 그래서
    /// 별도 Data 컨테이너 없이 이름/설명도 이 SO에 직접 둔다.
    /// </summary>
    public abstract class CardEffectBase : ScriptableObject, ICardEffect
    {
        public string displayName;

        [TextArea]
        public string description;

        [SerializeField] private TowerKind category = TowerKind.Attack;

        /// <summary>
        /// 카드 선택 UI가 프레임(테두리/아이콘)을 고를 때 참조. 기본값은 Attack — 파워/스킬
        /// 관련 카드만 인스펙터에서 값을 바꾸거나 재정의하면 되고, 플레이어/거점처럼 특정
        /// 타워 종류와 무관한 카드는 기본값(Attack) 그대로 두면 "그 외" 프레임으로 표시된다.
        /// TowerSlotExpansionCard/UnlockTowerCard처럼 이미 자기만의 TowerKind 필드가 있는
        /// 카드는 그 필드를 그대로 반환하도록 재정의해 값이 두 군데 따로 관리되는 걸 피한다.
        /// </summary>
        public virtual TowerKind Category => category;

        /// <summary>
        /// 지금 뽑을 가치가 있는 카드인지 (예: 이미 해금된 신규 타워 카드는 다시 뽑아도 효과가
        /// 없으므로 후보에서 제외). 기본값은 항상 true — 대부분의 카드(스탯 강화 등)는
        /// 몇 번을 뽑아도 유효하다. 필요한 카드만 재정의.
        /// </summary>
        public virtual bool IsAvailable(CardContext ctx) => true;

        public abstract void Apply(CardContext ctx);
    }
}
