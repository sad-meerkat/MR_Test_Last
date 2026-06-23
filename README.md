# 🕶️ Meta Quest MR Tabletop Battle (철권 스타일 1:1 대전)

<p align="center">
  <img src="https://img.shields.io/badge/Unity-6000.4.4f1__(Unity_6)-black?style=flat-square&logo=unity&logoColor=white" alt="Unity 6">
  <img src="https://img.shields.io/badge/Render_Pipeline-URP-blue?style=flat-square" alt="URP">
  <img src="https://img.shields.io/badge/Target_Hardware-Meta_Quest_3_/_Pro-black?style=flat-square&logo=meta&logoColor=white" alt="Target Hardware">
  <img src="https://img.shields.io/badge/Network-Netcode_for_GameObjects-orange?style=flat-square" alt="NGO">
</p>

> **금오공과대학교 컴퓨터공학과 'AR·VR프로그래밍-01' 과목 최종 결과물**
> Meta Quest 디바이스 환경에서 온 디바이스 환경으로 멀티플레이를 즐길 수 있는 혼합현실(MR) 대전 격투의 데모 버전입니다.
---

## 👥 Members
| 이동우 | 홍채영 | 장현서 |
| :---: | :---: | :---: |
| <img src="https://github.com/Honey0423.png" width="100" height="100" /> | <img src="https://github.com/identicon.png" width="100" height="100" /> | <img src="https://github.com/identicon.png" width="100" height="100" /> |
| **Main Programmer / Leader** | **UI/UX Designer** | **3D Asset Modeler** |
| • UGS 서버 인프라 구축<br>• NGO 1대1 동기화 로직<br>• 멀티모달 입력 모드 최적화 | • 대전 UI/UX 시스템 개발<br>• 1인칭 XR 스킬 연동<br>• 시나리오/인터랙션 기획 | • 3D 캐릭터 모델링<br>• 캐릭터 리깅 (Rigging)<br>• 음성 대사 에셋 수집 |

---

## 🛠️ 기술 스택
| Languages | Frameworks/Engines | Networking | Services |
| :---: | :---: | :---: | :---: |
| <img src="https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp&logoColor=white" /> | <img src="https://img.shields.io/badge/Unity_6-000000?style=flat-square&logo=unity&logoColor=white" /> | <img src="https://img.shields.io/badge/Netcode_NGO-orange?style=flat-square" /> | <img src="https://img.shields.io/badge/UGS_Lobby/Relay-blue?style=flat-square" /> |

## 📦 프로젝트 관리
| Collaboration Tools | Project Management Tools |
| :---: | :---: |
| <img src="https://img.shields.io/badge/Notion-000000?style=flat-square&logo=notion&logoColor=white" /> | <img src="https://img.shields.io/badge/GitHub-181717?style=flat-square&logo=github&logoColor=white" /> |

---

## 📖 Meta Quest MR Tabletop Battle란?
"Meta Quest MR Tabletop Battle Sandbox"는 Meta Quest 디바이스에서 단독(Standalone) 구동되는 차세대 혼합현실(Mixed Reality) 1:1 대전 격투 데모입니다. 사용자의 실제 방 환경을 정밀하게 매핑하여 실제 공간에 몰입감 넘치는 미니어처 배틀필드를 구현하여, 플레이어는 컨트롤러와 맨손 트래킹을 통해 원작 애니메이션 캐릭터 아바타를 직관적으로 제어할 수 있음.

---

## 🧩 핵심 구현 기능

1. 혁신적인 멀티모달 게임플레이: 왼손의 물리 컨트롤러로는 캐릭터의 이동 및 이동기(Locomotion)를 정밀하게 제어하고, 오른손은 컨트롤러를 내려놓은 맨손 상태로 핸드 트래킹 제스처를 취해 주술 및 기술을 발동하는 시스템임.

2. 철권 스타일의 대전 룰 체인: 경쟁적 몰입감을 극대화한 3판 2선승제 매치 로직을 통해 실시간 동기화되는 상단 체력바(HP)와 라운드 제한 타이머, 그리고 타격감을 높여주는 실시간 컨트롤러 진동 피드백이 플레이어에게 몰입감을 선사함.

---

## 🚀 승리 조건 (How to win?)

### 🥋 대전 플레이어 가이드
* **상대방 제압:** 대전 제한 시간이 모두 소진되기 전에, 실시간으로 동기화되는 상대방의 HP를 200에서 0으로 먼저 감소시키면 라운드에서 승리합니다.
* **스킬 제스처 영창:** 오른손으로 정확한 제스처 모양을 취하거나 명확한 가속도(Velocity) 움직임을 주입하면 원작 캐릭터의 특성에 맞는 스킬(예: 치도리, 만해 이펙트 등)이 발동되어 검 휘두르기, 회오리, 번개구, 화염구를 생성해 상대에게 치명타를 입힐 수 있음.
---

## 🎮 실행방법

### 🛠️ 유니티 에디터를 통한 구동
1. 저장소를 클론합니다: `git clone https://github.com/sad-meerkat/MR_Test_Last.git`
2. 클론된 폴더를 Unity Engine 6000.4.4f1 (Unity 6) 버전으로 오픈합니다.
3. 씬을 활성화합니다: `Assets/Scenes/핸즈 스킬 적용 2.unity`
4. `File > Build Settings` 메뉴에서 타겟 하드웨어 빌드 플랫폼을 Android로 전환합니다.
5. `Project Settings > XR Plug-in Management` 설정의 Android 탭에서 Oculus (Meta Quest) 플러그인 프로파일이 체크되어 있는지 확인합니다.
6. Meta Quest 헤드셋을 PC와 Link 또는 Type-C 케이블로 연결한 뒤 에디터 상단에서 Build and Run을 실행합니다.
---
