using Wta.Infrastructure.Scheduling;

namespace Wta.Application.Default.Data;

public class DefaultDbSeeder(IActionDescriptorCollectionProvider actionProvider, IEncryptionService encryptionService, ITenantService tenantService) : IDbSeeder<DefaultDbContext>
{
    public void Seed(DefaultDbContext context)
    {
        //添加定时任务
        InitJob(context);
        //添加字典
        InitDict(context);
        //添加部门
        InitDepartment(context);
        //添加权限
        InitPermission(context);
        //添加角色
        InitRole(context);
        //添加用户
        InitUser(context);
    }

    private static void InitJob(DefaultDbContext context)
    {
        AppDomain.CurrentDomain.GetCustomerAssemblies()
            .SelectMany(o => o.GetTypes())
            .Where(o => o.IsClass && !o.IsAbstract && o.IsAssignableTo(typeof(IScheduledTask)) && o.HasAttribute<CronAttribute>())
            .ForEach(o =>
            {
                context.Set<Job>().Add(new Job { Name = o.GetDisplayName(), Type = o.FullName!, Cron = o.GetCustomAttribute<CronAttribute>()!.Cron });
            });
        context.SaveChanges();
    }

    private static void InitDepartment(DbContext context)
    {
        context.Set<Department>().Add(new Department
        {
            Id = context.NewGuid(),
            Name = "机构",
            Number = "Organ",
            Children = [
                    new Department
                    {
                        Id = context.NewGuid(),
                        Name = "技术部",
                        Number = "Technology"
                    }
                ]
        }.UpdateNode());
        context.SaveChanges();
    }

    private static void InitDict(DbContext context)
    {
        context.Set<Dict>().Add(new Dict
        {
            Id = context.NewGuid(),
            Name = "语言",
            Number = "language",
            Children = new List<Dict>
            {
                new Dict
                {
                    Id= context.NewGuid(),
                    Name="简体中文",
                    Number="zh-CN"
                },
                new Dict
                {
                    Id= context.NewGuid(),
                    Name="English",
                    Number="en-US"
                }
            }
        }.UpdateNode());
    }

