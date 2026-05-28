# Speakat Server

AI 기반 영어 스피킹 학습 게임의 백엔드 서버입니다.  
플레이어는 실제 생활 시나리오(장보기, 카페 주문 등)에서 AI NPC와 영어로 대화하며, 발음·문법·맥락 적절성을 실시간으로 평가받습니다.

## 프로젝트 개요

| 항목 | 내용 |
|------|------|
| 기간 | 2025.03 ~ 2025.06 (학기 프로젝트) |
| 팀 규모 | 4인 (서버 1, 클라이언트 2, 기획 1) |
| 클라이언트 | Unity 3D 모바일 게임 |
| 서버 | ASP.NET Core (.NET 10) REST API |

## 핵심 기능

**AI 대화 및 평가 시스템**  
Claude API를 활용하여 NPC가 시나리오에 맞는 자연스러운 영어 대화를 생성하고, 플레이어의 응답을 발음 정확도, 문법, 맥락 적절성, 응답 시간 등 다차원으로 평가합니다.

**음성 인식 기반 게임플레이**  
Google STT를 통해 플레이어의 음성을 텍스트로 변환하고, 이를 AI 평가 파이프라인에 전달합니다.

**퀘스트 진행 및 학습 추적**  
시나리오별 목표(quest objectives)의 달성 여부를 세션 단위로 추적하며, 최고 기록과 누적 학습 데이터를 관리합니다.

**어휘 학습 (플래시카드)**  
게임 중 등장한 단어를 플래시카드로 저장하고, 반복 학습할 수 있는 기능을 제공합니다.

## 기술 스택

| 영역 | 기술 | 선택 근거 |
|------|------|-----------|
| Framework | ASP.NET Core (.NET 10) | 고성능 Kestrel 서버, 내장 DI 컨테이너, 강타입 시스템으로 API 안정성 확보 |
| ORM | EF Core (Pomelo MySQL Provider) | Code-First 마이그레이션으로 스키마 버전 관리, LINQ 기반 타입 안전 쿼리 |
| Database | MySQL 8.0 | 고정 스키마 기반 데이터 모델에 적합 — JSONB가 불필요한 구조에서 PostgreSQL 대신 선택 |
| Cache | Redis 7 | 세션 캐싱 및 AI 응답 캐싱을 통한 API 호출 비용 절감 |
| AI | Claude API | NPC 대화 생성 + 다차원 평가를 단일 API로 처리, 구조화된 JSON 출력 지원 |
| Voice | Google STT | 실시간 음성-텍스트 변환, 다국어 발음 평가 지원 |
| Auth | JWT + Google/Kakao OAuth | 소셜 로그인 전용 — 자체 회원가입 없이 온보딩 마찰 최소화 |
| Infra | AWS EC2, ALB, ACM, Route 53 | ALB + ACM으로 SSL 종료 처리, Let's Encrypt 대비 인증서 자동 갱신으로 운영 부담 제거 |

## 아키텍처

### Clean Architecture (4-Layer)

```
┌─────────────────────────────────────────┐
│                  Api                     │  ← Controllers, Middleware, DI 설정
├─────────────────────────────────────────┤
│              Application                 │  ← Use Cases, DTOs, Interfaces
├─────────────────────────────────────────┤
│                Domain                    │  ← Entities, Enums, Value Objects
├─────────────────────────────────────────┤
│            Infrastructure                │  ← EF Core, Redis, Claude API, Google STT
└─────────────────────────────────────────┘
```

의존성 방향은 안쪽(Domain)으로만 향합니다. Infrastructure의 외부 서비스 구현체는 Application 계층의 인터페이스에 의존하므로, AI 제공자 교체(Claude → OpenAI 등)나 DB 교체 시 Infrastructure만 수정하면 됩니다.

## 로컬 개발 환경

### 사전 요구사항

- .NET 10 SDK
- Docker & Docker Compose

### 실행

```bash
# 1. 인프라 컨테이너 실행 (MySQL, Redis)
docker compose up -d

# 2. 환경 변수 설정
cp appsettings.Development.example.json appsettings.Development.json
# Claude API Key, Google OAuth 등 설정

# 3. DB 마이그레이션
dotnet ef database update --project src/Infrastructure

# 4. 서버 실행
dotnet run --project src/Api
```

## 브랜치 및 커밋 컨벤션

**브랜치 네이밍:** `feature/#[issue번호]-설명` (예: `feature/#12-quest-api`)

**커밋 메시지:**
```
✨ feat: 퀘스트 목록 조회 API 구현 (#12)
🐛 fix: JWT 만료 시 갱신 로직 수정 (#15)
♻️ refactor: 세션 서비스 인터페이스 분리 (#18)
```

**브랜치 보호:** main 브랜치는 PR 리뷰 필수이며, CI 통과 후 머지됩니다.