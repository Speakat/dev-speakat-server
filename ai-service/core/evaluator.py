"""
LLM 통한 채점
"""
import json, re
from openai import AsyncOpenAI
from config import OPENAI_API_KEY, GPT_MODEL
from core.session import get_history, append_turn, append_turn_eval
from prompts.conversation import build_messages
from prompts.quest_system import build_system_prompt

_client = AsyncOpenAI(api_key=OPENAI_API_KEY)


async def evaluate(session_id: str, quest_id: int, user_text: str, quest_prompt: dict) -> dict:
    system_prompt = build_system_prompt(quest_prompt)
    history = await get_history(session_id, quest_id)

    messages = [{"role": "system", "content": system_prompt}]
    messages += build_messages(history)
    messages.append({"role": "user", "content": user_text})

    response = await _client.chat.completions.create(
        model=GPT_MODEL,
        messages=messages,
        response_format={"type": "json_object"},
    )

    raw = response.choices[0].message.content.strip()

    match = re.search(r"\{.*\}", raw, re.DOTALL)
    if not match:
        raise ValueError(f"JSON 파싱 실패: {raw}")

    result = json.loads(match.group())
    turn_eval = result.get("turn_evaluation", {})

    await append_turn(session_id, quest_id, user_text, result.get("npc_dialogue", ""))
    await append_turn_eval(
        session_id,
        quest_id,
        scores=turn_eval,
        new_objectives=turn_eval.get("objective_progress", []),
    )

    return result
