# AGENTS.md

## Project
Unity를 사용하여 Turn 개념이 존재하는 싱글게임을 하나 만들어본다.
이 게임을 출시를 목표로 작업할 내용이며 비슷한 게임으로는 Slay the Spire, 유희왕 마스터 듀얼등이 있다.

## Product goal
Turn 마다 카드를 사용하여 목표 몬스터를 제거 하여 정해진 Stage를 클리어하는 게임이다.

## GameLoop
1. 플레이어의 차례가되면 카드를 2장 드로우한다.
2. 플레이어는 카드를 최대 5장까지 소유가 가능하며 만약 5장 이상으로 보유할경우 마지막으로 얻은 카드부터 5장이 될때까지 사라진다.
3. 플레이어가 카드를 사용하면 플레이어 행동에 사용한 카드의 정보가 기록된다.
4. 플레이어가 카드를 사용하여 모든 정보를 기록했다면 UI를 통해 전투를 결정한다.
5. 전투가 진행되면서 플레이어 행동 -> 몬스터 행동 형태로 진행한다.
6. 위 과정을 몬스터가 모두 죽어 Stage가 클리어 된다면 종료 UI와 함께 Lobby로 복귀한다.

## Tech rules
- 작은 단위로 나누어 구현한다.
- 각 단계 후 실행 가능한 상태를 유지한다.
- 변경 후 lint, test, build를 우선 실행한다.
- 스크립트로 검증 가능한 항목은 반드시 스크립트로 검증한다.
- 데이터는 Unity Scriptable Object 형태로 작업한다.
- UI 공용 컴포넌트는 Asset/Scripts/UI/components 아래에 둔다.

## Commands
- install: npm install
- dev: npm run dev
- lint: npm run lint
- test: npm run test
- build: npm run build

## Data quality rules
- 날짜는 YYYY-MM-DD 형식으로 정규화한다.
- provider + title + end_date 조합이 같으면 중복 후보로 본다.
- url, provider, title, end_date 중 하나라도 비면 invalid 로 처리한다.
- end_date가 오늘보다 이전이면 expired=true로 표시한다.

## Review expectations
- 새 소스를 추가했으면 추가한 작업에대해 설명을 PDF형태로 반환하여 작성한다.
- UI 변경 시 16 : 9 사이즈에서 에서 가시성을 확인해둔다.
- README에 실행법과 데이터 갱신법을 반영한다.

## Done when
- 최소 3개 데이터 소스가 연결되어 있다.
- 검색, 필터, 정렬이 동작한다.
- 마감 숨김이 동작한다.
- 중복 제거가 동작한다.
- 링크 유효성 검사가 통과한다.
- README에 로컬 실행법이 있다.