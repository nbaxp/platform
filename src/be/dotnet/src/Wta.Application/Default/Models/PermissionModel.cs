namespace Wta.Application.Default.Models;

[DependsOn<Permission>]
public class PermissionModel
{
    public Guid? Id { get; set; }

    /// <summary>
    /// 类型：group\menu\button(Vue Router Meta Type)
    /// </summary>
    public MenuType? Type { get; set; }

    /// <summary>
    /// 权限或菜单编号(group和menu 对应 Vue Router Path，buton 对应权限标识)
    /// </summary>
    public string? Number { get; set; }

    /// <summary>
    /// Vue Router Meta Title
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Vue Router Redirect
    /// </summary>
    public string? Redirect { get; set; }

    /// <summary>
    /// 前端组件(Vue Router Component)
    /// </summary>
    public string? Component { get; set; }

    /// <summary>
    /// 是否隐藏菜单或按钮
    /// </summary>
    public bool? Hidden { get; set; }

    /// <summary>
    /// 不缓存
    /// </summary>
    public bool? NoCache { get; set; }

    /// <summary>
    /// Vue Router Meta Icon
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 按钮类型：table、row
    /// </summary>
    public ButtonType? ButtonType { get; set; }

    /// <summary>
    /// 按钮 html class
    /// </summary>
    public string? ButtonClass { get; set; }

    /// <summary>
    /// 请求方法
    /// </summary>
    public string? ApiMethod { get; set; }

    public string? Command { get; set; }

    public string? Schema { get; set; }

    /// <summary>
    /// 请求地址
    /// </summary>
    public string? ApiUrl { get; set; }

    public int? Order { get; set; }
    public bool Disabled { get; set; }

    public Guid? ParentId { get; set; }
}
