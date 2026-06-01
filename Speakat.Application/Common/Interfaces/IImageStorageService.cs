namespace Speakat.Application.Common.Interfaces;

public interface IImageStorageService
{
    Task<string> GenerateUploadUrlAsync(string key, string contentType);

    Task UploadFromUrlAsync(string sourceUrl, string key);

    string GetPublicUrl(string key);
}