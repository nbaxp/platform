using Wta.Infrastructure.Application.Domain;

namespace Wta.Infrastructure.Extensions;

public static class EntityExtensions
{
    public static TEntity SetIdBy<TEntity>(this TEntity entity, Func<TEntity, object> expression)
       where TEntity : BaseEntity
    {
        entity.Id = $"{entity.TenantNumber},{expression.Invoke(entity)}".ToGuid();
        return entity;
    }

    public static List<BaseTreeEntity<T>> ToTree<T>(this List<BaseTreeEntity<T>> list) where T : BaseEntity
    {
        var tree = new List<BaseTreeEntity<T>>();
        foreach (var item in list)
        {
            if (!item.ParentId.HasValue)
            {
                tree.Add(item);
            }
            else
            {
                var parent = list.FirstOrDefault((node) => node.Id == item.ParentId);
                if (parent != null)
                {
                    parent.Children.Add((item as T)!);
                }
                else
                {
                    tree.Add(item);
                }
            }
        }
        return tree;
    }

    public static T UpdateNode<T>(this BaseTreeEntity<T> entity) where T : BaseEntity
    {
        entity.Path = $"{(entity.Parent as BaseTreeEntity<T>)?.Path}/{entity.Number}";
        if (entity.Children.Any())
        {
            entity.Children.ForEach(o =>
            {
                var node = o as BaseTreeEntity<T>;
                if (node!.Parent == null)
                {
                    node!.Parent = entity as T;
                }
                node!.UpdateNode();
            });
        }
        return (entity as T)!;
    }
}
