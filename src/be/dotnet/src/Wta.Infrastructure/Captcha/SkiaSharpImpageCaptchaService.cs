using System.Security.Cryptography;
using SkiaSharp;

namespace Wta.Infrastructure.Captcha;

[Service<IImpageCaptchaService>]
public class SkiaSharpImpageCaptchaService : IImpageCaptchaService
{
    public byte[] Create(string code)
    {
        using var fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Wta.Infrastructure.Resources.carlito.ttf");
        var typeface = SKFontManager.Default.CreateTypeface(fontStream);
        var height = 30;
        var info = new SKImageInfo(100, height);
        using var surface = SKSurface.Create(info);
        var canvas = surface.Canvas;
        canvas.Clear(SKColor.Parse("#efefef"));

        for (var i = 0; i < code.Length; i++)
        {
            var fontSize = RandomNumberGenerator.GetInt32(15, 30);
            using var font = new SKFont(typeface, fontSize);
            using var paint = new SKPaint
            {
                Color = GetRandomColor(),
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
            };
            canvas.DrawText(code[i].ToString(), i * RandomNumberGenerator.GetInt32(20, 25), (height + fontSize) / 2, font, paint);
        }
        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = new MemoryStream();
        data.SaveTo(stream);
        return stream.ToArray();
    }

    private static SKColor GetRandomColor()
    {
        return SKColor.FromHsl(RandomNumberGenerator.GetInt32(361),
                RandomNumberGenerator.GetInt32(101) * 0.01f,
                RandomNumberGenerator.GetInt32(101) * 0.01f,
                (byte)RandomNumberGenerator.GetInt32(55, 255)
                );
    }
}
