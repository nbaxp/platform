using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Wta.Infrastructure.FileProviders;
public interface IFileService
{
    string Upload(IFormFile formFile);
    FileContentResult Download(string fileName, string? fileDownloadName = null);
}
