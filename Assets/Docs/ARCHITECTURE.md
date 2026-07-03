# TVD 아키텍처 노트

이 문서는 개발 중 내려진 구조적 결정과 그 이유를 기록한다. 게임 기획 자체는 Notion GDD(`GDD 초안`)를 따르며, 여기서는 "그 기획을 어떤 코드 구조로 구현할지"만 다룬다.

## 레이어 구조 (합의된 4계층)

1. **데이터 컨테이너** — `TowerData`, `EnemyData`, `PlayerData` 등. MonoBehaviour도 ScriptableObject도 아닌 순수 C# 클래스(`[Serializable]`). 스탯 수치의 "모양(shape)"만 정의한다.
2. **데이터 기반 SO** — `TowerAbility`, `EnemyAbility` 등. ScriptableObject 에셋으로, 위 Data를 감싸거나 참조하면서 인스펙터에서 드래그 앤 드롭으로 값을 채울 수 있게 한다. 특정 능력/행동 로직(전략 패턴)의 확장 지점이 된다.
3. **렌더러 프리팹** — `Attack_Tower_Default.prefab` 등. 시각적 표현만 담당하는 GameObject. 게임 로직을 갖지 않는다.
4. **매니저** — `TowerManager`, `EnemyManager`, `WaveManager` 등 MonoBehaviour. SO/Data를 읽어 스폰, 배치, 전투 로직을 조율한다.

**현재 단계: 1번(데이터 컨테이너)만 작업 중.** 2~4번은 이후 단계에서 진행하며, 이 문서에 이어서 기록한다.

## 데이터 컨테이너 설계 근거

### EnemyData
- GDD가 확정한 "베이스 3종 + 수식어" 구조를 반영. 3종(일반/돌진/탱커)은 로직이 아니라 수치(체력/속도) 차이이므로 클래스를 나누지 않고 `EnemyKind` enum으로 구분되는 단일 클래스로 설계.
- `waveCost`, `minWave`는 GDD 표에 확정값이 있음 (일반 1/W1, 돌진 1.5/W3, 탱커 2.5/W5). 나머지(`maxHealth`, `moveSpeed`, `contactDamage`, 보상)는 GDD가 "추후 논의"로 남긴 값이라 기본값을 강제하지 않고, 추후 SO 에셋 생성 시 인스펙터에서 직접 채우도록 비워둔다.
- `contactDamage`는 적이 충돌한 대상(플레이어 또는 거점)에게 주는 피해로 단일화. 플레이어 공격과 거점 공격을 분리해야 한다는 기획이 나오면 필드를 분리할 것.

### TowerData
- GDD가 타워를 공격/스킬/파워 3분류로 "확정"했고, 세 타입은 필드 자체가 다르다 (공격=데미지/사거리/공속, 스킬=오라 사거리/버프량, 파워=글로벌 버프/누적치). 그래서 억지로 필드를 공유시키지 않고 `TowerData` 추상 베이스 + `AttackTowerData`/`SkillTowerData`/`PowerTowerData` 3개 파생 클래스로 분리.
- 주의: 순수 C# 클래스 상속은 Unity 인스펙터에서 다형적으로 자동 노출되지 않는다. 2단계(SO)에서 타입별로 별도 SO 클래스를 만들거나 `[SerializeReference]`를 쓰는 방식으로 해결 예정 — 지금 단계에서는 "모양"만 정의.
- 슬롯 제한(공격3/스킬1/파워2, 카드로 확장)은 타워 개별 데이터가 아니라 전역 설정이므로 여기 포함하지 않음 → 매니저/게임 설정 단계에서 처리.

### PlayerData
- {{user}} 확인: 플레이어도 체력을 보유하고 적의 공격으로 피해를 입는다 (거점 체력과 별개). `maxHealth` 필드 포함.
- 기본 공격/스킬 수치는 GDD에 "세부는 추후 논의"로 명시되어 있어 임의로 확정하지 않고, 인스펙터에서 조정 가능한 필드만 정의 (데미지/사거리/공속/투사체 속도, 스킬 쿨다운/범위/피해).

## 그리드/이동 분리 (추후 매니저 단계 유의사항)
- 타워 설치는 그리드(Tilemap) 좌표계를 사용하지만, 적/플레이어의 이동은 그리드와 무관한 자유 좌표(Transform/Vector2) 기반이어야 함. Data 계층에는 영향 없음 — Manager/Controller 단계에서 지켜야 할 제약으로 기록.

## 웨이브 절차적 생성의 난수 요구사항 (추후 WaveManager 단계 유의사항)
- 웨이브 스폰 로직은 System.Random(고정 시드)을 웨이브 매니저 전용으로 사용해 UnityEngine.Random(전역 상태)과 분리해야 재현성(같은 시드 → 같은 웨이브 구성)과 독립성(다른 시스템의 Random 호출이 웨이브 결과에 영향 주지 않음)이 보장됨. Data 계층에는 영향 없음 — WaveManager 구현 시 반영.

## 2단계: 효과(Effect) 훅 레이어 — GDD "기술 노트" 반영

GDD에 추가된 기술 노트(2026-07-04)가 2단계("데이터 기반 SO")의 실제 설계를 지정함. 초기 지시의 "TowerAbility/EnemyAbility"라는 이름은 기술 노트의 `ITowerEffect`/`TowerEffectBase` 네이밍으로 대체됨 (더 구체적인 최신 스펙이므로 이쪽을 따름).

