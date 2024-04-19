namespace Wta.Infrastructure.Captcha;

public interface IImpageCaptchaService
{
    byte[] Create(string code);
}
