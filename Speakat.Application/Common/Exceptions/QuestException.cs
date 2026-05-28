namespace Speakat.Application.Common.Exceptions;

public class QuestException: BusinessException
{
    private QuestException(string code, string message, int statusCode) : base(code, message, statusCode) { }

    public static QuestException NotFound() =>
        new("QUEST_NOT_FOUND", "존재하지 않는 퀘스트", 404);

    public static QuestException Locked() =>
        new("QUEST_LOCKED", "잠긴 퀘스트", 403);
}