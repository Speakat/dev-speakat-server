from pydantic import BaseModel

class EvaluateRequest(BaseModel):
    audio_base64: str
    quest_id: int
    session_id: str
    turn: int
    quest_prompt: dict