import base64
from fastapi import FastAPI, HTTPException
from fastapi.responses import JSONResponse
from models.request import EvaluateRequest
from models.response import EvaluateResponse, TurnEvaluation, QuestResult
from core.stt import transcribe
from core.embedding import check_similarity
from core.evaluator import evaluate
from core.tts import speak_roleplay
from core.session import get_quest_summary, get_eval_data, start_closing_phase, tick_closing_phase

app = FastAPI()

@app.post("/evaluate", response_model=EvaluateResponse)
async def evaluate_endpoint(req: EvaluateRequest):
    try:
        audio_bytes = base64.b64decode(req.audio_base64)
    except Exception:
        raise HTTPException(status_code=400, detail="audio_base64 디코딩 실패")

    try:
        # STT
        user_text = transcribe(audio_bytes)

        #유사도 필터 (첫 번째 턴 0.3, 이후 0.1)
        reference_sentences = req.quest_prompt.get("reference_sentences", [])
        sim_threshold = 0.3 if req.turn == 1 else 0.1
        sim_score, passed = check_similarity(user_text, reference_sentences, sim_threshold)

        if not passed:
            return EvaluateResponse(
                user_text=user_text,
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

        #이전 턴의 마무리 단계 상태 확인 후 GPT 평가
        eval_data_before = await get_eval_data(req.session_id, req.quest_id)
        closing_turns_before: int | None = eval_data_before.get("closing_turns_remaining")

        result = await evaluate(req.session_id, req.quest_id, user_text, req.quest_prompt, closing_turns_before)

        npc_dialogue_text = result.get("npc_dialogue", "")
        npc_voice = req.quest_prompt.get("npc", {}).get("voice")
        if npc_voice is None:
            raise ValueError("quest_prompt.npc.voice가 설정되지 않았습니다.")
        npc_dialogue_audio = await speak_roleplay(npc_dialogue_text, npc_voice)
        turn_eval = result.get("turn_evaluation", {})

        #퀘스트 완료 처리: 목표 달성 즉시 종료하지 않고 마무리 단계를 거침
        all_objectives = [obj["name"] for obj in req.quest_prompt.get("objectives", [])]
        eval_data_after = await get_eval_data(req.session_id, req.quest_id)
        achieved = set(eval_data_after.get("achieved_objectives", []))
        all_achieved = set(all_objectives) <= achieved

        quest_result: QuestResult | None = None
        is_complete = False

        if not all_achieved:
            # 아직 목표 미달성: AI가 true를 내도 무시
            pass
        elif closing_turns_before is None:
            #전체 목표 달성 -> 마무리 단계 시작
            await start_closing_phase(req.session_id, req.quest_id)
        else:
            # 마무리 단계 진행 중: 턴 차감 후 종료 여부 판단
            remaining = await tick_closing_phase(req.session_id, req.quest_id)
            if turn_eval.get("is_quest_complete", False) or remaining == 0:
                is_complete = True
                summary = await get_quest_summary(req.session_id, req.quest_id, all_objectives)
                quest_result = QuestResult(**summary)

        turn_eval["is_quest_complete"] = is_complete

        return EvaluateResponse(
            user_text=user_text,
            npc_dialogue=npc_dialogue_text,
            npc_dialogue_audio=npc_dialogue_audio,
            similarity_score=sim_score,
            similarity_passed=True,
            turn_evaluation=TurnEvaluation(**turn_eval),
            quest_result=quest_result,
        )

    except Exception:
        return JSONResponse(status_code=500, content={"error": "Internal server error"})
