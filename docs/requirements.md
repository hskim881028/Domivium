# Domivium Requirements (Template)

Note (Early-Stage Rapid Mode): For now, prioritize fast answers and minimal iterations. Build/CI/test verification may be deferred as per docs/plan.md section "0a) Early-Stage Working Mode: Rapid Answers".

Fill in the sections below with as much detail as possible. Use bullet points for lists, and be explicit about hard constraints (must/shall) vs. preferences (should/nice-to-have).

## 1. Project Overview
- Unity 6.1 버전을 사용해서 게임 개발.
- pc, iOS, android, 닌텐도 스위치에 출시예정.
- DB는 PostgreSQL 사용
- 서버는 MagicOnion패키지를 활용해서 구성
- 클라이언트는 VContainer패키지를 활용해서 구조 구성
- UI는 MVP 구조에 VM을 섞어서 구성.
- 전투 스테이지는 x,z축으로 tilemap으로 바닥을 구성
- 전투 스테이지에 나오는 GameObject는 Monobehaviour를 상속받은 Actor를 상속 받는다.
- character, monster는 unit을 상속.
- character, monster는 x,y축 사용한 Plane mesh로 구성. 
- 게임 전투는 Unreal Engine에 있는 Gameplay Ability System와 동일하게 구성.

## 2. Goals (What success looks like)
- G1: 객체 지향 설계의 5원칙 지켜줘. (Single Responsibility Principle, Open Closed Priciple, Listov Substitution Priciple, Interface Segregation Principle, Dependency Inversion Principle)
- G2: 변수나 함수명은 게임 개발에 많이 사용하는 용어 선택.

## 3. Non-Goals (Out of scope)
<!-- - NG1: … -->
<!-- - NG2: … -->

## 4. Functional Requirements
Describe the core features and behaviors.
- FR1: 프로젝트는 Domivium.Clent, Domivium.Shared, Domivium.Server 3개로 구성.
- FR2: 기능에 맞게 asmdef로 솔루션 분리.

## 5. Non-Functional Requirements / Constraints
Hard constraints, quality attributes, and cross-cutting concerns.
- Platforms and runtimes (Unity version, .NET versions, target devices): …
- Performance (fps, memory limits, latency budgets): …
- Reliability/Availability: …
- Security/Privacy: …
- Localization/Internationalization: …
- Accessibility: …
- Offline/Online behavior: …
- Compliance/Legal: …

## 6. Architecture & Integration
- High-level architecture (Client/Server/Shared boundaries): …
- Networking/Protocols (e.g., gRPC/MagicOnion, MessagePack): …
- Data models & serialization: …
- Third-party packages/services and version constraints: …

## 7. UI/UX Requirements
- UI는 MVP 구조에 VM을 섞어서 구성.
- UI Navigation을 활용해서 UI 관리
- 사용자는 UI Manager를 활용해서 UI 사용
- 유니티 NewInputsystem 사용


## 8. References
- https://dev.epicgames.com/documentation/en-us/unreal-engine/gameplay-ability-system-for-unreal-engine?application_version=5.6
- https://github.com/GlitchEnzo/NuGetForUnity
- https://github.com/hadashiA/VContainer
- https://github.com/Cysharp/UniTask
- https://github.com/Cysharp/R3
- https://github.com/Cysharp/ZString
- https://github.com/Cysharp/MessagePipe
