using System.Collections.Generic;

namespace RCCom.Runtime
{
    /// <summary>
    /// ITowerEffect 훅에 전달되는 컨텍스트. 원본 CardContext(Project Vertex)가 없어
    /// 지금 필요한 최소 필드만 새로 정의함.
    /// </summary>
    public class TowerContext
    {
        public TowerInstance self;
        public float deltaTime;

        /// <summary>
        /// 현재 이 타워의 Collider2D 트리거 범위 안에 있는 적 목록. 타워 자신이
        /// OnTriggerEnter2D/Exit2D에서 채운다 (매니저가 채워주지 않음).
        /// </summary>
        public IReadOnlyList<EnemyInstance> activeEnemies;
    }
}
