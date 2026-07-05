using System.Collections.Generic;
using UnityEngine;

namespace RCCom.Definitions.Enemy
{
    /// <summary>
    /// 지금 게임에 존재하는 모든 적 종류(Definition)를 모아두는 프로젝트 창 에셋.
    /// WaveManager 등 여러 매니저가 각자 배열을 따로 들고 있지 않고 이 하나를 참조하게 해서,
    /// 새 적 종류가 추가될 때 한 곳만 고치면 되도록 한다. enemyId로 조회도 가능.
    /// </summary>
    [CreateAssetMenu(menuName = "RCCom/Enemy/Enemy Roster")]
    public class EnemyRoster : ScriptableObject
    {
        public List<EnemyDefinition> enemies = new();

        public EnemyDefinition FindById(string enemyId)
        {
            foreach (EnemyDefinition enemy in enemies)
            {
                if (enemy.data.enemyId == enemyId)
                {
                    return enemy;
                }
            }

            return null;
        }
    }
}
