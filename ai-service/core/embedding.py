"""
임베딩(문장 간 유사도 계산) 처리
"""

import numpy as np
import onnxruntime as ort
from tokenizers import Tokenizer
from huggingface_hub import hf_hub_download
from config import EMBED_MODEL, REFERENCE_SENTENCE, SIMILARITY_THRESHOLD

print("임베딩 모델 로딩 중")
_onnx_path  = hf_hub_download(EMBED_MODEL, "onnx/model.onnx")
_tok_path   = hf_hub_download(EMBED_MODEL, "tokenizer.json")
_session    = ort.InferenceSession(_onnx_path, providers=["CPUExecutionProvider"])
_tokenizer  = Tokenizer.from_file(_tok_path)
_tokenizer.enable_truncation(max_length=512)
print("임베딩 모델 로딩 완료")


def _embed(text: str) -> np.ndarray:
    encoding       = _tokenizer.encode(text)
    input_ids      = np.array([encoding.ids],            dtype=np.int64)
    attention_mask = np.array([encoding.attention_mask], dtype=np.int64)
    token_type_ids = np.zeros_like(input_ids)

    outputs   = _session.run(None, {
        "input_ids":      input_ids,
        "attention_mask": attention_mask,
        "token_type_ids": token_type_ids,
    })

    token_emb = outputs[0]  # (1, seq_len, hidden_size)
    mask      = attention_mask[..., np.newaxis].astype(np.float32)
    pooled    = np.sum(token_emb * mask, axis=1) / np.clip(mask.sum(axis=1), 1e-9, None)
    norm      = np.linalg.norm(pooled, axis=1, keepdims=True)
    return pooled / norm


def check_similarity(user_text: str) -> tuple[float, bool]:
    score  = float(np.sum(_embed(REFERENCE_SENTENCE) * _embed(user_text)))
    passed = score >= SIMILARITY_THRESHOLD
    return score, passed