- **계약(인터페이스) + 편의(추상 클래스) 병행**: `ITowerEffect`(계약, 독립성 보장) / `TowerEffectBase : ScriptableObject, ITowerEffect`(빈 기본 구현 제공). 실제 효과는 필요한 훅만 오버라이드.
- **훅 시점**: 공격 타워=`OnTick`(주기 반복), 스킬 타워=`OnAllyEnterRange`/`OnAllyExitRange`(아군 타워 사거리 출입 감지), 파워 타워=`OnBuild`(건설 즉시 1회).
- **스킬 타워 버프 = 직접 가감 아님, 파이프라인 조회**: `ITowerAura.ModifyOutgoingDamage(float)`. 공격 타워는 자신에게 걸린 `List<ITowerAura>`를 들고 있다가 데미지 계산 시점에만 조회 — 사거리 이탈 시 자동 소실, 중첩도 안전.
- **Enemy도 동일 패턴으로 설계** ({{user}} 확인, 2026-07-04: Project Vertex 개발 당시 EnemyAction도 CardEffect의 거의 미러였음): `IEnemyEffect` / `EnemyEffectBase`. 훅은 Tower와 이름은 다르지만 성격은 대응: `OnSpawn`(≈OnBuild, 스폰 시 1회) / `OnTick`(반복) / `OnDealContactDamage(ctx, IDamageable target)`(≈아군 사거리 출입에 대응하는 "Enemy 고유 상호작용 시점" — 접촉 피해를 주는 순간) / `OnDeath`(사망 시 1회, 정리·보상 트리거용으로 추가).
- **StatusType은 이 프로젝트용으로 새로 정의** ({{user}} 확인: Project Vertex 원본 소스 미보유라 그대로 재활용 불가). 의미:
  - `Weak`: 대상이 주는 피해 감소
  - `Vulnerable`: 대상이 받는 피해 증가
  - `Poison`: 매 틱 고정 피해 (DoT)
  - `Slow`: 이동속도 감소 (카드14 빙결 오라 타워가 사용 — 지속시간이 아니라 스킬 타워 아군 로직처럼 "사거리 안에 있는 동안"만 적용되는 aura형 적용 예정)
  - `Regen`: 매 틱 고정 회복 (카드15 재생의 축 — 거점 대상)
  - `Strength`: 공격력 고정치 증가
  - `Dexterity`: 예약값. 이 게임엔 블록/방어 스탯 개념이 없어 현재 대응되는 효과 없음 — 필요해지면 의미를 재정의할 것. 그 전까지는 사용하지 않음.
  - 상태이상의 지속시간 추적/스택 규칙(예: `StatusEffectInstance`)은 이번 단계에 포함하지 않음 — Poison/Regen처럼 실제로 틱 기반 상태이상을 쓰는 구체 효과를 만들 때 필요한 만큼만 설계해서 추가 (지금 만들면 쓰이지 않는 반쪽 구현이 됨).
- **`IDamageable`**: `OnDealContactDamage`에서 피해 대상을 추상화하기 위해 최소 계약(`TakeDamage(float)`)만 정의. 플레이어/거점이 구현 예정 (실제 MonoBehaviour 구현체는 매니저/컨트롤러 단계에서 작성).
- **TowerInstance/TowerContext, EnemyInstance/EnemyContext**: 기술 노트가 `CardContext` → `TowerContext`로 이름만 바꾸는 것을 제안했지만 원본 `CardContext`가 없어 필요한 최소 필드만 새로 설계함 (`self` 런타임 인스턴스, `deltaTime`). 얘네는 4계층 중 어디에도 속하지 않는 "런타임 상태" 개념이라 별도 `RCCom.Runtime` 네임스페이스로 분리 (Data=설계 스탯, Runtime=게임 중 변하는 상태).

### 다음 작은 단위 (보류, 다음 턴에 진행)
- Tower/Enemy "Definition" SO 래퍼: `TowerData`(또는 그 파생) + `List<TowerEffectBase>`를 묶어 인스펙터에서 드래그 조립할 수 있는 실제 SO 에셋 클래스. 이번 턴엔 계약(인터페이스/베이스클래스)까지만 만들고, 이 래퍼는 다음 단계에서 설계 확인 후 진행.
- 구체 효과 구현체(DamageEffect, AuraBuffEffect, GlobalBuffEffect 등)도 다음 단계.

## 3단계: Tower Definition SO + 기본 효과 구현체 ({{user}} 지시: "타워 쪽 먼저")

Tower/Enemy 중 Tower를 먼저 끝까지(Definition SO + 3종 기본 효과) 완성. Enemy 쪽 Definition/구체 효과는 아직 미착수.

### TowerDefinition (`Assets/Scripts/Definitions/Tower/TowerDefinition.cs`)
- 추상 베이스 `TowerDefinition : ScriptableObject` (`effects` 리스트 + `abstract TowerData Data { get; }`) + `AttackTowerDefinition`/`SkillTowerDefinition`/`PowerTowerDefinition` 3개 concrete 클래스. `TowerData`가 상속으로 나뉜 것과 같은 이유(다형 직렬화 회피, 타입별 필드 차이)로 Definition도 동일하게 나눔.
- 매니저 등 다른 시스템은 concrete 타입을 몰라도 `TowerDefinition.Data`/`effects`만으로 동작 가능.

