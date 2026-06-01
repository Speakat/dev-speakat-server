using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Speakat.Application.Common.Interfaces;

namespace Speakat.Infrastructure.Storage;

public class S3ImageStorageService : IImageStorageService
{
    private readonly AmazonS3Client _s3Client;
    private readonly string _bucketName;
    private readonly string _region;
    private readonly IHttpClientFactory _httpClientFactory;

    public S3ImageStorageService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _region = configuration["AWS:Region"] ?? throw new InvalidOperationException("AWS:Region이 설정되지 않았습니다.");
        _bucketName = configuration["AWS:BucketName"] ?? throw new InvalidOperationException("AWS:BucketName이 설정되지 않았습니다.");
        var accessKeyId = configuration["AWS:AccessKeyId"] ?? throw new InvalidOperationException("AWS:AccessKeyId가 설정되지 않았습니다.");
        var secretAccessKey = configuration["AWS:SecretAccessKey"] ?? throw new InvalidOperationException("AWS:SecretAccessKey가 설정되지 않았습니다.");
        _s3Client = new AmazonS3Client(accessKeyId, secretAccessKey, RegionEndpoint.GetBySystemName(_region));
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> GenerateUploadUrlAsync(string key, string contentType = "image/jpeg")
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddMinutes(5),
            ContentType = contentType
        };

        return await _s3Client.GetPreSignedURLAsync(request);
    }

    public async Task UploadFromUrlAsync(string sourceUrl, string key)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var imageBytes = await httpClient.GetByteArrayAsync(sourceUrl);

        using var stream = new MemoryStream(imageBytes);
        await _s3Client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = stream,
            ContentType = "image/jpeg"
        });
    }

    public string GetPublicUrl(string key)
        => $"https://{_bucketName}.s3.{_region}.amazonaws.com/{key}";
}
