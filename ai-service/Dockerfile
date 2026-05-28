FROM python:3.11-slim AS builder

RUN apt-get update && apt-get install -y --no-install-recommends \
    build-essential \
    gcc \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# ONNX 모델과 토크나이저를 빌드 시점에 다운로드
ENV HF_HOME=/hf_cache
RUN python -c "\
from huggingface_hub import hf_hub_download; \
hf_hub_download('sentence-transformers/all-MiniLM-L6-v2', 'onnx/model.onnx'); \
hf_hub_download('sentence-transformers/all-MiniLM-L6-v2', 'tokenizer.json')"


FROM python:3.11-slim

WORKDIR /app

# 빌드 도구 제외하고 패키지와 모델만 복사
COPY --from=builder /usr/local/lib/python3.11/site-packages /usr/local/lib/python3.11/site-packages
COPY --from=builder /usr/local/bin /usr/local/bin
COPY --from=builder /hf_cache /hf_cache

ENV HF_HOME=/hf_cache

COPY . .

EXPOSE 8000

CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8000"]
