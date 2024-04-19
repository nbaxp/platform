namespace Wta.Infrastructure.Web;

public class AutoErrorMessageMetadataProvider : IDisplayMetadataProvider
{
    public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
    {
        var attributes = context.Attributes;
        var displayAttribute = attributes.OfType<DisplayAttribute>().FirstOrDefault();
        if (displayAttribute != null && string.IsNullOrEmpty(displayAttribute.Name))
        {
            displayAttribute.Name = $"{context.Key.ContainerType?.Name}.{context.Key.Name}";
        }
        //此处必须保留
        foreach (var item in attributes)
        {
            if (item is ValidationAttribute attribute)
            {
                if (string.IsNullOrEmpty(attribute.ErrorMessage))
                {
                    if (attribute is DataTypeAttribute data && attribute.ErrorMessage != null)
                    {
                        attribute.ErrorMessage = data.GetDataTypeName();
                    }
                    else if (item is RequiredAttribute required)
                    {
                        required.ErrorMessage = "Required";
                    }
                    else
                    {
                        attribute.ErrorMessage = attribute.GetType().Name.TrimEnd("Attribute");
                        if (item is StringLengthAttribute stringLengthAttribute)
                        {
                            if (stringLengthAttribute.MinimumLength != 0)
                            {
                                attribute.ErrorMessage += "IncludingMinimum";
                            }
                        }
                    }
                }
            }
        }
    }
}
