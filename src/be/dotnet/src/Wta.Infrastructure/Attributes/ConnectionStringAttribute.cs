namespace Wta.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ConnectionStringAttribute : Attribute
{
    public ConnectionStringAttribute(string connectionString) => ConnectionString = connectionString;

    public string ConnectionString { get; }
}
