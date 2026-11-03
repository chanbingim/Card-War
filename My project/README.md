# Turn Card Game Prototype

Unity로 제작 중인 턴 기반 싱글 카드 전투 게임 프로토타입이다. 플레이어는 매 턴 카드를 2장 드로우하고, 손패에서 사용할 카드를 기록한 뒤 전투 해결 버튼으로 플레이어 행동과 몬스터 행동을 순서대로 처리한다.

## 현재 구현

### Frontend agent

- 런타임 자동 생성 시작 UI
- 스테이지 선택 UI
- 전투 UI
  - 플레이어 HP와 손패 수 표시
  - 몬스터 HP 표시
  - 소유 카드 목록 표시
  - 카드 사용 기록 표시
  - 전투 해결 버튼
- 스테이지 클리어 후 Lobby 역할의 Stage Select 화면으로 복귀
- GameClientManager가 카드 이동을 요청하고 Collection/Deck UI 이벤트로 두 Transform을 받은 뒤 DeckCardTransferAnimation 이벤트를 발행하는 카드 이동 구조

### GameLogic agent

- `Start -> StageSelect -> PlayerTurn -> ResolvingCombat -> StageCleared -> StageSelect` 루프 구현
- 플레이어 턴 시작 시 카드 2장 드로우
- 손패 최대 5장 제한
- 카드 사용 시 `CardActionRecord`로 행동 기록
- 전투 해결 시 플레이어 행동 후 생존 몬스터 행동 처리
- 몬스터가 모두 사망하면 Stage Cleared 상태로 전환

## 프로젝트 구조

- `Assets/Scripts/Data`: 카드, 몬스터, 스테이지 ScriptableObject 데이터
- `Assets/Scripts/Data/AI/CardUsePatternData.cs`: Stage AI Card Use XML 직렬화 struct와 대상 선택 enum
- `Assets/Scripts/Utility/XMLReader/XmlDataLoader.cs`: struct XML 데이터를 읽는 Resources 지원 제네릭 로더
- `Assets/Scripts/Data/SKillData/SkillInfo.cs`: Action CSV의 공격/스킬 정보 모델
- `Assets/Resources/Data/Xml`: Stage별 AI Card Use 턴 패턴 XML
- `Assets/Scripts/Game`: 턴/전투 상태 머신과 행동 기록 모델
- `Assets/Scripts/UI/Components`: 공용 UI 컴포넌트
- `Assets/Scripts/UI/Screens`: 런타임 UI 부트스트랩 및 화면 구성
- `Assets/Tests/EditMode`: GameSession EditMode 테스트
- `output/pdf`: 작업 설명 PDF 산출물

## 로컬 실행법

1. Unity Hub에서 Unity `6000.3.17f1`로 이 프로젝트를 연다.
2. `Assets/Scenes/GameScene.unity`를 연다.
3. Hierarchy에서 `SampleBattleSceneRoot` 오브젝트가 있는지 확인한다.
4. Play 버튼을 누르면 샘플 전투 화면으로 바로 진입한다.

### GameMenu 파티 편성 UI

1. `Assets/Scenes/GameMenu.unity`를 연다.
2. 파티 편성 화면은 16:9 기준이며 캐릭터 슬롯을 누르면 교체 모달이 열린다.
3. 상단 중앙의 A/B 테마 버튼으로 보랏빛 성역과 청록빛 월야 변형을 비교한다.
4. 우하단 `편성 완료`를 누르면 저장 완료 피드백이 상단 안내 문구에 표시된다.
5. 레이아웃을 다시 생성하려면 Unity 메뉴 `Tools > Haness > Build Character Party UI`를 실행한다.
6. 캐릭터 정보는 이름, 역할/포지션, `캐릭터 변경` 안내만 표시하며 레벨과 별 등급은 파티 편성 화면에서 숨긴다.

## 샘플 테스트

1. `GameScene.unity`는 `SampleBattleSceneRoot`의 `startInSampleTest` 옵션이 켜져 있어 Stage 1 전투 화면으로 바로 진입한다.
2. 전투 화면에서 카드 버튼을 눌러 행동을 기록한다.
3. `Resolve Combat`를 눌러 플레이어 행동 후 몬스터 행동을 처리한다.
4. 빠른 확인이 필요하면 `Sample Turn`을 눌러 현재 손패를 모두 기록하고 전투를 즉시 해결한다.

### 덱 카드 이동 애니메이션

1. 정상 부트스트랩 경로를 통해 `DeckBuildScene`으로 진입한다.
2. Deck Collection의 카드를 클릭한다.
3. Collection 카드가 사라지며 임시 카드 이미지가 실제 DeckViewEntry 위치로 이동하고, 완료 시 DeckViewEntry가 표시되는지 확인한다.
4. Game View `1600 x 900` 해상도에서 카드 컬렉션과 덱 목록이 화면 안에 표시되는지 확인한다.

### Screen Flash Additive 연출

1. Canvas 아래에 화면 전체를 덮는 UI Image를 만들고 `screen-flash-impact-gold` Sprite를 지정한다.
2. Image의 Material에 `Assets/Matrial/ScreenFlashAdditive.mat`을 지정한다.
3. `ScreenSplash`의 `flashImage`에 해당 Image를 연결한다.
4. 셰이더는 `Blend SrcAlpha One`을 사용하므로 `ScreenSplash`가 변경하는 Image 알파가 실제 발광 강도로 반영된다.
5. Image의 Raycast Target은 꺼서 입력을 가로막지 않도록 한다.

