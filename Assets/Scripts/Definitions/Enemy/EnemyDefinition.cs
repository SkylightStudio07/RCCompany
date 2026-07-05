using System.Collections.Generic;
using RCCom.Data;
using RCCom.Effects.Enemy;
using UnityEngine;

namespace RCCom.Definitions.Enemy
{
    /// <summary>
    /// 적 한 종류를 인스펙터에서 조립하는 SO. EnemyData(스탯) + 효과 목록 + 표시용 스프라이트를 묶는다.
    /// TowerDefinition과 달리 EnemyData는 EnemyKind로 종류만 구분하는 단일 클래스라
    /// (베이스 3종은 로직이 아닌 수치 차이) 상속 없이 concrete 클래스 하나로 충분하다.
    ///
    /// GDD "툴백 라인": 베이스 3종은 로직/프리팹을 그대로 두고 스프라이트만 교체해 다양성을
    /// 낸다 — 그래서 스프라이트도 EnemyView 프리팹을 종류별로 따로 두는 대신 여기(데이터)에
    /// 두고, EnemyView는 스폰 시 이 값을 읽어 자기 SpriteRenderer에 반영한다 (프리팹 1개 재사용).
    /// </summary>
    [CreateAssetMenu(menuName = "RCCom/Enemy/Enemy Definition")]
    public class EnemyDefinition : ScriptableObject
    {
        public EnemyData data = new();
        public List<EnemyEffectBase> effects = new();
        public Sprite sprite;

        /// <summary>
        /// 스프라이트 아트가 기본적으로 바라보는 방향 보정각 (오른쪽 기준 0, 위쪽 기준 90,
        /// 아래쪽 -90, 왼쪽 180). sprite와 마찬가지로 종류별 데이터 — 종류마다 아트가 서로
        /// 다른 방향을 보고 그려졌을 수 있어서(일부는 위, 일부는 아래 등) EnemyView 프리팹
        /// 공용 값 하나로는 전부 맞출 수 없어 여기(Definition)로 옮김.
        /// </summary>
        public float spriteForwardOffsetDegrees = 90f;
    }
}
