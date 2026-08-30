import os
from prompts.quest_system import build_system_prompt

OPENAI_API_KEY       = os.environ.get("OPENAI_API_KEY", "")
GPT_MODEL            = "gpt-4o"
SAMPLE_RATE          = 24000
SIMILARITY_THRESHOLD = 0.30
REDIS_URL            = os.environ.get("REDIS_URL", "redis://localhost:6379")
SESSION_TTL          = 3600  # 1시간
EMBED_MODEL          = "sentence-transformers/paraphrase-multilingual-MiniLM-L12-v2"
PASS_SCORE           = 70