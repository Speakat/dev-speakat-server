import base64
from openai import AsyncOpenAI
from config import OPENAI_API_KEY

_client = AsyncOpenAI(api_key=OPENAI_API_KEY)

async def speak_roleplay(text: str) -> str:
    response = await _client.audio.speech.create(
        model="tts-1", voice="alloy", input=text, response_format="mp3"
    )
    audio_bytes = response.content
    return base64.b64encode(audio_bytes).decode("utf-8")
