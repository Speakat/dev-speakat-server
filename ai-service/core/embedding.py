"""
임베딩(문장 간 유사도 계산) 처리
"""

import torch
import torch.nn.functional as F
from transformers import AutoTokenizer, AutoModel
from config import EMBED_MODEL, REFERENCE_SENTENCE, SIMILARITY_THRESHOLD

print("임베딩 모델 로딩 중...")
_tokenizer  = AutoTokenizer.from_pretrained(EMBED_MODEL)
_model      = AutoModel.from_pretrained(EMBED_MODEL)
_model.eval()
print("임베딩 모델 로딩 완료")


def _embed(text: str) -> torch.Tensor:
    encoded = _tokenizer(text, padding=True, truncation=True, return_tensors="pt")
    with torch.no_grad():
        output = _model(**encoded)
    token_emb = output[0]
    mask      = encoded["attention_mask"].unsqueeze(-1).expand(token_emb.size()).float()
    pooled    = torch.sum(token_emb * mask, 1) / torch.clamp(mask.sum(1), min=1e-9)
    return F.normalize(pooled, p=2, dim=1)


def check_similarity(user_text: str) -> tuple[float, bool]:
    score  = F.cosine_similarity(_embed(REFERENCE_SENTENCE), _embed(user_text)).item()
    passed = score >= SIMILARITY_THRESHOLD
    return score, passed