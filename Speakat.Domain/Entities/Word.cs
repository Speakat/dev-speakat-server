namespace Speakat.Domain.Entities;

public class Word
{
    public long WordId { get; set; }
    public long LanguageId { get; set; }
    public string Text { get; set; } = null!;
    public string Definition { get; set; } = null!;
    public string Phonetic { get; set; } = null!;
    public string? AudioUrl { get; set; }

    public Language? Language { get; set; }
}
