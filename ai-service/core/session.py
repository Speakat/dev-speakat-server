# Redis 기반 대화 히스토리 관리
import json
import redis.asyncio as aioredis
from config import REDIS_URL, SESSION_TTL

_redis: aioredis.Redis | None = None


def _get_client() -> aioredis.Redis:
    global _redis
    if _redis is None:
        _redis = aioredis.from_url(REDIS_URL, decode_responses=True)
    return _redis


def _session_key(session_id: str, quest_id: int) -> str:
    return f"conversation:{quest_id}:{session_id}"


async def get_history(session_id: str, quest_id: int) -> list[dict]:
    client = _get_client()
    key = _session_key(session_id, quest_id)
    raw = await client.get(key)
    if raw is None:
        return []
    return json.loads(raw)


async def append_turn(session_id: str, quest_id: int, user_text: str, assistant_text: str) -> None:
    client = _get_client()
    key = _session_key(session_id, quest_id)
    history = await get_history(session_id, quest_id)
    history.append({"role": "user", "content": user_text})
    history.append({"role": "assistant", "content": assistant_text})
    await client.set(key, json.dumps(history, ensure_ascii=False), ex=SESSION_TTL)


async def clear_history(session_id: str, quest_id: int) -> None:
    client = _get_client()
    await client.delete(_session_key(session_id, quest_id))