### TowerInstance/TowerContext 확장
- `TowerInstance.definition`(TowerDefinition 참조, `Data` 프로퍼티로 위임) + `cooldownRemaining`(공격 효과 타이머). 효과(SO)는 여러 인스턴스가 공유하는 상태 없는 자산이라, 인스턴스별로 달라지는 값은 전부 `TowerInstance`(런타임) 쪽에 둬야 한다는 원칙을 여기서도 적용.
- `TowerContext.activeEnemies`: 현재 살아있는 적 전체 목록. 사거리 필터링은 효과 쪽(타워 종류별 사거리 의미가 다름)에서 직접 수행하도록 해 매니저는 "전체 목록 제공"만 책임지도록 역할을 나눔.

### ITowerAura 확장: ModifyAttackInterval 추가
- `SkillTowerData.attackSpeedBuffMultiplier`를 실제로 반영할 파이프라인 지점이 기술 노트 스펙엔 없어서(`ModifyOutgoingDamage`만 정의) 대칭으로 `ModifyAttackInterval(float baseInterval)`을 추가. 데미지와 동일한 파이프라인 철학(직접 가감 아님, 조회 시점에만 적용) 유지.

### 구현 중 발견한 설계 간극과 해결: ITowerAura가 컨텍스트를 안 받는 문제
- 기술 노트의 `ITowerAura.ModifyOutgoingDamage(float baseDamage)`는 파라미터가 baseDamage 하나뿐이라, 버프를 "누가 제공했는지" 그 자리에서 알 수 없음. 그런데 `TowerEffectBase`(SO)는 여러 타워 인스턴스가 공유하는 상태 없는 자산이어야 해서, 효과 자산 자체에 버프 값을 박아둘 수 없음.
- **해결**: `AuraBuffEffect`가 `OnAllyEnterRange`/`OnAllyExitRange` 시점에 (제공자 TowerInstance, 대상 TowerInstance) 쌍을 key로 하는 `Dictionary`에 작은 `Aura`(ITowerAura 구현, private nested class) 인스턴스를 만들어 보관하고, 그 `Aura` 객체를 대상의 `activeAuras`에 Add/Remove. 인터페이스 시그니처는 기술 노트 그대로 유지하면서, 같은 정의(Definition)를 쓰는 스킬 타워가 여러 대 지어져도 값이 섞이지 않게 함.
- `GlobalBuffEffect`도 동일 원칙: `OnBuild` 시점에 캡처한 값을 가진 `GlobalDamageAura` 인스턴스를 만들어 `GlobalTowerAuraRegistry.Auras`(전역 static 리스트)에 등록. 파워 타워는 사거리 개념이 없으므로 스킬 타워의 "로컬 리스트" 대신 "전역 리스트"에 등록한다는 점만 다르고, 데미지 계산 시점에 조회하는 파이프라인 자체는 스킬 타워와 완전히 동일 (`DamageEffect.CalculateDamage`가 `self.activeAuras`와 `GlobalTowerAuraRegistry.Auras`를 모두 순회).

### GlobalTowerAuraRegistry (`Assets/Scripts/Runtime/GlobalTowerAuraRegistry.cs`)
- **임시방편으로 인지하고 있음**: TowerManager(4단계, 아직 없음)가 생기기 전까지 전역 버프를 모아둘 곳이 없어 static 컨테이너로 둠. Manager 도입 시 인스턴스 필드로 옮기고, 플레이 세션 시작 시 `Clear()` 하도록 바꿀 것 (에디터 도메인 리로드로 static 값이 남아있을 수 있음 — 지금 당장 문제가 되진 않지만 Manager 단계에서 반드시 정리).

### 구체 효과 3종 (`Assets/Scripts/Effects/Tower/Concrete/`)
- `DamageEffect` (공격 타워, OnTick): 쿨다운 감소 → 사거리 내 최근접 적 탐색 → `activeAuras`+전역 레지스트리로 데미지/공격주기 파이프라인 적용 → `IDamageable.TakeDamage` 호출. 투사체 등 시각 표현(렌더러 프리팹)은 포함하지 않음 — 다음 레이어에서 이 효과가 실제로 맞췄다는 걸 알리는 이벤트/콜백을 붙이거나, 매니저가 결과를 보고 시각효과를 트리거하는 방식으로 연결할 것.
- `AuraBuffEffect` (스킬 타워, OnAllyEnterRange/Exit): 위 설계 간극 해결 방식대로 구현.
- `GlobalBuffEffect` (파워 타워, OnBuild): 위 설계 간극 해결 방식대로 구현. `upgradeIncrement`(카드로 누적 강화)는 아직 미소비 — 업그레이드 카드 시스템 단계에서 등록된 `GlobalDamageAura.bonus`를 직접 증가시키는 방식으로 연결 예정.

### EnemyInstance가 IDamageable을 구현하도록 변경
- `DamageEffect`가 적에게 데미지를 넣으려면 적도 `IDamageable`이어야 함. 애초 문서엔 "플레이어/거점이 구현 예정"이라고만 적었었는데, 적도 포함하도록 범위를 넓힘 (당연한 확장이라 별도 질의 없이 진행).

### 아직 안 한 것 (다음 단계 후보)
- ~~Enemy 쪽 Definition SO + 구체 효과~~ → 아래 4단계에서 완료.
- 실제로 이 모든 걸 프레임마다 호출하는 TowerManager (OnTick 매 프레임 호출, OnBuild 건설 시 1회 호출, OnAllyEnterRange/Exit 판정 로직, activeEnemies 갱신 등) — 지금까지 만든 건 "로직은 맞지만 아무도 호출 안 하는" 상태. Definition 에셋 실제 생성(Attack/Skill/Power 기본 타워 3종 + 효과 SO 에셋들) + Manager 구현이 다음 우선순위.
- 그리드(Tilemap) 배치 시스템과의 연결 — TowerInstance.position은 있지만 그리드 슬롯 좌표 변환은 아직 없음.

