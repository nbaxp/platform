namespace Wta.Infrastructure.Sms;

public interface ISmsService
{
    void Send(string phoneNumber, out string code);
}
