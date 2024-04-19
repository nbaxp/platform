using System.Security.Cryptography;

namespace Wta.Infrastructure.Security;

[Service<IEncryptionService>]
public class DefaultEncryptionService(IConfiguration configuration) : IEncryptionService
{
    public string CreateSalt()
    {
        using var rng = RandomNumberGenerator.Create();
        var buff = new byte[5];
        rng.GetBytes(buff);
        return Convert.ToBase64String(buff);
    }

    public string EncryptText(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
        {
            return plainText;
        }

        using var provider = GetEncryptionAlgorithm();
        var encryptedBinary = EncryptTextToMemory(plainText, provider);

        return Convert.ToBase64String(encryptedBinary);
    }

    public string DecryptText(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
        {
            return cipherText;
        }

        using var provider = GetEncryptionAlgorithm();

        var buffer = Convert.FromBase64String(cipherText);
        return DecryptTextFromMemory(buffer, provider);
    }

    public string HashPassword(string password, string salt)
    {
        var data = Encoding.UTF8.GetBytes(string.Concat(password, salt));
        var algorithm = CryptoConfig.CreateFromName("SHA256") as HashAlgorithm;
        return BitConverter.ToString(algorithm!.ComputeHash(data)).Replace("-", string.Empty);
    }

    private SymmetricAlgorithm GetEncryptionAlgorithm()
    {
        var encryptionKey = configuration.GetValue("EncryptionKey", "0123456789abcdef0123456789abcdef");

        if (string.IsNullOrEmpty(encryptionKey))
        {
            throw new ArgumentNullException(nameof(encryptionKey));
        }

        SymmetricAlgorithm provider = Aes.Create(); //: TripleDES.Create();

        var vectorBlockSize = provider.BlockSize / 8;

        provider.Key = Encoding.ASCII.GetBytes(encryptionKey[0..16]);
        provider.IV = Encoding.ASCII.GetBytes(encryptionKey[^vectorBlockSize..]);

        return provider;
    }

    private static byte[] EncryptTextToMemory(string data, SymmetricAlgorithm provider)
    {
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, provider.CreateEncryptor(), CryptoStreamMode.Write))
        {
            var toEncrypt = Encoding.Unicode.GetBytes(data);
            cs.Write(toEncrypt, 0, toEncrypt.Length);
            cs.FlushFinalBlock();
        }

        return ms.ToArray();
    }

    private static string DecryptTextFromMemory(byte[] data, SymmetricAlgorithm provider)
    {
        using var ms = new MemoryStream(data);
        using var cs = new CryptoStream(ms, provider.CreateDecryptor(), CryptoStreamMode.Read);
        using var sr = new StreamReader(cs, Encoding.Unicode);

        return sr.ReadToEnd();
    }
}
