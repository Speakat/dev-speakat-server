from pydantic import BaseModel

class EvaluateResponse(BaseModel):
    roleplay: str
    roleplay_audio: str
    score: int
    grade: str
    better_suggestions: list[str]
    recommendation_reason: str
    similarity_score: float
    similarity_passed: bool
