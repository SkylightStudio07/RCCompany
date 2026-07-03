using System.Collections.Generic;
using RCCom.Data;
using RCCom.Effects.Enemy;
using UnityEngine;

namespace RCCom.Definitions.Enemy
{
    /// <summary>
    /// 적 한 종류를 인스펙터에서 조립하는 SO. EnemyData(스탯) + 효과 목록을 묶는다.
    /// TowerDefinition과 달리 EnemyData는 EnemyKind로 종류만 구분하는 단일 클래스라
    /// (베이스 3종은 로직이 아닌 수치 차이) 상속 없이 concrete 클래스 하나로 충분하다.
    /// </summary>
    [CreateAssetMenu(menuName = "RCCom/Enemy/Enemy Definition")]
    public class EnemyDefinition : ScriptableObject
    {
        public EnemyData data = new();
        public List<EnemyEffectBase> effects = new();
    }
}
