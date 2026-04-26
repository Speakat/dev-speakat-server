"""
LLM 통한 채점 
"""
import base64, json, re
from openai import AsyncOpenAI
from config import OPENAI_API_KEY, GPT_MODEL, SYSTEM_PROMPT

_client = AsyncOpenAI(api_key=OPENAI_API_KEY)


async def evaluate(audio_bytes: bytes) -> dict:
    audio_b64     = base64.b64encode(audio_bytes).decode("utf-8")
    collected     = []

    async with _client.beta.realtime.connect(model=GPT_MODEL) as conn:
        await conn.session.update(session={
            "modalities": ["text"],
            "instructions": SYSTEM_PROMPT,
            "input_audio_format": "pcm16",
            "input_audio_transcription": {"model": "whisper-1"},
            "turn_detection": None,
        })
        await conn.input_audio_buffer.append(audio=audio_b64)
        await conn.input_audio_buffer.commit()
        await conn.response.create()

        async for event in conn:
            if event.type == "response.text.delta":
                collected.append(event.delta)
            elif event.type == "response.done":
                break
            elif event.type == "error":
                raise RuntimeError(f"Realtime API 오류: {event.error}")

    raw = "".join(collected).strip()

    match = re.search(r"\{.*\}", raw, re.DOTALL)
    if not match:
        raise ValueError(f"JSON 파싱 실패: {raw}")

    return json.loads(match.group())