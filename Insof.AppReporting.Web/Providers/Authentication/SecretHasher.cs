using System;
using System.Security.Cryptography;

namespace Insof.AppReporting.Web.Providers.Authentication;

public interface ISecretHasher
{
    string Hash(string input);
    bool Verify(string input, string hashString);
}

public sealed class SecretHasher : ISecretHasher
{
    private const int SaltSize = 128 / 8;
    private const int KeySize = 512 / 8;
    private const int Iterations = 75_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;

    private const char SegmentDelimiter = ':';

    public string Hash(string input)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            input,
            salt,
            Iterations,
            Algorithm,
            KeySize
        );
        return string.Join(
            SegmentDelimiter,
            Convert.ToBase64String(hash),
            Convert.ToBase64String(salt),
            Iterations,
            Algorithm
        );
    }

    public bool Verify(string input, string hashString)
    {
        var segments = hashString.Split(SegmentDelimiter);
        var hash = Convert.FromBase64String(segments[0]);
        var salt = Convert.FromBase64String(segments[1]);
        var iterations = int.Parse(segments[2]);
        var algorithm = new HashAlgorithmName(segments[3]);
        var inputHash = Rfc2898DeriveBytes.Pbkdf2(
            input,
            salt,
            iterations,
            algorithm,
            hash.Length
        );
        return CryptographicOperations.FixedTimeEquals(inputHash, hash);
    }
}
