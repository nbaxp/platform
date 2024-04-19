using System.Security.Cryptography;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Wta.Infrastructure.FileProviders;

[Service<IFileService>]
public class LocalFileService(IWebHostEnvironment environment, FileExtensionContentTypeProvider contentTypeProvider) : IFileService
{
    private const string basePath = "upload";

    public FileContentResult Download(string fileName, string? fileDownloadName = null)
    {
        var file = basePath + $"/{fileName}";
        var phicyPath = Path.Combine(environment.ContentRootPath, file);
        var extension = Path.GetExtension(phicyPath);
        contentTypeProvider.TryGetContentType(extension, out var contentType);
        var bytes = File.ReadAllBytes(phicyPath);
        return new FileContentResult(bytes, contentType ?? "application/octet-stream")
        {
            FileDownloadName = fileDownloadName
        };
    }

    public string Upload(IFormFile formFile)
    {
        using var stream = formFile.OpenReadStream();
        var ext = Path.GetExtension(formFile.FileName);
        var md5 = Convert.ToHexString(MD5.HashData(stream)).ToLowerInvariant();
        var phicyPath = Path.Combine(environment.ContentRootPath, basePath);
        Directory.CreateDirectory(phicyPath);
        var name = string.Format(CultureInfo.CurrentCulture, "{0}{1}", md5, ext);
        var fullName = Path.Combine(phicyPath, name);
        if (!File.Exists(fullName))
        {
            using var fs = File.Create(fullName);
            formFile.CopyTo(fs);
        }
        return name;
    }
}
