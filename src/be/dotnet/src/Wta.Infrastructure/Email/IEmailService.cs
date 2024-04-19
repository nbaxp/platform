namespace Wta.Infrastructure.Email;

public interface IEmailService
{
    void Send(string subject, string body, string toName, string toAddress);
}
