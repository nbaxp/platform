using Wta.Infrastructure.Application.Domain;

namespace Wta.Infrastructure.Auth;

public interface IResourceService
{
}

public interface IResourceService<TResource> : IResourceService
    where TResource : IResource
{
}
