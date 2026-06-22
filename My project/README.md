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

### GameLogic agent

- `Start -> StageSelect -> PlayerTurn -> ResolvingCombat -> StageCleared -> StageSelect` 루프 구현
- 플레이어 턴 시작 시 카드 2장 드로우
- 손패 최대 5장 제한
- 카드 사용 시 `CardActionRecord`로 행동 기록
- 전투 해결 시 플레이어 행동 후 생존 몬스터 행동 처리
- 몬스터가 모두 사망하면 Stage Cleared 상태로 전환

## 프로젝트 구조

- `Assets/Scripts/Data`: 카드, 몬스터, 스테이지 ScriptableObject 데이터
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

## 샘플 테스트

1. `GameScene.unity`는 `SampleBattleSceneRoot`의 `startInSampleTest` 옵션이 켜져 있어 Stage 1 전투 화면으로 바로 진입한다.
2. 전투 화면에서 카드 버튼을 눌러 행동을 기록한다.
3. `Resolve Combat`를 눌러 플레이어 행동 후 몬스터 행동을 처리한다.
4. 빠른 확인이 필요하면 `Sample Turn`을 눌러 현재 손패를 모두 기록하고 전투를 즉시 해결한다.

## 데이터 갱신법

현재 샘플 데이터는 `GameAppBootstrap`에서 런타임 ScriptableObject 인스턴스로 생성된다. 실제 에셋 데이터로 전환할 때는 다음 타입의 ScriptableObject를 생성해 연결한다.

- `CardData`: 카드 이름, 효과 타입, 수치, 설명
- `MonsterData`: 몬스터 이름, 체력, 공격력
- `StageData`: 몬스터 목록, 시작 덱 목록

Unity Editor에서 `Assets/Create/Turn Card Game` 메뉴를 사용해 에셋을 만들 수 있다.

## 검증

- EditMode 테스트: `Assets/Tests/EditMode/GameSessionTests.cs`
- 주요 테스트 범위:
  - 스테이지 선택 시 2장 드로우
  - 손패 5장 제한
  - 몬스터 처치 시 Stage Cleared 전환

Unity Test Runner 또는 batchmode test 실행으로 검증한다.

```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.3.17f1\Editor\Unity.exe" -batchmode -nographics -projectPath "C:\Users\gimch\My project" -runTests -testPlatform EditMode -testResults "C:\Users\gimch\My project\Temp\editmode-results.xml" -quit
```
