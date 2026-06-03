namespace Speakat.Application.Common.Interfaces;

public record DictionaryWordData(
    string Text,
    IReadOnlyList<string> Definitions,
    string Phonetic,
    string? AudioUrl
);

public interface IDictionaryService
{
    Task<DictionaryWordData?> LookupAsync(string word);
}