## 4단계: Enemy Definition SO + 기본 효과 ({{user}} 지시: "Enemy도 Tower와 같은 패턴으로")

### EnemyDefinition (`Assets/Scripts/Definitions/Enemy/EnemyDefinition.cs`)
- `TowerDefinition`과 달리 상속 없이 concrete 클래스 하나(`EnemyDefinition : ScriptableObject` — `EnemyData data` + `List<EnemyEffectBase> effects`). 이유: `EnemyData`가 애초에 `EnemyKind`로 종류만 구분하는 단일 클래스로 설계됐기 때문(베이스 3종은 로직이 아니라 수치 차이) — Tower처럼 타입별 필드가 갈리지 않아 다형 직렬화 문제 자체가 없음.

### EnemyInstance가 EnemyDefinition을 참조하도록 변경
- `TowerInstance.definition → Data` 패턴과 동일하게 `EnemyInstance.definition(EnemyDefinition) → Data(EnemyData)` 프로퍼티로 위임하도록 변경 (기존엔 `EnemyData data` 필드를 직접 들고 있었음). 대칭성 유지 목적 — Enemy는 다형성 문제가 없어서 꼭 필요한 변경은 아니었지만, Tower와 동일한 접근 패턴(`ctx.self.Data`)을 쓰게 해서 효과 코드 스타일을 통일함.

### 구체 효과: ContactDamageEffect만 우선 구현 (`Assets/Scripts/Effects/Enemy/Concrete/ContactDamageEffect.cs`)
- GDD "다이" 섹션(플레이어 접촉 시 1회성 고정 피해, 어그로/타겟팅 없음)을 그대로 반영. `OnDealContactDamage(ctx, target)` 훅 하나만 오버라이드.
- 나머지 훅(OnSpawn/OnTick/OnDeath)은 베이스 3종에는 아직 쓸 데가 없어 구현체를 만들지 않음 — Tower가 3개 훅 전부에 대응하는 기본 효과가 있었던 것과 달리, Enemy는 현재 "접촉 피해"라는 상호작용 지점 하나만 확정 기획이라 그것만 구현 (향후 정예/무리, 보스, 독 타워 연동 등에서 나머지 훅에 대응하는 효과가 추가될 것).
- **연속 피격 방지(피격 후 짧은 무적)는 이 효과의 책임이 아님**: 대상(플레이어)의 `IDamageable.TakeDamage` 구현이 자체 무적 타이머를 갖고 스스로 걸러내야 함 — 플레이어 컨트롤러 구현 시 반드시 반영할 것 (지금 잊으면 GDD가 명시한 "동일 적에게 연속 피격 방지" 요구사항이 빠지게 됨).

## 5단계: 매니저 아키텍처 원칙 (GDD 기술 노트, Project Vertex 코드 분석 기반) — 이전 설계 일부 정정

{{user}} 확인: "그리드 처리는 MapManager가 맞고, Tower/Enemy 매니저는 굳이 필요한가 싶다" → GDD 기술 노트에 Project Vertex(이전 프로젝트) 실제 코드 분석 결과가 추가됨. **핵심 원칙: 매니저는 "게임 흐름 단계"당 1개만 두고, "오브젝트 타입"별 매니저(EnemyManager, TowerManager 등)는 만들지 않는다.** 개별 오브젝트가 스스로 상태/로직을 들고 이벤트로 알리고, 매니저는 리스트 순회나 흐름 조율만 한다.

**최종 매니저 구성**: `GameManager` / `MapManager`(그리드·배치) / `WaveManager`(웨이브 절차적 생성 + 적 리스트 순회) / `CardManager`(업그레이드 카드) — 4개뿐. TowerManager, EnemyManager 없음.

**개체별 처리 방식이 Tower/Enemy가 서로 다름** (개체 수 차이 때문):
- **Enemy = 순수 C# 인스턴스** (MonoBehaviour 아님, 성능 이유 — 실시간 웨이브라 동시 개체 수가 Vertex의 턴제 전투(~5)보다 훨씬 많을 수 있어 `MonoBehaviour.Update()` 오버헤드가 치명적). `EnemyInstance`가 스스로 `Tick(dt)`, `TakeDamage`를 갖고 `Damaged`/`Died` 이벤트로 알림. `WaveManager`가 `List<EnemyInstance>`를 들고 자기 `Update()`에서 직접 순회하며 `Tick()` 호출 (별도 EnemyManager 없음). `EnemyView`(MonoBehaviour, 스프라이트)는 로직 없이 매 프레임 `Instance.position`만 읽어 `transform.position`에 반영하고 `Died` 구독해 자기 파괴.
- **Tower = 일반 MonoBehaviour + Collider2D 트리거**. 설치 슬롯 제한(공격3/스킬1/파워2, 카드로 확장해도 6~8개 수준)으로 개체 수가 원래 적어 성능 문제가 없음 — 억지로 순수 C#화할 필요 없음. 각 타워가 자기 `Awake()`/`Update()`/`OnTriggerEnter2D`/`OnTriggerExit2D`에서 직접 `ITowerEffect` 훅을 호출 (별도 TowerManager 없음). 아군 타워 사거리 출입(스킬 타워)과 적 사거리 진입(공격 타워) 둘 다 같은 방식(Collider2D 트리거)으로 감지 — 단, 적을 감지하려면 `EnemyView`도 Collider2D를 가지고 있어야 함(트리거 감지엔 최소 한쪽이 Rigidbody2D 필요 — 프리팹 설정 시 유의).

