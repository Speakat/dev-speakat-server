# Speakat API 명세서

> **Version:** v1.0  
> **Base URL:** `/api/v1`  
> **Last Updated:** 2026-04-29

---

## 목차

1. [개요](#1-개요)
2. [인증 방식](#2-인증-방식)
3. [공통 응답 형식](#3-공통-응답-형식)
4. [에러 코드 정의](#4-에러-코드-정의)
5. [API 엔드포인트](#5-api-엔드포인트)
   - 5.1 [Auth (인증)](#51-auth-인증)
   - 5.2 [Stages (스테이지)](#52-stages-스테이지)
   - 5.3 [Quests (퀘스트)](#53-quests-퀘스트)
   - 5.4 [Gameplay Sessions (게임 플레이)](#54-gameplay-sessions-게임-플레이)
   - 5.5 [Flashcards (플래시카드 / 단어 학습)](#55-flashcards-플래시카드--단어-학습)
   - 5.6 [User / My Page (마이페이지)](#56-user--my-page-마이페이지)

---

## 1. 개요

Speakat은 Unity 기반 영어 어휘 학습 게임의 백엔드 API입니다. 사용자는 스테이지와 퀘스트를 통해 영어 단어를 학습하며, AI 대화와 음성 인식을 활용한 게임 플레이 세션을 진행합니다.

**기술 스택:**
- ASP.NET Core (Kestrel)
- MySQL (Pomelo EF Core)
- Redis (세션/캐시)
- S3 (세션 대화 아카이빙, 프로필 이미지)
- JWT 인증

---

## 2. 인증 방식

### 2.1 JWT (JSON Web Token)

모든 인증이 필요한 요청에는 `Authorization` 헤더에 Bearer Token을 포함합니다.

```
Authorization: Bearer <access_token>
```

### 2.2 토큰 구성

| 토큰 | 용도 | 만료 시간 |
|------|------|-----------|
| Access Token | API 요청 인증 | 30분 |
| Refresh Token | Access Token 재발급 | 14일 |

### 2.3 토큰 Payload 구조

```json
{
  "sub": "550e8400-e29b-41d4-a716-446655440000",
  "email": "user@example.com",
  "nickname": "학습자",
  "iat": 1712448000,
  "exp": 1712449800
}
```

### 2.4 인증 흐름 (소셜 로그인)

Speakat은 자체 이메일 회원가입 없이 **Google / Kakao OAuth 소셜 로그인만** 지원합니다.

1. Unity 클라이언트에서 OAuth Provider(Google/Kakao) WebView 로그인
2. Authorization Code를 서버에 전달 (`POST /auth/oauth/{provider}`)
3. 서버가 Provider에서 사용자 정보 조회 후 Access Token + Refresh Token 발급
4. 신규 사용자(`isNewUser: true`)인 경우 클라이언트에서 닉네임 설정 온보딩 진행
5. API 요청 시 Access Token을 헤더에 포함
6. Access Token 만료 시 Refresh Token으로 재발급 (`POST /auth/refresh`)
7. Refresh Token 만료 시 재로그인 필요

> **참고:** `🔒` 표시가 있는 엔드포인트는 인증이 필요합니다. 모든 🔒 엔드포인트에 `UNAUTHORIZED`, `ACCESS_TOKEN_EXPIRED`, `INVALID_TOKEN`이 공통 적용되므로 각 에러 섹션에는 도메인별 에러만 명시합니다.

---

## 3. 공통 응답 형식

### 3.1 성공 응답

```json
{
  "isSuccess": true,
  "data": { ... },
  "message": null
}
```

### 3.2 에러 응답

```json
{
  "isSuccess": false,
  "data": null,
  "error": {
    "code": "ACCESS_TOKEN_EXPIRED",
    "message": "Access Token이 만료되었습니다.",
    "details": null
  }
}
```

### 3.3 페이지네이션 응답

```json
{
  "isSuccess": true,
  "data": {
    "items": [ ... ],
    "pagination": {
      "page": 1,
      "size": 20,
      "totalItems": 58,
      "totalPages": 3
    }
  }
}
```

---

## 4. 에러 코드 정의

### 4.1 공통 에러

| 코드 | HTTP Status | 설명 |
|------|-------------|------|
| `INVALID_REQUEST` | 400 | 잘못된 요청 형식 (Validation 실패) |
| `NOT_FOUND` | 404 | 요청한 리소스를 찾을 수 없음 (catch-all) |
| `INTERNAL_SERVER_ERROR` | 500 | 서버 내부 오류 |
| `RATE_LIMIT_EXCEEDED` | 429 | 요청 횟수 초과 (Rate Limit) |

### 4.2 인증 에러

| 코드 | HTTP Status | 설명 |
|------|-------------|------|
| `UNAUTHORIZED` | 401 | 인증 헤더 없음 (토큰 미제공) |
| `ACCESS_TOKEN_EXPIRED` | 401 | Access Token 만료 |
| `INVALID_TOKEN` | 401 | 유효하지 않은 토큰 |
| `REFRESH_TOKEN_EXPIRED` | 401 | Refresh Token 만료 |
| `OAUTH_AUTH_FAILED` | 401 | OAuth 인증 실패 (Provider 토큰 검증 실패) |
| `UNSUPPORTED_OAUTH_PROVIDER` | 400 | 지원하지 않는 OAuth Provider |
| `ACCOUNT_DISABLED` | 403 | 비활성화된 계정 |
| `DUPLICATE_NICKNAME` | 409 | 이미 존재하는 닉네임 |

### 4.3 게임 플레이 에러

| 코드 | HTTP Status | 설명 |
|------|-------------|------|
| `SESSION_NOT_FOUND` | 404 | 존재하지 않는 세션 |
| `SESSION_ALREADY_IN_PROGRESS` | 409 | 이미 진행 중인 세션 존재 |
| `SESSION_ALREADY_ENDED` | 400 | 세션이 이미 종료됨 |
| `AI_SERVICE_ERROR` | 502 | AI 서비스 호출 실패 (STT 포함) |
| `INVALID_AUDIO` | 400 | 잘못된 음성 데이터 형식 |

### 4.4 스테이지/퀘스트 에러

| 코드 | HTTP Status | 설명 |
|------|-------------|------|
| `STAGE_NOT_FOUND` | 404 | 존재하지 않는 스테이지 |
| `STAGE_LOCKED` | 403 | 잠긴 스테이지 (이전 스테이지 미완료) |
| `QUEST_NOT_FOUND` | 404 | 존재하지 않는 퀘스트 |
| `QUEST_LOCKED` | 403 | 잠긴 퀘스트 (선행 조건 미충족) |

### 4.5 단어 학습 에러

| 코드 | HTTP Status | 설명 |
|------|-------------|------|
| `FLASHCARD_NOT_FOUND` | 404 | 존재하지 않는 플래시카드 |

---

## 5. API 엔드포인트

---

### 5.1 Auth (인증)

#### `POST /auth/oauth/{provider}`- Google, Kakao 소셜 로그인 / 회원가입

소셜 로그인과 회원가입을 하나의 엔드포인트로 처리합니다. 기존 사용자는 로그인, 신규 사용자는 자동 회원가입 후 로그인됩니다.

**Path Parameter:**

| 파라미터 | 타입 | 설명 |
|----------|------|------|
| provider | string | OAuth 제공자 (`google`, `kakao`) |

**Request Body:**

```json
{
  "authorizationCode": "4/0AX4XfWh..."
}
```

| 필드 | 타입 | 필수 | 설명 |
|------|------|------|------|
| authorizationCode | string | ✅ | OAuth Provider에서 발급받은 인가 코드 |

**Response (200 OK):**

```json
{
  "isSuccess": true,
  "data": {
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "email": "user@gmail.com",
    "nickname": "GoogleUser",
    "profileImageUrl": "https://lh3.googleusercontent.com/...",
    "provider": "GOOGLE",
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "dGhpcyBpcyBhIHJlZnJl...",
    "isNewUser": true
  }
}
```

| 필드 | 타입 | 설명 |
|------|------|------|
| provider | enum | `GOOGLE`, `KAKAO` |
| isNewUser | boolean | `true`면 신규 가입 → 클라이언트에서 추가 정보 입력 진행 |

**에러:**
- `OAUTH_AUTH_FAILED`: OAuth 인증 실패
- `UNSUPPORTED_OAUTH_PROVIDER`: 지원하지 않는 OAuth Provider

---

#### `POST /auth/refresh`- 토큰 재발급

**Request Body:**

```json
{
  "refreshToken": "dGhpcyBpcyBhIHJlZnJl..."
}
```

**Response (200 OK):**

```json
{
  "isSuccess": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "bmV3IHJlZnJlc2ggdG9r..."
  }
}
```

**에러:**
- `REFRESH_TOKEN_EXPIRED`: Refresh Token 만료
- `INVALID_TOKEN`: 유효하지 않은 Refresh Token

---

#### `POST /auth/logout` 🔒- 로그아웃

**Request Body:**

```json
{
  "refreshToken": "dGhpcyBpcyBhIHJlZnJl..."
}
```

**Response (204 No Content)**

> Refresh Token을 Redis 블랙리스트에 추가하여 무효화합니다.

---

#### `POST /auth/check-nickname`- 닉네임 중복 확인

**Request Body:**

```json
{
  "nickname": "학습자"
}
```

**Response (200 OK):**

```json
{
  "isSuccess": true,
  "data": {
    "available": false,
    "suggestion": "학습자_42"
  }
}
```

**에러:**
- `INVALID_REQUEST`: 닉네임 형식 오류

---

### 5.2 Stages (스테이지)

#### `GET /stages` 🔒- 스테이지 목록 조회

**Response (200 OK):**

```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "stageId": 1,
        "title": "카페에서 주문하기",
        "description": "카페에서 음료를 주문하는 상황을 연습합니다.",
        "status": "COMPLETED",
        "questCount": 3,
        "completedQuestCount": 3
      },
      {
        "stageId": 2,
        "title": "공항 체크인",
        "description": "공항에서 체크인하는 상황을 연습합니다.",
        "status": "IN_PROGRESS",
        "questCount": 4,
        "completedQuestCount": 1
      },
      {
        "stageId": 3,
        "title": "비즈니스 미팅",
        "description": "비즈니스 미팅에서 의견을 교환하는 상황입니다.",
        "status": "LOCKED",
        "questCount": 5,
        "completedQuestCount": 0
      }
    ]
  }
}
```

| 필드 | 타입 | 설명 |
|------|------|------|
| status | enum | 사용자의 스테이지 진행 상태 (`LOCKED`, `IN_PROGRESS`, `COMPLETED`) - `user_stages.status` |
| questCount | int | 해당 스테이지에 포함된 전체 퀘스트 수 |
| completedQuestCount| int | 사용자가 완료한 퀘스트 수 |

---

#### `GET /stages/{stageId}` 🔒- 스테이지 상세 조회

특정 스테이지의 상세 정보와 포함된 퀘스트 목록을 조회합니다.

**Path Parameters:**

| 파라미터 | 타입 | 필수 | 설명 |
|----------|------|------|--------|
| stageId | long | ✅ | 조회할 스테이지의 고유 ID |

**Response (200 OK):**

```json
{
  "isSuccess": true,
  "data": {
    "stageId": 1,
    "title": "카페에서 주문하기",
    "description": "카페에서 음료를 주문하는 상황을 연습합니다.",
    "status": "IN_PROGRESS",
    "quests": [
      {
        "questId": 1,
        "title": "인사하고 메뉴판 받기",
        "description": "점원에게 인사하고 메뉴판을 요청하세요.",
        "thumbnailUrl": "https://cdn.speakat.com/quests/quest1-thumb.png",
        "sortOrder": 1,
        "status": "COMPLETED",
        "attemptCount": 2
      },
      {
        "questId": 2,
        "title": "음료 주문하기",
        "description": "원하는 음료와 수량을 말하세요.",
        "thumbnailUrl": "https://cdn.speakat.com/quests/quest2-thumb.png",
        "sortOrder": 2,
        "status": "IN_PROGRESS",
        "attemptCount": 1
      },
      {
        "questId": 3,
        "title": "결제 및 인사",
        "description": "결제 수단을 선택하고 작별 인사를 하세요.",
        "thumbnailUrl": "https://cdn.speakat.com/quests/quest3-thumb.png",
        "sortOrder": 3,
        "status": "LOCKED",
        "attemptCount": 0
      }
    ]
  }
}
```

| 필드 | 타입 | 설명 |
|------|------|------|
| status | enum | 사용자의 스테이지 진행 상태 (LOCKED, IN_PROGRESS, COMPLETED) |
| sortOrder | int | 퀘스트가 진행되는 순서 |
| status | enum | 퀘스트 진행 상태 (`LOCKED`, `IN_PROGRESS`, `COMPLETED`) |
| attemptCount | int | 유저가 해당 퀘스트에 도전한 총 횟수 |

**에러:**
- `STAGE_NOT_FOUND`: 존재하지 않는 스테이지 ID 요청 시

---

### 5.3 Quests (퀘스트)

#### `GET /quests/{questId}` 🔒- 퀘스트 상세 조회

**Response (200 OK):**

```json
{
  "isSuccess": true,
  "data": {
    "questId": 2,
    "stageId": 1,
    "title": "커스텀 주문",
    "description": "자신만의 커스텀 음료를 주문해보세요.",
    "thumbnailUrl": "https://cdn.speakat.com/quests/quest2-thumb.png",
    "objectives": [
      "음료 사이즈를 선택하세요",
      "커스텀 옵션을 2가지 이상 요청하세요",
      "최종 주문을 확인하세요"
    ],
    "status": "IN_PROGRESS",
    "bestScore": null,
    "attemptCount": 0
  }
}
```
| 필드 | 타입 | 설명                                                                           |
|------|------|------------------------------------------------------------------------------|
| status | enum | 사용자의 퀘스트 진행 상태 (`LOCKED`, `IN_PROGRESS`, `COMPLETED`) |

**에러:**
- `QUEST_NOT_FOUND`: 존재하지 않는 퀘스트

---

### 5.4 Gameplay Sessions (게임 플레이)

#### `POST /sessions` 🔒- 게임 세션 시작

**Request Body:**

```json
{
  "questId": 1
}
```

**Response (201 Created):**

```json
{
  "isSuccess": true,
  "data": {
    "sessionId": "a1b2c3d4-5678-90ab-cdef-1234567890ab",
    "npcDialogue": "Hi there! Welcome to Bean & Brew. What can I get started for you today?"
  }
}
```

**에러:**
- `QUEST_NOT_FOUND`: 존재하지 않는 퀘스트
- `QUEST_LOCKED`: 잠긴 퀘스트
- `SESSION_ALREADY_IN_PROGRESS`: 이미 진행 중인 세션 존재

---

#### `POST /sessions/{sessionId}/speech` 🔒- 음성 입력 및 AI 응답

**Request Body:**

```json
{
  "questId": 1,
  "turn": 1,
  "audio": "<base64 인코딩된 오디오>"
}
```

| 필드 | 타입 | 필수 | 설명 |
|------|------|------|------|
| questId | int | ✅ | 퀘스트 ID |
| turn | int | ✅ | 현재 대화 턴 번호 (1부터 시작) |
| audio | string | ✅ | base64 인코딩된 오디오 데이터 (WAV 형식) |

**Response (200 OK) - 퀘스트 미완료 턴:**

```json
{
  "isSuccess": true,
  "data": {
    "npcDialogue": "No worries at all! How can I help you today?",
    "npcDialogueAudio": "<base64 인코딩된 오디오>",
    "isTurnPassed": false,
    "turnEvaluation": {
      "contextRelevance": 0.2,
      "grammarAccuracy": 1.0,
      "expressionQuality": 0.5,
      "objectiveProgress": [],
      "isQuestComplete": false,
      "reason": "사용자의 말이 맥락과 관련이 없는 사과뿐이지만, 문법적으로 정확하다.",
      "betterSuggestions": ["hello", "hi"],
      "recommendationReason": "대화를 자연스럽게 시작할 수 있는 인사말이 더 적절하다."
    },
    "questResult": null
  }
}
```

**Response (200 OK) - 퀘스트 완료 턴 (`isQuestComplete: true`):**

```json
{
  "isSuccess": true,
  "data": {
    "npcDialogue": "Perfect! Your order is all set. Have a great day!",
    "npcDialogueAudio": "<base64 인코딩된 오디오>",
    "isTurnPassed": true,
    "turnEvaluation": {
      "contextRelevance": 0.9,
      "grammarAccuracy": 0.95,
      "expressionQuality": 0.85,
      "objectiveProgress": ["최종 주문을 확인하세요"],
      "isQuestComplete": true,
      "reason": "모든 주문 목표를 달성하고 자연스럽게 대화를 마무리했다.",
      "betterSuggestions": ["bye"],
      "recommendationReason": "대화를 자연스럽게 끝낼 수 있는 인사말이 더 적절하다."
    },
    "questResult": {
      "averageContextRelevance": 0.75,
      "averageGrammarAccuracy": 0.90,
      "averageExpressionQuality": 0.80,
      "achievedObjectives": ["음료 사이즈를 선택하세요", "최종 주문을 확인하세요"],
      "isQuestSuccess": true
    }
  }
}
```

| 필드 | 타입 | 설명 |
|------|------|------|
| npcDialogue | string | NPC 응답 텍스트 |
| npcDialogueAudio | string | NPC 응답 음성 (base64) |
| isTurnPassed | boolean | 해당 턴 통과 여부 |
| turnEvaluation.contextRelevance | float (0~1) | AI 채점: 의미 관련성 |
| turnEvaluation.grammarAccuracy | float (0~1) | AI 채점: 문법 정확성 |
| turnEvaluation.expressionQuality | float (0~1) | AI 채점: 표현의 자연스러움 |
| turnEvaluation.objectiveProgress | array\<string\> | 이번 턴에서 달성된 목표 목록 |
| turnEvaluation.isQuestComplete | boolean | 퀘스트 완료 여부 |
| turnEvaluation.reason | string | AI 채점 근거 |
| turnEvaluation.betterSuggestions | array\<string\> | 더 나은 표현 제안 - 플래시카드 저장 대상 |
| turnEvaluation.recommendationReason | string | 제안 이유 - `user_flashcards.recommendation_reason`으로 저장 |
| questResult | object\|null | 퀘스트 완료 시 결과. `isQuestComplete: false`이면 `null` |
| questResult.averageContextRelevance | float (0~1) | 세션 전체 의미 관련성 평균 |
| questResult.averageGrammarAccuracy | float (0~1) | 세션 전체 문법 정확성 평균 |
| questResult.averageExpressionQuality | float (0~1) | 세션 전체 표현 자연스러움 평균 |
| questResult.achievedObjectives | array\<string\> | 달성한 퀘스트 목표 목록 |
| questResult.isQuestSuccess | boolean | 퀘스트 성공 여부 |

**에러:**
- `SESSION_NOT_FOUND`: 존재하지 않는 세션
- `SESSION_ALREADY_ENDED`: 이미 종료된 세션
- `INVALID_AUDIO`: 잘못된 음성 데이터 형식
- `AI_SERVICE_ERROR`: AI 서비스 호출 실패

---

#### `POST /sessions/{sessionId}/end` 🔒- 게임 세션 강제 종료 (포기/오류)

> 정상 완료 시에는 `/speech` 응답의 `questResult`로 결과가 반환됩니다. 이 엔드포인트는 중도 포기(`FAILED`) 또는 오류 처리 용도입니다.

**Response (200 OK):**

```json
{
  "isSuccess": true,
  "data": {
    "sessionId": "a1b2c3d4-5678-90ab-cdef-1234567890ab",
    "status": "FAILED",
    "endedAt": "2026-04-07T14:28:05Z"
  }
}
```

| status 값 | 설명 |
|-----------|------|
| `IN_PROGRESS` | 진행 중 |
| `COMPLETED` | 정상 완료 |
| `ABANDONED` | 비정상 종료 |
| `FAILED` | 퀘스트 실패 (목표 미달성) 및 사용자 정상 포기 |

**에러:**
- `SESSION_NOT_FOUND`: 존재하지 않는 세션
- `SESSION_ALREADY_ENDED`: 이미 종료된 세션

---

### 5.5 Flashcards (플래시카드 / 단어 학습)

#### `GET /flashcards` 🔒- 내 플래시카드 목록 조회

**Query Parameters:**

| 파라미터 | 타입 | 필수 | 기본값 | 설명 |
|----------|------|------|--------|------|
| cursor | string | ❌ | null | 이전 응답의 `nextCursor` 값. 첫 요청 시 생략 |
| size | int | ❌ | 20 | 한 번에 가져올 항목 수 |
| questId | long | ❌ | null | 특정 퀘스트의 단어만 조회 (`user_flashcards.quest_id`) |
| sort | string | ❌ | `recent` | 정렬: `recent`, `alphabetical` |

**Response (200 OK):**

```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "flashcardId": 101,
        "word": "espresso",
        "meaning": "에스프레소, 고압으로 추출한 진한 커피",
        "phonetic": "/eˈspresəʊ/",
        "isMastered": false,
        "savedAt": "2026-04-07T14:30:00Z",
        "questId": 2,
        "questTitle": "커스텀 주문"
      },
      {
        "flashcardId": 102,
        "word": "latte",
        "meaning": "라떼, 에스프레소에 스팀 밀크를 넣은 음료",
        "phonetic": "/ˈlɑːteɪ/",
        "isMastered": true,
        "savedAt": "2026-04-06T10:00:00Z",
        "questId": 2,
        "questTitle": "커스텀 주문"
      }
    ],
    "nextCursor": "dXNlcl9mbGFzaGNhcmRfaWQ6MTAw",
    "hasMore": true
  }
}
```

| 필드 | 타입 | 설명 |
|------|------|------|
| nextCursor | string\|null | 다음 페이지 커서. `null`이면 마지막 페이지 |
| hasMore | boolean | 추가 데이터 존재 여부 |

---

#### `GET /flashcards/{flashcardId}` 🔒- 플래시카드 상세 조회

**Path Parameters:**

| 파라미터 | 타입 | 필수 | 설명 |
|----------|------|------|------|
| flashcardId | long | ✅ | 조회할 플래시카드 ID |

**Response (200 OK):**

```json
{
  "isSuccess": true,
  "data": {
    "flashcardId": 101,
    "word": "espresso",
    "meaning": "에스프레소, 고압으로 추출한 진한 커피",
    "phonetic": "/eˈspresəʊ/",
    "audioUrl": "https://cdn.speakat.com/audio/espresso.mp3",
    "isMastered": false,
    "questId": 2,
    "questTitle": "커스텀 주문"
  }
}
```

**에러:**
- `FLASHCARD_NOT_FOUND`: 존재하지 않는 플래시카드

---

#### `PATCH /flashcards/{flashcardId}` 🔒- 플래시카드 마스터

**Path Parameters:**

| 파라미터 | 타입 | 필수 | 설명 |
|----------|------|------|------|
| flashcardId | long | ✅ | 수정할 플래시카드 ID |

**Request Body:**

```json
{
  "isMastered": true
}
```

| 필드 | 타입 | 필수 | 설명 |
|------|------|------|------|
| isMastered | boolean | ✅ | 숙달 여부 — 현재는 사용자가 직접 설정, 추후 퀴즈 통과 시 자동 설정으로 전환 예정 |

**Response (200 OK):**

```json
{
  "isSuccess": true,
  "data": {
    "flashcardId": 101,
    "isMastered": true
  }
}
```

**에러:**
- `FLASHCARD_NOT_FOUND`: 존재하지 않는 플래시카드

---

### 5.6 User / My Page (마이페이지)

#### `GET /users/me` 🔒- 내 프로필 조회

**Response (200 OK):**

```json
{
  "isSuccess": true,
  "data": {
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "nickname": "학습자",
    "profileImageUrl": "https://cdn.speakat.com/profiles/default.png",
    "englishLevel": "INTERMEDIATE"
  }
}
```

| 필드 | 타입 | 설명 |
|------|------|------|
| englishLevel | enum | `semantic/grammar/naturalness` 누적 평균으로 산출 |

**English Level 기준 (누적 3항목 평균):**

| 레벨 | 범위 |
|------|------|
| `BEGINNER` | ~ 40 |
| `ELEMENTARY` | 40 ~ 60 |
| `INTERMEDIATE` | 60 ~ 75 |
| `UPPER_INTERMEDIATE` | 75 ~ 88 |
| `ADVANCED` | 88 ~ |

**에러:** 없음

---

#### `PATCH /users/me` 🔒- 프로필 수정

**Request Body:**

```json
{
  "nickname": "고급학습자",
  "profileImageKey": "profiles/user123.png"
}
```

> 모든 필드는 선택적이며, 포함된 필드만 업데이트됩니다.  
> **profileImageKey**: 이미지 업로드 후 반환된 S3 객체 키를 전달합니다. 응답에서는 `profileImageUrl`로 변환되어 반환됩니다.

**Response (200 OK):**

```json
{
  "isSuccess": true,
  "data": {
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "nickname": "고급학습자",
    "profileImageUrl": "https://cdn.speakat.com/profiles/user123.png"
  }
}
```

**에러:**
- `DUPLICATE_NICKNAME`: 이미 존재하는 닉네임
- `INVALID_REQUEST`: 닉네임 형식 오류

---

#### `DELETE /users/me` 🔒- 회원 탈퇴

**Response (204 No Content)**

> 회원 탈퇴 시 Soft Delete 처리되며, 30일 후 데이터가 영구 삭제됩니다.

---

#### `GET /users/me/stats` 🔒- 학습 통계 조회

**Response (200 OK):**

```json
{
  "isSuccess": true,
  "data": {
    "semanticScore": 100,
    "grammarScore": 50,
    "naturalnessScore": 0
  }
}
```

**에러:** 없음

#### `GET /users/me/streak` 🔒- 연속 학습 조회

**Response (200 OK):**

```json
{
  "isSuccess": true,
  "data": {
    "currentStreak": 5,
    "goal": 7,
    "daysToGoal": 2
  }
}
```

| 필드 | 타입 | 설명 |
|------|------|------|
| currentStreak | int | 현재 연속 학습 일수 (`game_sessions` 날짜 집계) |
| goal | int | 목표 연속 학습 일수 (`user_settings.streak_goal`) |
| daysToGoal | int | 목표까지 남은 일수 (`max(0, goal - currentStreak)`) |

**에러:** 없음

---

#### `GET /users/me/settings` 🔒- 사용자 설정 조회

**Response (200 OK):**

```json
{
  "isSuccess": true,
  "data": {
    "showNpcScript": true
  }
}
```

| 필드 | 타입 | 설명 |
|------|------|------|
| showNpcScript | boolean | NPC 대사 스크립트 표시 여부 (`user_settings.show_npc_script`) |

**에러:** 없음

---

#### `PATCH /users/me/settings` 🔒- 사용자 설정 수정

**Request Body:**

```json
{
  "showNpcScript": false
}
```

> 모든 필드는 선택적이며, 포함된 필드만 업데이트됩니다.

**Response (200 OK):**

```json
{
  "isSuccess": true,
  "data": {
    "showNpcScript": false
  }
}
```

**에러:** 없음

---

#### `GET /users/me/calendar` 🔒- 월별 학습 기록 조회

**Query Parameters:**

| 파라미터 | 타입 | 필수 | 설명 |
|----------|------|------|------|
| year | int | ✅ | 연도 (예: 2026) |
| month | int | ✅ | 월 (1~12) |

**Response (200 OK):**

```json
{
  "isSuccess": true,
  "data": {
    "year": 2026,
    "month": 4,
    "records": [
      {
        "date": "2026-04-01",
        "played": true,
        "sessionsCount": 2,
        "wordsLearned": 8,
        "averageScore": 85
      },
      {
        "date": "2026-04-02",
        "played": false,
        "sessionsCount": 0,
        "wordsLearned": 0,
        "averageScore": null
      },
      {
        "date": "2026-04-03",
        "played": true,
        "sessionsCount": 1,
        "wordsLearned": 5,
        "averageScore": 78
      }
    ]
  }
}
```

| 필드 | 타입 | 설명 |
|------|------|------|
| sessionsCount | int | 해당 날 플레이한 세션 수 |
| wordsLearned | int | 해당 날 저장된 플래시카드 단어 수 |
| averageScore | int\|null | 세션 평균 점수 (0~100%). 해당 날 세션 없으면 `null` |

**에러:**
- `INVALID_REQUEST`: year 또는 month 누락/범위 오류

---

## 부록: HTTP 상태 코드 요약

| 코드 | 의미 | 사용 상황 |
|------|------|-----------|
| 200 | OK | 조회, 수정 성공 |
| 201 | Created | 리소스 생성 성공 (회원가입, 세션 시작) |
| 204 | No Content | 처리 성공, 반환 데이터 없음 (로그아웃) |
| 400 | Bad Request | 요청 데이터 유효성 검사 실패 |
| 401 | Unauthorized | 인증 실패 (토큰 만료/누락/유효하지 않음) |
| 403 | Forbidden | 접근 권한 없음 (잠긴 스테이지 등) |
| 404 | Not Found | 리소스를 찾을 수 없음 |
| 409 | Conflict | 리소스 충돌 (중복 닉네임, 진행 중인 세션) |
| 429 | Too Many Requests | Rate Limit 초과 |
| 500 | Internal Server Error | 서버 내부 오류 |
| 502 | Bad Gateway | 외부 서비스 호출 실패 (Claude API, Google STT) |