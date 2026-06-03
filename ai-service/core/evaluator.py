"""
LLM 통한 채점
"""
import json, re
from openai import AsyncOpenAI
from config import OPENAI_API_KEY, GPT_MODEL
from core.session import get_history, append_turn, append_turn_eval, get_eval_data
from prompts.conversation import build_messages
from prompts.quest_system import build_system_prompt

_client = AsyncOpenAI(api_key=OPENAI_API_KEY)


async def evaluate(
    session_id: str,
    quest_id: int,
    user_text: str,
    quest_prompt: dict,
    closing_turns_remaining: int | None = None,
) -> dict:
    system_prompt = build_system_prompt(quest_prompt)
    history = await get_history(session_id, quest_id)

    eval_data = await get_eval_data(session_id, quest_id)
    achieved_objectives: list[str] = eval_data.get("achieved_objectives", [])

    all_objectives = [obj["name"] for obj in quest_prompt.get("objectives", [])]
    remaining_objectives = [o for o in all_objectives if o not in achieved_objectives]

    messages = [{"role": "system", "content": system_prompt}]
    messages += build_messages(history)

    if all_objectives:
        progress_note = (
            f"[SYSTEM NOTE: Objectives achieved so far: {achieved_objectives or 'none'}. "
            f"Remaining: {remaining_objectives or 'none'}.]"
        )
        messages.append({"role": "system", "content": progress_note})

    if closing_turns_remaining is not None:
        if closing_turns_remaining > 0:
            note = (
                f"[SYSTEM NOTE: All quest objectives have been achieved. "
                f"Guide the conversation to a natural, polite conclusion within {closing_turns_remaining} turn(s). "
                f"Set is_quest_complete: true only once the conversation has genuinely ended.]"
            )
        else:
            note = (
                "[SYSTEM NOTE: This is the final turn. "
                "Conclude the conversation naturally and set is_quest_complete: true.]"
            )
        messages.append({"role": "system", "content": note})

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
