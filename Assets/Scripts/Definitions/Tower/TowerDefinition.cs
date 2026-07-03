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
        public abstract TowerData Data { get; }
    }
}
