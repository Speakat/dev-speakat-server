from pydantic import BaseModel

class EvaluateRequest(BaseModel):
    audio_base64: str  # wav base64
    quest_id: int
    turn: int