### 이 결정이 이미 만든 코드에 미치는 영향 → 정정
- **`TowerInstance`를 순수 C#에서 `MonoBehaviour`로 전환**. 원래 Enemy와 대칭 맞추려고 순수 C#으로 설계했었는데, 위 원칙에 따르면 이건 잘못된 선택 — Tower는 개체 수가 적어서 그냥 MonoBehaviour가 맞음. `position`은 이제 `transform.position`을 그대로 쓰는 `Position` 프로�터티로 바뀜 (`DamageEffect.cs`의 `ctx.self.position` 참조도 `Position`으로 수정).
- **`TowerContext.activeEnemies`는 이제 매니저가 아니라 타워 자기 자신이 Collider2D 트리거로 채운다** — 필드 존재/타입/소비 방식(`DamageEffect`)은 그대로, "누가 채워주는지"만 바뀜(매니저 → 타워 자신).
- **`ITowerEffect`/`TowerEffectBase`/`ITowerAura`/`TowerDefinition`/`DamageEffect`/`AuraBuffEffect`/`GlobalBuffEffect`는 변경 없음** — 이 원칙은 "누가 Tick을 호출하고 범위를 감지하는가"에 대한 것이지 "효과를 어떻게 데이터 기반으로 조립하는가"와는 별개 문제라서 그대로 유효함.
- **`EnemyInstance`에 `Tick(dt)`/`Damaged`/`Died` 이벤트 추가 + `EnemyView`(MonoBehaviour) 신설** — 기존엔 효과 훅(OnSpawn/OnTick/OnDealContactDamage/OnDeath)만 정의돼 있고 아무도 호출을 안 했는데, 이제 `EnemyInstance` 스스로 `Spawn()`/`Tick()`/`TakeDamage()`/`DealContactDamageTo()` 메서드에서 자기 `definition.effects`를 구동함. `WaveManager`(아직 미구현)가 이 인스턴스들의 리스트를 들고 순회하며 `Tick()`만 불러주면 됨.
- **`GlobalTowerAuraRegistry`**: 애초 "TowerManager 도입 시 옮길 것"이라고 적어뒀는데, TowerManager 자체가 생기지 않으므로 **GameManager의 인스턴스 필드로 옮길 것**으로 정정. GameManager도 아직 없어서 지금 당장은 그대로 static 유지.

### 코드 반영 완료
- `TowerInstance`(`Assets/Scripts/Runtime/TowerRuntime.cs`): `MonoBehaviour`로 전환. `Awake()`에서 `OnBuild`, `Update()`에서 `OnTick`, `OnTriggerEnter2D/Exit2D`에서 아군(`TowerInstance`)이면 `OnAllyEnterRange/Exit`, 적(`EnemyView`)이면 `_enemiesInRange` 목록만 갱신(공격 타워용). `Position`은 `transform.position` 프록시 프로퍼티.
- `EnemyInstance`(`Assets/Scripts/Runtime/EnemyRuntime.cs`): 순수 C# 유지, `Spawn()`/`Tick(dt)`/`DealContactDamageTo(target)`/`TakeDamage(amount)` 메서드가 각각 `OnSpawn`/`OnTick`/`OnDealContactDamage`/`OnDeath` 효과 훅을 스스로 구동. `Damaged`/`Died` 이벤트 추가.
- `EnemyView`(`Assets/Scripts/Runtime/EnemyView.cs`, 신규): MonoBehaviour, 로직 없이 `LateUpdate()`에서 `Instance.position`을 `transform.position`에 반영하고 `Died` 구독해 자기 파괴.
- `DamageEffect.cs`: `ctx.self.position` → `ctx.self.Position`으로 수정 (TowerInstance가 MonoBehaviour가 되며 프로퍼티명 변경).
- `GlobalTowerAuraRegistry.cs`: 주석을 "TowerManager 도입 시" → "GameManager 도입 시"로 정정 (TowerManager는 만들지 않기로 확정됐으므로).

### 아직 없음 (다음 단계)
- `GameManager`/`MapManager`/`WaveManager`/`CardManager` 자체 — 지금까지 만든 Tower/Enemy 로직을 실제로 살아 움직이게 하려면 최소한 `WaveManager`(적 스폰 + `List<EnemyInstance>` 순회하며 `Tick()` 호출)와 `MapManager`(그리드 슬롯 계산 + 타워 프리팹 배치)가 필요.
- 적 이동(웨이포인트 추종) 로직 — `EnemyInstance.Tick()`에 자리는 마련해뒀지만 실제 이동 계산은 비어있음.
- Tower/Enemy 프리팹 실제 생성 및 Collider2D/Rigidbody2D 세팅 — 지금은 스크립트 계약만 존재.

## 6단계: 플레이어 컨트롤러

Player는 개체가 하나뿐이라 Tower와 같은 이유로 그냥 `MonoBehaviour`(`PlayerController`)로 둔다.
Tower/Enemy처럼 Definition+Effect 레이어를 새로 만들지는 않았음 — 이유: Tower/Enemy는
"교체 가능한 여러 종류"(공격/스킬/파워, 일반/돌진/탱커)가 있어서 전략 패턴(효과 SO)이 필요했지만,
Player는 영원히 하나의 정체성이고 카드로 받는 변화도 전부 숫자 비율 조정(공격력 +15% 등)이라
효과 훅 시스템이 필요 없음 — `PlayerData`(이미 있음)를 인스펙터에 직접 물리는 것으로 충분.

