namespace Wta.Infrastructure.Application.Configuration;

[Options]
public class EmailOptions
{
    public string Host { get; set; } = default!;
    public int Port { get; set; }
    public bool EnableSsl { get; set; }
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
}
