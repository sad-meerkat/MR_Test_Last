🕶️ Meta Quest MR Tabletop Battle Sandbox (철권 스타일 1:1 대전)
본 프로젝트는 금오공과대학교 컴퓨터공학과 'AR·VR프로그래밍-01' 과목 최종 결과물입니다.

Meta Quest 디바이스 환경에서 단독 구동(On-Device)되는 최첨단 혼합현실(Mixed Reality) 테이블탑 대전 격투 데모입니다. 
유니티 6 엔진의 최신 XR 및 멀티플레이어 기술 스택을 활용하여 독립형 핸드/컨트롤러 동시 인식(Multimodal SHC), Client-Host Listen Server 멀티플레이 네트워크, 그리고 실시간 물리 테이블 경계 생성 시스템을 체계적으로 융합 구현하였습니다.

프로젝트 개요 (Project Overview)
과목명: AR·VR프로그래밍-01

개발 기간: 2026.05.01 ~ 2026.06.16 (총 7주, WBS 간트차트 기준 100% 완료)

개발 인원 및 역할 분담 (Team):
이동우 (팀장 / Undergraduate Researcher): 프로젝트 게임 컨셉/전체 기획 설계, UGS 서버 인프라 구축 및 Netcode 멀티플레이어 1대1 대전 시스템 동기화 로직 개발, 멀티모달(SHC) 입력 모드 최적화, 최종 빌드 및 배포
홍채영: 시나리오 및 주술·기술 상호작용 기획, uGUI World Space 대전 UI/UX(체력바, 타이머, 승패 화면) 시스템 개발, 1인칭 XR 스킬 이펙트 연동 및 버그 수정
장현서: 블리치(뱌쿠야), 나루토(사스케) 애니메이션 캐릭터 3D 모델링, 리깅(Rigging), 음성 대사 수집

🛠️ Tech Stack & Architecture (기술 스택 및 시스템 구성)
1. 기술 스택
Unity Engine: 6000.4.4f1 (Unity 6)
Render Pipeline: Universal Render Pipeline (URP - Performant Mobile XR Profile)
Target Hardware: Meta Quest 3 / Quest 3S / Quest Pro (Android standalone)
Multiplayer Network: Unity Netcode for GameObjects (NGO v1.x)
Backend Infrastructure: Unity Gaming Services (UGS - Auth, Lobby, Relay, Vivox Voice)
XR SDKs: XR Interaction Toolkit (XRI 3.3.0) / XR Hands / Meta XR SDK (OVR Integration)

2. 전체 시스템 아키텍처 (Network Architecture)
<img width="426" height="788" alt="image" src="https://github.com/user-attachments/assets/66ce714c-75ee-41ff-8c29-d8925078513e" />
본 프로젝트는 중앙 서버 비용을 최소화하고 독립형 기기의 성능을 극대화하기 위해 Client-Host(Listen Server) 구조와 UGS 클라우드 백엔드를 결합하여 설계되었습니다.

2-1. 사용 시나리오 (Use Scenario)
<img width="962" height="360" alt="image" src="https://github.com/user-attachments/assets/605fbd15-bc31-4cc0-aa39-b6018c9b199c" />

Data & Session Management:
영속성 데이터: 별도의 외부 무거운 RDB 없이 세션 기반 런타임 동기화로 구동.

세션 데이터: 실시간 방 정보 및 난입 코드, 매칭 대기열 목록은 Unity Lobby Service에서 임시 버퍼로 관리.

로컬 스토리지: 플레이어 커스텀 세팅(이름, 아바타 색상)은 클라이언트 기기 로컬에 저장되거나 소스코드 내 BindableVariable 프로퍼티 래퍼로 안전하게 캡슐화.

Core Features (핵심 구현 기능)
1.  독립형 입력 모드 전환 (Independent Hand Modality Switcher)
비대칭 개별 감지: 양손의 컨트롤러 활성 플래그와 핸드 트래킹 상태를 독립 연산합니다.
왼손으로는 물리 컨트롤러의 조이스틱을 움직여 캐릭터를 부드럽게 조작(Locomotion)하고, 
오른손은 컨트롤러를 내려놓고 맨손(Hand Tracking)으로 주술 영창 제스처 및 검 휘두르기 액션을 구사하는 혁신적인 하이브리드 비대칭 조작계를 구축했습니다.


실시간 감도 제어: 기기 노이즈 한계치 이상의 미세 움직임 변화(Delta Position Threshold)와 트래킹 플래그 정보를 실시간 비교 분석하여 프레임 저하나 오작동 없는 정밀한 입력 모드 스위칭을 제공합니다.


XRI 레이 상호작용 호환: MetaToXRIBridge를 통해 입력 좌표계를 래핑하여, 모드 전환 시 하위 UI 포인터 레이(Ray Interactor)와 그랩 인터랙터가 입력 간섭 없이 정교하게 동적 개폐됩니다.

3.  테이블탑 동적 물리 경계 (Dynamic Tabletop Physics Boundary)
자동 물리 벽 생성 (TableBoundary.cs): 가상 배틀필드로 지정된 테이블 표면 메쉬(MeshRenderer)의 외곽 Bounds 데이터 및 로컬 스케일을 감지하여 테이블 사면에 완벽히 일치하는 투명 물리 벽(BoxCollider)을 실시간으로 자동 연산/배치합니다.
인터랙티브 오브젝트 낙하 방지: 물리 중력과 격투 타격 반동에 의해 테이블 위의 미니어처 가상 캐릭터나 상호작용 소품들이 테이블 아래 바닥으로 떨어져 소실되는 버그를 하드웨어 수준에서 완벽하게 차단합니다.

4.  가상 터치 인터랙션 및 철권 룰 체인 (Battle System & UI)
직관적 물리 버튼 (VRButtonTouch.cs): 핸드 트래킹 메쉬나 컨트롤러 콜라이더가 가상 버튼 표면에 접촉했을 때 트리거 이벤트를 감지하여, 로비 매칭 및 캐릭터 선택(뱌쿠야 / 사스케) 시퀀스를 매끄럽게 연결합니다.

철권 스타일 대전 UI: 월드 스페이스(World Space) 3D UI 기반으로 상단에 플레이어 1, 2의 실시간 동기화 체력바(HP Bar)와 타이머가 배치됩니다. 타이머가 만료되거나 한쪽 HP가 0이 되면 게임이 즉시 종료되며, 판정 후 재경기(R u ready?) 루프로 안전하게 진입합니다.

<img width="724" height="294" alt="image" src="https://github.com/user-attachments/assets/17e9e782-edfc-4ed9-ad21-21f0975698da" />