### 사거리 판정: 몸통 히트박스 vs 원거리 사거리 콜라이더 분리
- 접촉 피해(GDD "다이" 섹션, 근접 거리에서만 발생)와 원거리 자동 공격 사거리는 반경이 서로
  다르다. Unity는 한 오브젝트에 콜라이더가 여러 개 있어도 `OnTriggerEnter2D`에서 "내 콜라이더
  중 어느 것이 부딪혔는지" 구분해주지 않으므로, 원거리 사거리용 큰 콜라이더는 자식 오브젝트로
  분리하고 `AttackRangeTrigger`(`Assets/Scripts/Runtime/AttackRangeTrigger.cs`, 신규)라는
  최소 릴레이 컴포넌트로 이벤트만 부모에 전달한다. 플레이어 본체의 콜라이더는 몸통
  히트박스(접촉 피해 판정용)로 남긴다.
- 접촉 피해 판정 자체는 `PlayerController`가 아니라 `EnemyView`가 담당하도록 함(적이 플레이어를
  건드렸을 때 `EnemyInstance.DealContactDamageTo`를 호출) — `EnemyView.OnTriggerEnter2D`가
  `IDamageable`을 구현한 아무 대상(플레이어든 나중에 만들 거점이든)에게 범용으로 반응하도록
  작성해 거점이 생겨도 코드 변경이 필요 없게 함.

### 공유 유틸리티로 뺀 것: EnemyTargeting
- "후보 목록에서 사거리 내 가장 가까운 적 찾기"는 공격 타워(`DamageEffect`)와 플레이어 기본
  공격이 완전히 동일한 로직이 필요해서, `Assets/Scripts/Core/EnemyTargeting.cs`(신규)로 추출해
  양쪽에서 재사용. `DamageEffect`의 기존 private 메서드는 제거하고 이 유틸리티 호출로 교체.

### 입력: 기존 InputSystem_Actions 재사용, 세부는 임시 결정
- 프로젝트에 이미 있는 `InputSystem_Actions.inputactions`(Unity 기본 템플릿)의 `Player` 액션맵
  중 `Move`(이동)를 그대로 사용. 기본 공격은 GDD상 "자동 발동"(사거리 내 최근접 적, 쿨다운
  기반)이라 별도 입력이 필요 없어서, 남는 `Attack` 버튼 액션을 스킬 발동으로 재사용하기로
  임의 결정함 (GDD가 스킬 조작 방식을 확정하지 않았고, 액션 이름과 실제 용도가 다르다는 점은
  인지하고 있음 — 나중에 카드 시스템 등에서 별도 액션이 필요해지면 이름을 정리할 것).

### 아직 없음 (다음 단계)
- 실제 씬에 Player GameObject 배치 + Collider2D/Rigidbody2D/자식 AttackRangeTrigger 오브젝트
  구성 (인스펙터 수동 작업, {{user}} 몫).
- 투사체 시각 표현(렌더러 프리팹) — 지금은 데미지 계산만 있고 시각 효과 없음.
- 거점(수비 대상) — `IDamageable`만 구현하면 `EnemyView`의 접촉 피해 판정이 자동으로 적용됨.
- `CurrentHealth <= 0` 감지 → 패배 처리는 GameManager 몫 (아직 없음).

## 7단계: MapManager

4개 매니저 중 첫 번째 실제 구현 (`Assets/Scripts/Managers/MapManager.cs`, `RCCom.Managers` 네임스페이스 신설 — Data/Core/Runtime/Effects/Definitions와 구분되는 "게임 흐름 단계 매니저" 전용 폴더).

### 설치 슬롯(그리드)과 웨이포인트(자유 좌표)를 서로 다른 데이터로 분리
- {{user}} 지침("타워 설치는 그리드, 적/플레이어 이동은 그리드 무관")을 그대로 반영: `slotTilemap`(Unity Tilemap, 셀 좌표)과 `waypoints`(Transform 배열, 자유 좌표)를 완전히 별개 필드로 둠. 웨이포인트는 스테이지당 고정이라 `Awake()`에서 `Vector2[]`로 한 번만 캐싱해 `Waypoints` 프로퍼티로 노출 — 실제 "웨이포인트를 따라 이동하는" 계산 로직은 아직 없음 (WaveManager/EnemyInstance.Tick 쪽 다음 단계 몫).
- 슬롯은 물리적으로 "칠해진 셀이냐 아니냐"만 구분하고 특정 타워 종류로 제한하지 않음 — GDD가 "경로 주변에 설치 슬롯 배치"라고만 했지 슬롯별 종류 제한을 언급하지 않았고, 종류별 개수 제한(공격3/스킬1/파워2)은 슬롯이 아니라 전역 카운터(`_attackTowerCount` 등)로 별도 관리하는 게 GDD 문구에 더 맞는 해석이라 판단.
- `IncreaseMaxSlots(kind, amount)`: 업그레이드 카드(9~11번, "OO 타워 증설")가 나중에 호출할 확장 지점만 미리 마련해둠.

