"""
speech to text 처리
"""

import tempfile, os
from openai import OpenAI
from config import OPENAI_API_KEY

_client = OpenAI(api_key=OPENAI_API_KEY)

def transcribe(audio_bytes: bytes) -> str:
    with tempfile.NamedTemporaryFile(suffix=".wav", delete=False) as tmp:
        tmp.write(audio_bytes)
        tmp_path = tmp.name
    try:
        with open(tmp_path, "rb") as f:
            result = _client.audio.transcriptions.create(
                model="whisper-1",
                file=f,
                language="en",
            )
        return result.text.strip()
    finally:
        os.remove(tmp_path)