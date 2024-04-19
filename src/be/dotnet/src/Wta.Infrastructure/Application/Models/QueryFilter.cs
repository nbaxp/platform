using System.Linq.Expressions;

namespace Wta.Infrastructure.Application.Models;

public class QueryFilter
{
    public string Property { get; set; } = default!;
    public object? Value { get; set; } = default!;
    public string Operator { get; set; } = default!;
    public string Logic { get; set; } = default!;
    public List<QueryFilter> Children = new List<QueryFilter>();

    public Expression<Func<T, bool>>? ToExpression<T>()
    {
        var propertyType = typeof(T).GetProperty(Property, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)?.PropertyType!;//属性类型
        if (Value != null && propertyType != null)
        {
            var parameter = Expression.Parameter(typeof(T), "p");//参数
            var property = Expression.PropertyOrField(parameter, Property);//字段或属性
            var valueType = Operator == "in" ? typeof(List<>).MakeGenericType(propertyType) : propertyType;
            var constant = Operator == "in" ? Expression.Constant(JsonSerializer.Deserialize(Value?.ToString()!, valueType), valueType) : Expression.Constant(Value!.ToString().GetValue(propertyType), valueType); //常量
            //比较运算
            if (Operator == "=")
            {
                //单选
                return Expression.Lambda<Func<T, bool>>(Expression.Equal(property, constant), parameter);
            }
            else if (Operator == "!=")
            {
                return Expression.Lambda<Func<T, bool>>(Expression.NotEqual(property, constant), parameter);
            }
            else if (Operator == ">")
            {
                return Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(property, constant), parameter);
            }
            else if (Operator == ">=")
            {
                return Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(property, constant), parameter);
            }
            else if (Operator == "<")
            {
                return Expression.Lambda<Func<T, bool>>(Expression.LessThan(property, constant), parameter);
            }
            else if (Operator == "<=")
            {
                return Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(property, constant), parameter);
            }
            else if (Operator == "like")
            {
                //字符串
                var methodInfo = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)]);
                var methodCall = Expression.Call(property, methodInfo!, constant);
                return Expression.Lambda<Func<T, bool>>(methodCall, parameter);
            }
            else if (Operator == "in")
            {
                //多选
                var methodInfo = valueType.GetMethod(nameof(string.Contains));
                var methodCall = Expression.Call(constant, methodInfo!, property);
                return Expression.Lambda<Func<T, bool>>(methodCall, parameter);
            }
        }
        return null;
    }

    public static Expression<Func<TEntity, bool>>? ToExpression<TEntity>(List<QueryFilter> filters)
    {
        Expression<Func<TEntity, bool>>? result = null;
        foreach (var filter in filters)
        {
            if (filter.Children.Any())
            {
                Expression<Func<TEntity, bool>>? expression = null;
                foreach (var subFilter in filter.Children)
                {
                    var subExpresson = subFilter.ToExpression<TEntity>();
                    if (subExpresson != null)
                    {
                        if (subFilter.Logic == "and")
                        {
                            expression = expression == null ? subExpresson : expression.And(subExpresson);
                        }
                        else
                        {
                            expression = expression == null ? subExpresson : expression.Or(subExpresson);
                        }
                    }
                }
                if (expression != null)
                {
                    if (filter.Logic == "and")
                    {
                        result = result == null ? expression : result.And(expression);
                    }
                    else
                    {
                        result = result == null ? expression : result.Or(expression);
                    }
                }
            }
            else
            {
                var itemExpresson = filter.ToExpression<TEntity>();
                if (itemExpresson != null)
                {
                    if (filter.Logic == "and")
                    {
                        result = result == null ? itemExpresson : result.And(itemExpresson);
                    }
                    else
                    {
                        result = result == null ? itemExpresson : result.Or(itemExpresson);
                    }
                }
            }
        }
        return result;
    }
}