### 씬 세팅 완료 + 검증 완료 (2026-07-04)
- 배경용 Tilemap(무료 에셋) + `SlotMarkers` Tilemap(논리용, 분리) + 웨이포인트 배치 완료.
- `Assets/Scripts/Debug/MapManagerDebugTester.cs`(임시 디버그용)로 검증: 클릭한 셀의 `CanBuild` 결과가 콘솔에 정상 출력, Scene 뷰에 웨이포인트 순서가 의도한 경로로 표시됨 — {{user}} 확인 완료.

### 아직 없음 (다음 단계)
- 웨이포인트를 실제로 따라 이동하는 로직 — `MapManager.Waypoints`는 데이터만 제공, 소비는 WaveManager/EnemyInstance 쪽.
- 타워 설치 UI(타입 선택 → 그리드 클릭 → `MapManager.TryGetSlotCell`+`CanBuild`+`Build` 호출) — Day2 체크리스트 항목, 아직 미착수.
- `GameManager`/`WaveManager`/`CardManager` 나머지 3개.
- 실제 Tower/Enemy 프리팹 제작 (Collider2D/Rigidbody2D 포함) — 지금까지는 스크립트 계약만 존재.

## 8단계: Day1 핵심 루프 마무리 (적 이동 / 거점 / 웨이브 스폰너 / 처치 보상)

Day1 체크리스트 중 비어있던 4개를 한 번에 처리. Tower 프리팹 조립 도중, 일정상 Day2인데 Day1 루프가 안 끝난 걸 점검하고 우선순위를 다시 여기로 돌림.

### 적 웨이포인트 추종 이동 (`EnemyRuntime.cs`)
- `EnemyInstance.Spawn(path, goal)`로 시그니처 변경 — 웨이포인트 목록과 "경로 끝에서 피해를 줄 대상(거점)"을 스폰 시점에 받는다.
- `Tick()`에서 `MoveAlongPath()` 호출: 현재 목표 웨이포인트로 `moveSpeed`만큼 이동, 도달하면 다음 웨이포인트로. 마지막 웨이포인트 도달 시 `DealContactDamageTo(goal)` 호출 후 `ReachedGoal` 이벤트 발생.
- **왜 물리 충돌이 아니라 "경로 완주"로 거점 피해를 처리했는가**: 처음엔 거점에도 Collider2D를 둬서 EnemyView의 기존 접촉 판정(플레이어와 동일한 방식)을 재사용할까 했으나, `Tick()`에서 위치를 순간 이동시키고 `ReachedGoal`로 View를 그 자리에서 파괴하면 물리 엔진이 트리거를 감지하기 전에 오브젝트가 사라져버리는 타이밍 경쟁이 생길 수 있음. 그래서 거점 피해만은 경로 완주 시점에 직접 호출하는 결정론적 방식으로 처리 — 플레이어 접촉 피해(물리 트리거, 아무 때나 어디서나 부딪힐 수 있음)와는 성격이 달라 별도 메커니즘이 타당하다고 판단.
- `Died`(처치)와 `ReachedGoal`(누수) 이벤트를 분리 — 처치 보상은 `Died`에서만 지급, 거점에 도달해 사라지는 경우는 보상 없음.
- `TakeDamage`에 `_isDead` 가드 추가 — 같은 프레임에 여러 공격원(타워+플레이어 등)이 동시에 데미지를 넣어도 `Died`가 중복 발생해 보상이 두 번 지급되는 걸 방지 (발견해서 같이 고침).

### BaseController — 거점 (`Runtime/BaseController.cs`, 신규)
- Player와 같은 이유로 그냥 MonoBehaviour (개체 하나, BaseManager 없음). `IDamageable` 구현, `maxHealth` 하나만 있는 단순 구조라 별도 Data 컨테이너 클래스로 안 뺐음 (필드 1개짜리 클래스는 과함).
- `Defeated` 이벤트만 노출 — 패배 조건 판정(거점 체력 0 또는 플레이어 체력 0)을 실제로 게임오버 처리하는 건 GameManager 몫(아직 없음).

### WaveManager (`Managers/WaveManager.cs`, 신규)
- Day1 범위: 고정 간격 순차 스폰 베타버전(예산 시스템은 Day2에 이 매니저를 확장해서 추가).
- `List<EnemyInstance>`를 직접 들고 자기 `Update()`에서 역순 순회하며 `Tick()` 호출 (Died/ReachedGoal 중 리스트에서 빠질 수 있어 역순).
- **최초 지시사항 반영**: "절차적 생성 웨이브 시스템은 난수 독립성·재현성 보장" 요구를 여기서 처음 실제로 반영 — `UnityEngine.Random` 대신 시드 고정된 전용 `System.Random`을 사용해, 다른 시스템의 Random 호출과 완전히 분리되고 같은 시드면 같은 스폰 결과가 재현되게 함.
- 처치 보상(Day1 "수치는 가짜값 OK" 허용 범위): `Died` 구독 시점에 골드/경험치 누계를 콘솔에 로그만 남김 — GameManager 도입 시 그쪽으로 이관 예정.

### EnemySpawnerDebug 갱신
- `EnemyInstance.Spawn()` 시그니처 변경에 맞춰 수정. `path`/`goal`을 `null`로 넘겨 제자리에 가만히 서있게 해서, 맵/거점 세팅 없이 적 1체만 두고 타워 효과 등을 격리 테스트할 때 계속 쓸 수 있게 함.

