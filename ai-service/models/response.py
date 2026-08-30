from pydantic import BaseModel

class BetterSuggestion(BaseModel):
    word: str
    meaning: str
    part_of_speech: str

class TurnEvaluation(BaseModel):
    context_relevance: float
    grammar_accuracy: float
    expression_quality: float
    objective_progress: list[str]
    is_quest_complete: bool
    reason: str
    better_suggestions: list[BetterSuggestion]
    recommendation_reason: str

class QuestResult(BaseModel):
    average_context_relevance: float
    average_grammar_accuracy: float
    average_expression_quality: float
    achieved_objectives: list[str]
    is_quest_success: bool

class EvaluateResponse(BaseModel):
    user_text: str
    npc_dialogue: str
    npc_dialogue_audio: str
    similarity_score: float
    similarity_passed: bool
    turn_evaluation: TurnEvaluation
    quest_result: QuestResult | None = None

class BestDefinitionResponse(BaseModel):
    best_definition: str
    scores: list[float]
