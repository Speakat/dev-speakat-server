#대화 히스토리 빌더
def build_messages(session_messages: list[dict]) -> list[dict]:
    return [
        {"role": msg["role"], "content": msg["content"]}
        for msg in session_messages
    ]