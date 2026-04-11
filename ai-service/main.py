from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import base64, asyncio

import asyncio
import json
import re
import base64
import pyaudio
import wave
import tempfile
import os
from openai import AsyncOpenAI, OpenAI
from transformers import AutoTokenizer, AutoModel
import torch
import torch.nn.functional as F


app = FastAPI()

# 사용자 요청 오디오
class EvaluateRequest(BaseModel):
    audio_base64: str

class EvaluateResponse(BaseModel):
    roleplay: str
    score: int
    grade: str
    better_suggestions: list[str]
    recommendation_reason: str
    similarity_score: float
    similarity_passed: bool

@app.post("/evaluate", response_model=EvaluateResponse)
async def evaluate(req: EvaluateRequest):
    audio_bytes = base64.b64decode(req.audio_base64)
    
    # 1. STT
    user_text = transcribe_audio(audio_bytes)
    
    # 2. 임베딩 필터
    sim_score, passed = check_similarity(user_text)
    
    if not passed:
        return EvaluateResponse(
            roleplay="",
            score=0,
            grade="wrong",
            better_suggestions=["sorry", "apologize", "regret"],
            recommendation_reason="주제와 관련 없는 발화입니다.",
            similarity_score=sim_score,
            similarity_passed=False,
        )
    
    # 3. GPT Realtime
    result = await evaluate_with_realtime(audio_bytes)
    
    return EvaluateResponse(
        **result,
        similarity_score=sim_score,
        similarity_passed=True,
    )