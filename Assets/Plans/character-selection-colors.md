# 캐릭터 선택 색상 시각화 구현 계획

플레이어 1(P1)과 플레이어 2(P2)가 캐릭터를 선택할 때 서로 다른 색상의 외곽선(Outline)을 표시하고, 두 플레이어가 같은 캐릭터를 선택했을 경우 제3의 색상으로 표시되도록 수정합니다.

## Project Overview
- **Game Title:** MR Tabletop Game (Fighting Game Mode)
- **High-Level Concept:** 테이블탑 환경에서 진행되는 1대1 대전 게임
- **Players:** 멀티플레이어 (호스트/클라이언트)
- **Render Pipeline:** URP
- **UI System:** uGUI (Canvas 기반)

## Game Mechanics
- **Core Gameplay Loop:** 캐릭터 선택 -> 대전 진행 -> 결과 확인
- **Selection Visuals:** 캐릭터 선택 창에서 각 플레이어의 선택 상태를 외곽선 색상으로 명확히 구분하여 인지성을 높임.

## UI
- **Character Selection UI:** 5개의 슬롯(Grid 자식)으로 구성됨.
- **Outline:** 각 슬롯 하위에 `Outline` 게임 오브젝트가 존재하며, `Image` 컴포넌트를 통해 색상을 표현함.

## Key Asset & Context
- **Script:** `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FightingGameManager.cs`
- **Fields to Add:**
  - `m_P1SelectColor`: P1 선택 색상 (기본값: Blue)
  - `m_P2SelectColor`: P2 선택 색상 (기본값: Red)
  - `m_BothSelectColor`: 양쪽 선택 색상 (기본값: Magenta)

## Implementation Steps

### 1. FightingGameManager.cs 필드 추가 (developer)
- P1, P2, 그리고 동시 선택 시 사용할 색상 변수를 `[SerializeField]`로 추가합니다.
- **Dependency:** None

### 2. UpdateSelectionVisuals 메서드 로직 수정 (developer)
- 슬롯 순회 시 P1과 P2의 선택 여부(`m_P1Choice`, `m_P2Choice`)와 준비 상태(`m_P1Ready`, `m_P2Ready`)를 체크합니다.
- `Outline` 오브젝트의 `Image` 컴포넌트를 가져와 조건에 맞는 색상을 할당합니다.
  - P1만 선택: `m_P1SelectColor`
  - P2만 선택: `m_P2SelectColor`
  - 둘 다 선택: `m_BothSelectColor`
  - 선택 없음: `SetActive(false)`
- **Dependency:** Step 1

### 3. 검증 및 테스트 (explorer/developer)
- 멀티플레이어 환경에서 두 플레이어가 서로 다른 캐릭터를 고를 때 색상이 구분되는지 확인합니다.
- 동일한 캐릭터를 고를 때 색상이 변하는지 확인합니다.
- **Dependency:** Step 2

## Verification & Testing
- **Unit Test:** `UpdateSelectionVisuals` 호출 시 `Image.color`가 기대한 값과 일치하는지 확인.
- **Manual Check:** Unity Editor 상에서 `m_P1Choice` 등의 값을 수동으로 변경하여 UI 업데이트 확인.
- **Edge Case:** 한 명만 선택했다가 취소하거나 변경할 때 색상이 즉각 반영되는지 확인.
