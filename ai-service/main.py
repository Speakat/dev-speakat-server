import base64
from fastapi import FastAPI, HTTPException
from fastapi.responses import JSONResponse
from models.request import EvaluateRequest
from models.response import EvaluateResponse, TurnEvaluation, QuestResult
from core.stt import transcribe
from core.embedding import check_similarity
from core.evaluator import evaluate
from core.tts import speak_roleplay
from core.session import get_quest_summary

app = FastAPI()

@app.post("/evaluate", response_model=EvaluateResponse)
async def evaluate_endpoint(req: EvaluateRequest):
    try:
        audio_bytes = base64.b64decode(req.audio_base64)
    except Exception:
        raise HTTPException(status_code=400, detail="audio_base64 디코딩 실패")

    try:
        # 1. STT
        user_text = transcribe(audio_bytes)

        # 2. 유사도 필터
        sim_score, passed = check_similarity(user_text)

        if not passed:
            return EvaluateResponse(
                npc_dialogue="",
                npc_dialogue_audio="",
                similarity_score=sim_score,
                similarity_passed=False,
                turn_evaluation=TurnEvaluation(
                    context_relevance=0.0,
                    grammar_accuracy=0.0,
                    expression_quality=0.0,
                    objective_progress=[],
                    is_quest_complete=False,
                    reason="주제와 관련 없는 발화입니다.",
                    better_suggestions=[],
                    recommendation_reason="",
                ),
            )

        # 3. GPT 평가 (점수·목표 Redis 누적 포함)
        result = await evaluate(req.session_id, req.quest_id, user_text, req.quest_prompt)

        npc_dialogue_text = result.get("npc_dialogue", "")
        npc_voice = req.quest_prompt.get("npc", {}).get("voice")
        if npc_voice is None:
            raise ValueError("quest_prompt.npc.voice가 설정되지 않았습니다.")
        npc_dialogue_audio = await speak_roleplay(npc_dialogue_text, npc_voice)
        turn_eval = result.get("turn_evaluation", {})

        # 4. 퀘스트 완료 시 평균 점수 + 목표 달성 여부 계산
        quest_result: QuestResult | None = None
        if turn_eval.get("is_quest_complete", False):
            all_objectives = [obj["name"] for obj in req.quest_prompt.get("objectives", [])]
            summary = await get_quest_summary(req.session_id, req.quest_id, all_objectives)
            quest_result = QuestResult(**summary)

        return EvaluateResponse(
            npc_dialogue=npc_dialogue_text,
            npc_dialogue_audio=npc_dialogue_audio,
            similarity_score=sim_score,
            similarity_passed=True,
            turn_evaluation=TurnEvaluation(**turn_eval),
            quest_result=quest_result,
        )

    except Exception as e:
        return JSONResponse(status_code=500, content={"error": "Internal server error"})
