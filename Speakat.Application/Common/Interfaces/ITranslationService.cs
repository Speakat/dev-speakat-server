namespace Speakat.Application.Common.Interfaces;

public interface ITranslationService
{
    Task<string> TranslateAsync(string text, string targetLanguage);
}