    private void InitPermission(DbContext context)
    {
        var list = new List<Permission>();
        // 添加菜单分组
        var groups = AppDomain.CurrentDomain.GetCustomerAssemblies()
            .SelectMany(o => o.GetTypes())
            .Where(o => o.IsClass && !o.IsAbstract && o.IsAssignableTo(typeof(GroupAttribute)))
            .ToList();
        /// 添加菜单分组
        groups.ForEach(groupType =>
        {
            var number = groupType.Name.ToSlugify()!;
            if (!list.Any(o => o.Number == number))
            {
                list.Add(new Permission
                {
                    Id = context.NewGuid(),
                    Type = MenuType.Group,
                    Authorize = "Anonymous",
                    Name = groupType.GetDisplayName(),
                    Number = groupType.FullName?.TrimEnd("Attribute")!,
                    RouterPath = groupType.Name.TrimEnd("Attribute").ToSlugify()!,
                    Icon = groupType.GetCustomAttribute<IconAttribute>()?.Icon ?? "folder",
                    Order = groupType.GetCustomAttribute<DisplayAttribute>()?.GetOrder() ?? 0
                });
            }
        });
        /// 设置分组上级
        groups.ForEach(groupType =>
        {
            var number = groupType.Name.ToSlugify()!;
            var group = list.FirstOrDefault(o => o.Number == number);
            var current = group;
            groupType.GetBaseClasses().Where(o => !o.IsAbstract).ForEach(type =>
            {
                if (current != null)
                {
                    var number = type.FullName!;
                    current.ParentId = list.FirstOrDefault(o => o.Number == number)?.Id;
                    current = list.FirstOrDefault(o => o.Number == number);
                }
            });
        });
        //添加首页
        list.Add(new Permission
        {
            Id = context.NewGuid(),
            Type = MenuType.Menu,
            Authorize = "Anonymous",
            Name = "首页",
            Number = "Home",
            RouterPath = "home",
            Component = "home",
            Icon = "home",
            NoCache = true,
            Order = 1
        });
        //添加用户中心
        var userCenterGroup = new Permission
        {
            Id = context.NewGuid(),
            Type = MenuType.Group,
            Authorize = "Authenticated",
            Name = "用户中心",
            Number = "UserCenter",
            RouterPath = "user-center",
            Icon = "user",
            Order = 2,
        };
        list.Add(userCenterGroup);
        //添加修改密码菜单
        list.Add(new Permission
        {
            ParentId = userCenterGroup.Id,
            Id = context.NewGuid(),
            Type = MenuType.Menu,
            Authorize = "Authenticated",
            Name = "修改密码",
            Number = "ResetPasswrod",
            RouterPath = "reset-password",
            Component = "reset-password",
            Order = 1
        });
        //添加资源菜单和资源操作按钮
        var order = 1;
        var actionDescriptors = actionProvider.ActionDescriptors.Items;
        AppDomain.CurrentDomain.GetCustomerAssemblies()
            .SelectMany(o => o.GetTypes())
            .Where(o => !o.IsAbstract && o.IsAssignableTo(typeof(IResource)))
            .ForEach(resourceType =>
            {
                if (tenantService.TenantNumber != null && resourceType == typeof(Tenant))
                {
                    return;
                }
                // 菜单
                var resourceServiceType = typeof(IResourceService<>).MakeGenericType(resourceType);
                var resourcePermission = new Permission
                {
                    Id = context.NewGuid(),
                    Type = MenuType.Menu,
                    Authorize = "Authenticated",
                    Name = resourceType.GetDisplayName(),
                    Number = resourceType.FullName!,
                    RouterPath = resourceType.Name.ToSlugify()!,
                    Component = "list",
                    Schema = $"{resourceType.Name.ToSlugify()}",
                    Order = resourceType.GetCustomAttribute<DisplayAttribute>()?.GetOrder() ?? order++
                };
                // 按钮
                actionDescriptors
                .Select(o => (o as ControllerActionDescriptor)!)
                .Where(o => o != null && o.ControllerTypeInfo.AsType().IsAssignableTo(resourceServiceType) && !o.MethodInfo.GetCustomAttributes<IgnoreAttribute>().Any())
                .ForEach(descriptor =>
                {
                    if (descriptor.ControllerTypeInfo.AsType().GetCustomAttribute<ViewAttribute>()?.Component is string component)
                    {
                        resourcePermission.Component = component;
                    }
                    var number = $"{descriptor.ControllerName}.{descriptor.ActionName}";

                    list.Add(new Permission
                    {
                        ParentId = resourcePermission.Id,
                        Id = context.NewGuid(),
                        Type = MenuType.Button,
                        Authorize = number,
                        Name = (descriptor.MethodInfo.GetCustomAttribute<DisplayAttribute>()?.Name ?? descriptor.ActionName).ToLowerCamelCase(),
                        Number = number,
                        RouterPath = number,
                        Url = descriptor.AttributeRouteInfo?.Template,
                        Method = (descriptor.ActionConstraints?.FirstOrDefault() as HttpMethodActionConstraint)?.HttpMethods.FirstOrDefault(),
                        Command = descriptor.ActionName.ToSlugify(),
                        ButtonType = descriptor.MethodInfo.GetCustomAttribute<ButtonAttribute>()?.Type ?? ButtonType.Table,
                        Hidden = descriptor.MethodInfo.GetCustomAttribute<HiddenAttribute>() == null ? false : true,
                        Order = descriptor.MethodInfo.GetCustomAttribute<DisplayAttribute>()?.GetOrder() ?? 0
                    });
                });
                // 设置菜单分组
                var groupAttribute = resourceType.GetCustomAttributes().FirstOrDefault(o => o.GetType().IsAssignableTo(typeof(GroupAttribute)));
                if (groupAttribute != null && groupAttribute is GroupAttribute group)
                {
                    var number = group.GetType().FullName?.TrimEnd("Attribute")!;
                    var groupPermission = list.FirstOrDefault(o => o.Number == number);
                    if (groupPermission != null)
                    {
                        resourcePermission.ParentId = groupPermission.Id;
                    }
                }
                list.Add(resourcePermission);
            });
        list.AsQueryable()
            .Cast<BaseTreeEntity<Permission>>()
            .ToList()
            .ToTree()
            .Cast<Permission>()
            .ForEach(o =>
            {
                o.UpdateNode();
                context.Set<Permission>().Add(o);
            });
        context.SaveChanges();
    }

    private void InitRole(DbContext context)
    {
        var permisions = context.Set<Permission>().ToList();
        if (tenantService.TenantNumber != null)
        {
            permisions.Where(o => !o.Disabled && !tenantService.Permissions.Contains(o.Number)).ForEach(o => o.Disabled = true);
            permisions = permisions.Where(o => tenantService.Permissions.Contains(o.Number)).ToList();
        }
        context.Set<Role>().Add(new Role
        {
            Id = context.NewGuid(),
            Name = tenantService.TenantNumber != null ? "租户管理员" : "管理员",
            Number = "admin",
            RolePermissions = permisions!.Select(o => new RolePermission
            {
                PermissionId = o.Id,
                IsReadOnly = true
            }).ToList()
        });
        context.SaveChanges();
    }

    private void InitUser(DbContext context)
    {
        var userName = "admin";
        var password = "123456";
        var salt = encryptionService.CreateSalt();
        var passwordHash = encryptionService.HashPassword(password, salt);

        var userId = context.NewGuid();

        context.Set<User>().Add(new User
        {
            Id = userId,
            Name = tenantService.TenantNumber != null ? "租户管理员" : "管理员",
            UserName = userName,
            Avatar = "api/file/avatar.svg",
            NormalizedUserName = userName.ToUpperInvariant(),
            SecurityStamp = salt,
            PasswordHash = passwordHash,
            IsReadOnly = true,
            UserRoles = new List<UserRole> {
                new UserRole
                {
                    UserId=userId,
                    RoleId = context.Set<Role>().First(o=>o.Number=="admin").Id,
                    IsReadOnly = true
                }
            }
        });
        context.SaveChanges();
    }
}
