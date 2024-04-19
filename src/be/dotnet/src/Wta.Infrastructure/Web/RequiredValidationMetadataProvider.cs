namespace Wta.Infrastructure.Web;

public class RequiredValidationMetadataProvider : IValidationMetadataProvider
{
    public void CreateValidationMetadata(ValidationMetadataProviderContext context)
    {
        if (context.Key.MetadataKind == ModelMetadataKind.Parameter || context.Key.MetadataKind == ModelMetadataKind.Property)
        {
            var nullabilityContext = new NullabilityInfoContext();
            var nullability = context.Key.MetadataKind == ModelMetadataKind.Parameter ? nullabilityContext.Create(context.Key.ParameterInfo!) : nullabilityContext.Create(context.Key.PropertyInfo!);
            var isOptional = nullability != null && nullability.ReadState != NullabilityState.NotNull;
            if (!isOptional)
            {
                var attribute = new RequiredAttribute { AllowEmptyStrings = false, ErrorMessage = "Required" };
                context.ValidationMetadata.ValidatorMetadata.Add(attribute);
                context.ValidationMetadata.IsRequired = true;
            }
        }
    }
}
