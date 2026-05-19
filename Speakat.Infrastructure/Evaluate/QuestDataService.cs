using Microsoft.EntityFrameworkCore;
using Speakat.Application.Common.Exceptions;
using Speakat.Infrastructure.Persistence;

namespace Speakat.Infrastructure.Evaluate;

public class QuestDataService(AppDbContext db) : IQuestDataService
{
    public async Task<QuestPromptDto> GetQuestPromptDtoAsync(int questId)
    {
        var quest = await db.Quests
            .Include(q => q.Prompt)
            .FirstOrDefaultAsync(q => q.QuestId == questId)
            ?? throw new NotFoundException($"Quest {questId} 없음");

        var questNpc = await db.QuestNpcs
            .Include(qn => qn.Npc)
            .FirstOrDefaultAsync(qn => qn.QuestId == questId)
            ?? throw new NotFoundException($"Quest {questId}에 NPC 없음");

        var objectives = await db.QuestObjectives
            .Include(qo => qo.Objective)
            .Where(qo => qo.QuestId == questId)
            .OrderBy(qo => qo.SortOrder)
            .ToListAsync();

        return new QuestPromptDto(
            Scenario:        quest.Prompt!.Scenario,
            SuccessCriteria: quest.Prompt.SuccessCriteria,
            Npc: new NpcDto(
                Name:  questNpc.Npc!.Name,
                Role:  questNpc.Role,
                Tone:  questNpc.Npc.Tone,
                Voice: questNpc.Npc.Voice
            ),
            Objectives: objectives
                .Select(qo => new ObjectiveDto(
                    qo.Objective!.Name,
                    qo.Objective.Description ?? ""))
                .ToList()
        );
    }

    public async Task<string> GetOpeningLineAsync(int questId)
    {
        var quest = await db.Quests
            .Include(q => q.Prompt)
            .FirstOrDefaultAsync(q => q.QuestId == questId)
            ?? throw new NotFoundException($"Quest {questId} 없음");

        return quest.Prompt!.OpeningLine;
    }
}
