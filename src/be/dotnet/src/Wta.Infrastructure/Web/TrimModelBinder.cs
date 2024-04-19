namespace Wta.Infrastructure.Web;

public class TrimModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var modelName = bindingContext.ModelName;
        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.Result = ModelBindingResult.Success(valueProviderResult.FirstValue?.Trim());
        return Task.CompletedTask;
    }
}
