// public async Task<QuestPromptDto> GetQuestPromptDtoAsync(int questId)
// {
//     var quest = await _db.Quests
//         .Include(q => q.Prompt)
//         .Include(q => q.QuestNpcsList)
//             .ThenInclude(qn => qn.Npc)
//         .Include(q => q.QuestObjectivesList)
//             .ThenInclude(qo => qo.Objective)
//         .FirstOrDefaultAsync(q => q.QuestId == questId);

//     // 등장 npc는 퀘스트 당 1명
//     var questNpc = quest.QuestNpcsList.First();

//     return new QuestPromptDto
//     {
//         Scenario = quest.Prompt.Scenario,
//     }
// }

// public async Task<string> GetOpeningLineAsync(int questId)
// {
//     // TODO: opening_line 저장 위치 결정 후 구현
// }