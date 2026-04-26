import base64
from fastapi import FastAPI, HTTPException
from models.request import EvaluateRequest
from models.response import EvaluateResponse
from core.stt import transcribe
from core.embedding import check_similarity
from core.evaluator import evaluate
from core.tts import speak_roleplay

app = FastAPI()
#TODO: 컨텍스트 json 로드 필요
@app.post("/evaluate", response_model=EvaluateResponse)
async def evaluate_endpoint(req: EvaluateRequest) -> EvaluateResponse:
    try:
        audio_bytes = base64.b64decode(req.audio_base64)
    except Exception:
        raise HTTPException(status_code=400, detail="audio_base64 디코딩 실패")

    # 1. STT
    user_text = transcribe(audio_bytes)

    # 2. 유사도 필터
    sim_score, passed = check_similarity(user_text)

    if not passed:
        return EvaluateResponse(
            roleplay="",
            roleplay_audio="",
            score=0,
            grade="wrong",
            better_suggestions=["sorry", "apologize", "regret"],
            recommendation_reason="주제와 관련 없는 발화입니다.",
            similarity_score=sim_score,
            similarity_passed=False,
        )

    # 3. GPT Realtime 평가
    result = await evaluate(audio_bytes)

    roleplay_text = result.get("roleplay", "")
    roleplay_audio = await speak_roleplay(roleplay_text)

    return EvaluateResponse(
        roleplay=roleplay_text,
        roleplay_audio=roleplay_audio,
        score=result.get("score", 0),
        grade=result.get("grade", "wrong"),
        better_suggestions=result.get("better_suggestions", []),
        recommendation_reason=result.get("recommendation_reason", ""),
        similarity_score=sim_score,
        similarity_passed=True,
    )