using System.Security.Cryptography;

namespace TourApi.Services;

// Packs a 16-byte random salt + 48-byte PBKDF2 derived key into exactly
// 64 bytes, matching the `binary(64)` PasswordHash column in the schema.
public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 48;
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public byte[] Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);

        var result = new byte[SaltSize + KeySize];
        Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
        Buffer.BlockCopy(key, 0, result, SaltSize, KeySize);
        return result;
    }

    public bool Verify(string password, byte[] storedHash)
    {
        if (storedHash.Length != SaltSize + KeySize) return false;

        var salt = storedHash[..SaltSize];
        var expectedKey = storedHash[SaltSize..];
        var actualKey = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);

        return CryptographicOperations.FixedTimeEquals(expectedKey, actualKey);
    }
}