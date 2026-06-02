"""
Redis 기반 대화 히스토리 및 턴 평가 누적 관리
"""
import json
import redis.asyncio as aioredis
from config import REDIS_URL, SESSION_TTL

_redis: aioredis.Redis | None = None


def _get_client() -> aioredis.Redis:
    global _redis
    if _redis is None:
        _redis = aioredis.from_url(REDIS_URL, decode_responses=True)
    return _redis


def _conv_key(session_id: str, quest_id: int) -> str:
    return f"conversation:{quest_id}:{session_id}"


def _eval_key(session_id: str, quest_id: int) -> str:
    return f"eval:{quest_id}:{session_id}"


# 대화 히스토리
async def get_history(session_id: str, quest_id: int) -> list[dict]:
    client = _get_client()
    raw = await client.get(_conv_key(session_id, quest_id))
    return json.loads(raw) if raw else []


async def append_turn(session_id: str, quest_id: int, user_text: str, assistant_text: str) -> None:
    client = _get_client()
    key = _conv_key(session_id, quest_id)
    history = await get_history(session_id, quest_id)
    history.append({"role": "user", "content": user_text})
    history.append({"role": "assistant", "content": assistant_text})
    await client.set(key, json.dumps(history, ensure_ascii=False), ex=SESSION_TTL)


async def clear_history(session_id: str, quest_id: int) -> None:
    client = _get_client()
    await client.delete(_conv_key(session_id, quest_id))


# 턴 평가 누적
async def append_turn_eval(
    session_id: str,
    quest_id: int,
    scores: dict,
    new_objectives: list[str],
) -> None:
    # 턴별 점수와 달성된 objective를 누적
    client = _get_client()
    key = _eval_key(session_id, quest_id)
    raw = await client.get(key)
    data = json.loads(raw) if raw else {"scores": [], "achieved_objectives": []}

    data["scores"].append({
        "context_relevance":  scores.get("context_relevance", 0.0),
        "grammar_accuracy":   scores.get("grammar_accuracy", 0.0),
        "expression_quality": scores.get("expression_quality", 0.0),
    })
    # 중복 없이 달성 목표 누적
    existing = set(data["achieved_objectives"])
    data["achieved_objectives"] = list(existing | set(new_objectives))

    await client.set(key, json.dumps(data, ensure_ascii=False), ex=SESSION_TTL)


async def get_eval_data(session_id: str, quest_id: int) -> dict:
    #현재 eval 누적 데이터 반환
    client = _get_client()
    raw = await client.get(_eval_key(session_id, quest_id))
    return json.loads(raw) if raw else {"scores": [], "achieved_objectives": [], "closing_turns_remaining": None}


async def start_closing_phase(session_id: str, quest_id: int) -> None:
    #모든 목표 달성 직후 마무리 단계(최대 3턴)를 시작
    client = _get_client()
    key = _eval_key(session_id, quest_id)
    raw = await client.get(key)
    data = json.loads(raw) if raw else {"scores": [], "achieved_objectives": []}
    data["closing_turns_remaining"] = 3
    await client.set(key, json.dumps(data, ensure_ascii=False), ex=SESSION_TTL)


async def tick_closing_phase(session_id: str, quest_id: int) -> int:
    #마무리 턴 카운트를 1 감소하고 남은 횟수를 반환
    client = _get_client()
    key = _eval_key(session_id, quest_id)
    raw = await client.get(key)
    data = json.loads(raw) if raw else {}
    remaining = max(0, (data.get("closing_turns_remaining") or 1) - 1)
    data["closing_turns_remaining"] = remaining
    await client.set(key, json.dumps(data, ensure_ascii=False), ex=SESSION_TTL)
    return remaining


async def get_quest_summary(
    session_id: str,
    quest_id: int,
    all_objective_names: list[str],
) -> dict:
    """퀘스트 완료 시 평균 점수와 목표 달성 여부를 반환한다."""
    client = _get_client()
    raw = await client.get(_eval_key(session_id, quest_id))
    data = json.loads(raw) if raw else {"scores": [], "achieved_objectives": []}

    scores = data["scores"]
    achieved = set(data["achieved_objectives"])

    def avg(field: str) -> float:
        return sum(s[field] for s in scores) / len(scores) if scores else 0.0

    return {
        "average_context_relevance":  avg("context_relevance"),
        "average_grammar_accuracy":   avg("grammar_accuracy"),
        "average_expression_quality": avg("expression_quality"),
        "achieved_objectives":        list(achieved),
        "is_quest_success":           set(all_objective_names) <= achieved,
    }
