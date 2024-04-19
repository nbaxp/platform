using System.Linq.Dynamic.Core;
using Wta.Infrastructure.Application.Models;

namespace Wta.Infrastructure.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> query, string ordering, params object?[] args)
    {
        query = DynamicQueryableExtensions.OrderBy(query, ordering, args);
        return query;
    }

    public static IQueryable<TEntity> WhereByModel<TEntity, TModel>(this IQueryable<TEntity> query, TModel model)
        where TModel : class
    {
        var properties = model!.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty)
            .Where(o => o.PropertyType.IsValueType || o.PropertyType == typeof(string))
            .Where(o => !o.CustomAttributes.Any(o => o.AttributeType == typeof(ScaffoldColumnAttribute)));
        foreach (var property in properties)
        {
            var propertyName = property.Name;
            var propertyValue = property.GetValue(model, null);
            if (propertyValue != null)
            {
                var attributes = property.GetCustomAttributes<OperatorTypeAttribute>().Where(o => o.OperatorType != OperatorType.Ignore);
                if (attributes.Any())
                {
                    foreach (var attribute in attributes)
                    {
                        var targetPropertyName = attribute.PropertyName ?? propertyName;
                        if (typeof(TEntity).GetProperty(targetPropertyName) != null)
                        {
                            var expression = attribute.OperatorType.GetType().GetCustomAttribute<ExpressionAttribute>()?.Expression;
                            if (expression != null)
                            {
                                query = query.Where(string.Format(CultureInfo.InvariantCulture, expression, targetPropertyName), propertyValue);
                            }
                        }
                    }
                }
                else
                {
                    if (property.PropertyType == typeof(string))
                    {
                        if (typeof(TEntity).GetProperty(propertyName) != null)
                        {
                            var expression = OperatorType.Contains.GetAttributeOfType<ExpressionAttribute>()?.Expression!;
                            query = query.Where(string.Format(CultureInfo.InvariantCulture, expression, propertyName), propertyValue);
                        }
                    }
                    else if (property.PropertyType.GetUnderlyingType() == typeof(DateTime))
                    {
                        var start = $"{propertyName}Start";
                        if (typeof(TEntity).GetProperty(start) != null)
                        {
                            var expression = OperatorType.GreaterThanOrEqual.GetAttributeOfType<ExpressionAttribute>()?.Expression!;
                            query = query.Where(string.Format(CultureInfo.InvariantCulture, expression, start), propertyValue);
                        }
                        var end = $"{propertyName}End";
                        if (typeof(TEntity).GetProperty(end) != null)
                        {
                            var expression = OperatorType.LessThanOrEqual.GetAttributeOfType<ExpressionAttribute>()?.Expression!;
                            query = query.Where(string.Format(CultureInfo.InvariantCulture, expression, end), propertyValue);
                        }
                    }
                    else
                    {
                        if (typeof(TEntity).GetProperty(propertyName) != null)
                        {
                            var expression = OperatorType.Equal.GetAttributeOfType<ExpressionAttribute>()?.Expression!;
                            query = query.Where(string.Format(CultureInfo.InvariantCulture, expression, propertyName), propertyValue);
                        }
                    }
                }
            }
        }
        return query;
    }
}
