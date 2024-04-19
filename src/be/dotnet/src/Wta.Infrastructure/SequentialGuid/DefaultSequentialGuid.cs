using System.Security.Cryptography;

namespace Wta.Infrastructure.SequentialGuid;

/// <summary>
/// https://www.codeproject.com/Articles/388157/GUIDs-as-fast-primary-keys-under-multiple-database
/// https://github.com/jhtodd/SequentialGuid/blob/master/SequentialGuid/Classes/SequentialGuid.cs
/// </summary>
[Service<ISequentialGuid>(ServiceLifetime.Singleton)]
public class DefaultSequentialGuid : ISequentialGuid
{
    private static readonly RandomNumberGenerator RandomGenerator = RandomNumberGenerator.Create();

    public Guid Create(string guidType)
    {
        byte[] randomBytes = new byte[10];
        RandomGenerator.GetBytes(randomBytes);
        long timestamp = DateTime.UtcNow.Ticks / 10000L;

        byte[] timestampBytes = BitConverter.GetBytes(timestamp);

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(timestampBytes);
        }

        byte[] guidBytes = new byte[16];
        switch (guidType)
        {
            case "SequentialAsString":
            case "SequentialAsBinary":
                Buffer.BlockCopy(timestampBytes, 2, guidBytes, 0, 6);
                Buffer.BlockCopy(randomBytes, 0, guidBytes, 6, 10);

                if (guidType == "SequentialAsString" && BitConverter.IsLittleEndian)
                {
                    Array.Reverse(guidBytes, 0, 4);
                    Array.Reverse(guidBytes, 4, 2);
                }
                break;

            case "SequentialAtEnd":
                Buffer.BlockCopy(randomBytes, 0, guidBytes, 0, 10);
                Buffer.BlockCopy(timestampBytes, 2, guidBytes, 10, 6);
                break;
        }
        return new Guid(guidBytes);
    }
}
