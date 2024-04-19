using Mapster;

namespace Wta.Infrastructure.Extensions;

public static class MapperExtensions
{
    public static TModel ToModel<TEntity, TModel>(this TEntity entity, Action<TEntity, TModel>? action = null)
    {
        var model = entity.Adapt<TModel>();
        action?.Invoke(entity, model);
        return model;
    }

    public static TEntity FromModel<TEntity, TModel>(this TEntity entity, TModel model, Action<TEntity, TModel, bool>? action = null, bool isCreate = false)
    {
        var setter = TypeAdapterConfig<TModel, TEntity>.NewConfig().Ignore(["Id"]);
        if (!isCreate)
        {
            setter.IgnoreAttribute(typeof(ReadOnlyAttribute));
        }
        var config = setter.Config;
        model.Adapt(entity, config);
        action?.Invoke(entity, model, isCreate);
        return entity;
    }
}
