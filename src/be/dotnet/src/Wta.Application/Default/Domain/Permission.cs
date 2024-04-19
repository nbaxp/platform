namespace Wta.Application.Default.Domain;

/// <summary>
/// name=>meta.title,number=>meta.path
/// </summary>
[System, Display(Name = "权限", Order = 4)]
public class Permission : BaseTreeEntity<Permission>
{
    /// <summary>
    /// Vue Router Meta
    /// "Anonymous"
    /// "Authenticated"
    /// Roles:"['role1','role2']"
    /// "Permission"
    /// </summary>
    public string Authorize { get; set; } = default!;

    /// <summary>
    /// Vue Router Meta,按钮类型：table、row
    /// </summary>
    public ButtonType? ButtonType { get; set; }

    /// <summary>
    /// Vue Router Meta,按钮 html class
    /// </summary>
    public string? ClassList { get; set; }

    /// <summary>
    /// Vue Router Meta
    /// </summary>
    public string? Command { get; set; }

    /// <summary>
    /// Vue Router
    /// </summary>
    public string? Component { get; set; }

    /// <summary>
    /// 禁用，不可赋予角色
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Vue Router Meta,是否隐藏菜单或按钮
    /// </summary>
    public bool Hidden { get; set; }

    /// <summary>
    /// Vue Router Meta
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Vue Router Meta,请求方法
    /// </summary>
    public string? Method { get; set; }

    /// <summary>
    /// Vue Router Meta
    /// </summary>
    public bool NoCache { get; set; }

    /// <summary>
    /// Vue Router
    /// </summary>
    public string? Redirect { get; set; }

    /// <summary>
    /// 角色权限
    /// </summary>
    public List<RolePermission> RolePermissions { get; set; } = [];

    public string RouterPath { get; set; } = default!;

    /// <summary>
    /// Vue Router Meta
    /// </summary>
    public string? Schema { get; set; }

    /// <summary>
    /// Vue Router Meta,group\menu\button)
    /// </summary>
    public MenuType Type { get; set; }
    /// <summary>
    /// Vue Router Meta,请求地址
    /// </summary>
    public string? Url { get; set; }
}
