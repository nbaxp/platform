using Microsoft.AspNetCore.Http;

namespace Wta.Infrastructure.Application.Models;

public class ImportModel<T> : QueryModel<T>
{
    public bool Update { get; set; }
    public List<IFormFile> Files { get; set; } = new List<IFormFile>();
}
