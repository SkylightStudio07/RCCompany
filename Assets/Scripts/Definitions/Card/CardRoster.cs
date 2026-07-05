using System.Collections.Generic;
using RCCom.Effects.Card;
using UnityEngine;

namespace RCCom.Definitions.Card
{
    /// <summary>
    /// 업그레이드 카드 15장을 모아두는 프로젝트 창 에셋. Tower/EnemyRoster와 달리 지금은
    /// CardManager 하나만 참조해서 "여러 매니저 공유" 필요성은 약하지만, 씬을 열지 않고
    /// 프로젝트 창에서 카드 구성을 확인/편집(밸런싱)할 수 있는 편의를 위해 둔다.
    /// </summary>
    [CreateAssetMenu(menuName = "RCCom/Card/Card Roster")]
    public class CardRoster : ScriptableObject
    {
        public List<CardEffectBase> cards = new();
    }
}
