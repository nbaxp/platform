using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;

namespace Wta.Infrastructure.FileProviders;

public class MinIOFileService(IConfiguration configuration, FileExtensionContentTypeProvider contentTypeProvider) : IFileService
{
    public FileContentResult Download(string fileName, string? fileDownloadName = null)
    {
        using var client = CreateClient();
        client.StatObjectAsync(new StatObjectArgs().WithBucket(GetBucket()).WithObject(fileName)).Wait();
        using var stream = new MemoryStream();
        var args = new GetObjectArgs()
            .WithBucket(GetBucket())
            .WithObject(fileName)
            .WithCallbackStream(o =>
            {
                o.CopyTo(stream);
            });
        var result = client.GetObjectAsync(args).Result;
        var contentType = result.ContentType;
        var data = stream.ToArray();
        return new FileContentResult(data, contentType)
        {
            FileDownloadName = fileDownloadName
        };
    }

    public string Upload(IFormFile formFile)
    {
        using var stream = formFile.OpenReadStream();
        var ext = Path.GetExtension(formFile.FileName);
        var md5 = Convert.ToHexString(MD5.HashData(stream)).ToLowerInvariant();
        var fileName = string.Format(CultureInfo.CurrentCulture, "{0}{1}", md5, ext);

        using var client = CreateClient();
        try
        {
            client.StatObjectAsync(new StatObjectArgs().WithBucket(GetBucket()).WithObject(fileName)).Wait();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            stream.Seek(0, SeekOrigin.Begin);
            contentTypeProvider.TryGetContentType(fileName, out var contentType);
            var args = new PutObjectArgs()
                .WithBucket(GetBucket())
                .WithObject(fileName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(contentType ?? "application/octet-stream");
            var uploadResult = client.PutObjectAsync(args).Result;
        }
        return fileName;
    }

    private IMinioClient CreateClient()
    {
        var endpoint = configuration.GetValue("MinIO:Endpoint", "minio:9000");
        var accessKey = configuration.GetValue("MinIO:AccessKey", "admin");
        var secretKey = configuration.GetValue("MinIO:SecretKey", "aA123456!");
        var secure = configuration.GetValue("MinIO:Secret", false);
        var minio = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
            .WithSSL(secure)
            .Build();
        var bucketName = GetBucket();
        if (!minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName)).Result)
        {
            minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName)).Wait();
        }
        return minio;
    }

    private string GetBucket()
    {
        return configuration.GetValue("MinIO:Buket", "default")!;
    }
}