### 아직 없음 (다음 단계)
- 씬 배치: `BaseController` 오브젝트 생성, `WaveManager`에 `mapManager`/`baseController`/`spawnPool`(적 Definition들)/`viewPrefab`/`spawnInterval`/`randomSeed` 연결 (사용자 몫).
- 돌진형/탱커형 `EnemyDefinition` 에셋 (지금은 일반형만 있음) — `spawnPool`에 여러 종류를 넣으려면 필요.
- `GameManager`/`CardManager`, 웨이브 예산 시스템, UI — 여전히 다음 단계.

## 9단계: 파일당 클래스 1개로 전면 분리 ({{user}} 지적)

{{user}} 지적: ScriptableObject/MonoBehaviour처럼 실제 .asset/컴포넌트로 만들어지는 타입을 파일명과
다른 이름으로 한 파일에 여러 개 몰아두면, 나중에 그중 하나 이름을 바꾸는 순간 Unity가 그 클래스의
스크립트 참조(GUID+fileID)를 다시 못 찾아 이미 만든 에셋/프리팹이 "Missing Script"가 될 수 있음.

### 실제로 위험했던 파일 (SO/MonoBehaviour가 파일명과 다른 이름으로 여러 개 있었음)
- `Definitions/Tower/TowerDefinition.cs` — `TowerDefinition`(추상) + `AttackTowerDefinition`/`SkillTowerDefinition`/`PowerTowerDefinition`(전부 `[CreateAssetMenu]`로 실제 .asset이 되는 타입) 4개가 한 파일에.
- `Runtime/TowerRuntime.cs` — `TowerInstance`(MonoBehaviour, 프리팹에 붙는 실제 컴포넌트)가 파일명과 다른 이름으로 `TowerContext`와 함께 있었음.

### 위험하진 않지만 일관성 있게 같이 분리한 파일 (인터페이스/추상 클래스 — abstract라 직접 .asset이 되지 않음, 또는 MonoBehaviour/ScriptableObject가 아닌 순수 C# 필드용 데이터라 GUID+fileID 참조 자체가 없음)
- `Effects/Tower/TowerEffect.cs` → `ITowerEffect.cs` + `TowerEffectBase.cs`
- `Effects/Enemy/EnemyEffect.cs` → `IEnemyEffect.cs` + `EnemyEffectBase.cs`
- `Runtime/EnemyRuntime.cs` → `EnemyInstance.cs`(순수 C#, MonoBehaviour 아님) + `EnemyContext.cs`
- `Data/TowerData.cs` → `TowerKind.cs` + `TowerData.cs` + `AttackTowerData.cs` + `SkillTowerData.cs` + `PowerTowerData.cs`
- `Data/EnemyData.cs` → `EnemyKind.cs` + `EnemyData.cs`

### 규칙 (앞으로 계속 지킬 것)
**파일 1개 = 클래스/인터페이스/enum 1개, 파일명 = 타입명.** 특히 `ScriptableObject`/`MonoBehaviour` 상속 타입은 예외 없이 지킬 것 — 이미 만든 .asset/프리팹이 있는 상태에서 이 규칙을 어기고 나중에 이름을 바꾸면 데이터가 유실될 수 있음.

### 주의 (사용자 확인 필요할 수 있음)
혹시 이 정리 전에 `AttackTowerDefinition`/`SkillTowerDefinition`/`PowerTowerDefinition` .asset을 이미 만들어두셨다면, 파일 분리 자체만으로는 기존 .asset이 깨지지 않아야 정상이지만(같은 파일 안에서도 각 클래스는 원래 자기 이름으로 fileID가 계산되어 있었음), 혹시 Unity 콘솔에 Missing Script 경고가 뜨면 해당 에셋을 다시 만들어야 할 수 있음.

## Day1 핵심 루프 검증 완료 (2026-07-04)
{{user}} 확인: 적 스폰 → 웨이포인트 추종 이동(시작~끝) → 타워/플레이어 공격 → 피해/처치 보상까지 실제 Play 모드에서 정상 동작 확인. 원인이었던 버그(바인딩 안 된 EnemyView가 씬에 남아있던 것)도 해결.

**다음 세션 시작점**: 그리드에 타워 배치(빌드 UI) 구현 — `MapManager.TryGetSlotCell`(클릭 위치 → 셀) → `CanBuild`(종류별 슬롯 한도 확인) → `Build`(프리팹 생성) 순서로 이어붙이는 작업. 지금까지 클릭 판정 로직 자체는 `MapManagerDebugTester`로 검증 완료된 상태라, UI/입력 연결만 남음.

## 미해결/추후 확인 필요
- (현재 없음 — 새로 생기면 이 섹션에 추가)

---

## 진행 로그

### 2026-07-04 — 데이터 컨테이너 1차 작업
- `Assets/Scripts/Data/EnemyData.cs`, `TowerData.cs`, `PlayerData.cs` 작성.
- 확인 완료: 플레이어도 체력/피격 있음 (거점 체력과 별개의 값).
- 다음 단계: 사용자가 Unity 에디터에서 컴파일 에러 없는지 확인 → 이후 SO(Ability) 레이어로 진행.

### 2026-07-04 — 효과 훅 레이어 (Tower/Enemy)
- GDD 기술 노트 반영해 `ITowerEffect`/`TowerEffectBase`/`ITowerAura`, `IEnemyEffect`/`EnemyEffectBase`, `StatusType`, `IDamageable`, `TowerInstance`/`TowerContext`, `EnemyInstance`/`EnemyContext` 작성.
- 확인 완료: StatusType은 새로 정의, Enemy도 Tower와 동일 훅 패턴 적용.
- 다음 단계: Definition SO 래퍼 + 구체 효과 구현체 (사용자 확인 후 진행).