### 가챠 1회 / 10회 뽑기

1. 정상 부트스트랩 경로로 `GachaScene`에 진입한다.
2. 화면 하단의 `1회 뽑기` 또는 `10회 뽑기` 버튼을 선택한다.
3. 1회 뽑기는 1,000 Gold, 10회 뽑기는 10,000 Gold를 사용한다.
4. 연출이 끝나면 1회 결과는 `SingleResultPivot` 중앙에 한 장, 10회 결과는 `TenResultPivot` 기준 5 x 2 형태로 배치된다.
5. 첫 확인 버튼은 모든 카드 공개, 두 번째 확인 버튼은 결과 화면 종료로 동작한다.
6. Game View `1600 x 900`에서 두 버튼과 10장 결과가 화면 안에 표시되는지 확인한다.
7. 버튼, 결과 피벗, 카드 10장과 원근 카메라 설정은 `GachaScene.unity`에 저장되어 있으므로 Play 전에도 Hierarchy와 Game View에서 확인할 수 있다.
8. 뽑기 버튼 영역은 `World Space Canvas`를 사용하며 Main Camera는 Perspective/FOV 55, 위치 `(0, 1.2, -10)`, X축 7도로 설정되어 있다. 1회/10회 버튼은 서로 다른 Z 위치를 사용해 크기와 원근 차이를 만든다.

## 데이터 갱신법

현재 샘플 데이터는 `GameAppBootstrap`에서 런타임 ScriptableObject 인스턴스로 생성된다. 실제 에셋 데이터로 전환할 때는 다음 타입의 ScriptableObject를 생성해 연결한다.

- `CardData`: 카드 이름, 효과 타입, 수치, 설명
- `MonsterData`: 몬스터 이름, 체력, 공격력
- `StageData`: 몬스터 목록, 시작 덱 목록

Unity Editor에서 `Assets/Create/Turn Card Game` 메뉴를 사용해 에셋을 만들 수 있다.

공격/스킬 데이터는 `Assets/Resources/Data/Csv/Action.csv`에서 갱신한다. CSV 헤더는
`SkillInfo`의 public property 이름과 대소문자까지 일치해야 하며, `ActionType`은
`NormalAttack` 또는 `Skill`, `TargetType`은 `EnemySelf` 또는 `EnemyAll`을 사용한다.

### Stage AI Card Use 패턴 갱신

Stage 1의 10턴 AI 카드 사용 패턴은 `Assets/Resources/Data/Xml/Stage01_CardUsePattern.xml`에서 갱신한다.

- `CardUseTurnData number`: Queue에 넣을 턴 순서이며 1부터 연속되어야 한다.
- 같은 턴의 `CardUseActionData`는 `CardUses`에 작성된 순서대로 실행한다.
- `cardId`: `Cards_ko.csv`에 존재하는 카드 ID를 사용한다.
- `targetPolicy`: 현재 패턴에서는 `Self`와 `LowestHpEnemy`를 사용한다.
- 카드 이름, 설명, 효과 수치는 `cardId`로 `Cards_ko.csv`를 조회해 사용한다.

XML 파일은 Unity에서 `Resources.Load<TextAsset>("Data/Xml/Stage01_CardUsePattern")`로 불러올 수 있다.
`XmlSerializer(typeof(CardUsePatternData))`로 역직렬화하면 `CardUsePatternData.Turns`를 턴 Queue의 원본 데이터로 사용할 수 있다.

제네릭 XML 로더를 사용하면 Resources 로드와 역직렬화를 한 번에 처리할 수 있다.

```csharp
using TurnCardGame.Data.AI;
using TurnCardGame.Serialization;

CardUsePatternData pattern =
    XmlDataLoader<CardUsePatternData>.LoadFromResources("Data/Xml/Stage01_CardUsePattern");
```

잘못된 XML이나 누락된 Resources 파일을 예외 없이 처리해야 하는 초기화 코드에서는
`TryLoadFromResources(path, out data, out error)`를 사용한다. 로더는 DTD와 외부 XML 엔티티를 허용하지 않는다.

## 검증

### 파티 편성 플레이트 확인

1. `GameMenu` 씬을 열고 `Tools > Haness > Build Character Party UI`를 실행한다.
2. Game View를 `1600 x 900`으로 맞춘 뒤 Play한다.
3. 캐릭터 슬롯을 누르고 다른 캐릭터를 선택해 이름, 특성, 초상 이니셜이 함께 바뀌는지 확인한다.
4. 화면 상단 중앙의 A/B 버튼을 눌러 각진 금색 플레이트와 부드러운 청록 플레이트가 배경·강조색과 함께 전환되는지 확인한다.
5. 이름과 특성은 저상형 플레이트 안에, `캐릭터 변경`은 분리된 하단 칩에 표시되며 Lv/별 설명은 표시하지 않는다.
6. `편성 완료`를 눌러 상단 안내 문구가 저장 완료 상태로 바뀌는지 확인한다.

- EditMode 테스트: `Assets/Tests/EditMode/GameSessionTests.cs`
- 주요 테스트 범위:
  - 스테이지 선택 시 2장 드로우
  - 손패 5장 제한
  - 몬스터 처치 시 Stage Cleared 전환

Unity Test Runner 또는 batchmode test 실행으로 검증한다.

```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.3.17f1\Editor\Unity.exe" -batchmode -nographics -projectPath "C:\Git\GitHub\HanessTest\My project" -runTests -testPlatform EditMode -testResults "C:\Git\GitHub\HanessTest\My project\Temp\editmode-results.xml" -quit
```
