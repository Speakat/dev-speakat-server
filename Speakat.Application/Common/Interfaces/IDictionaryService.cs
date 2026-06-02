namespace Speakat.Application.Common.Interfaces;

public record DictionaryWordData(
    string Text,
    string Definition,
    string Phonetic,
    string? AudioUrl
);

public interface IDictionaryService
{
    Task<DictionaryWordData?> LookupAsync(string word);
}
