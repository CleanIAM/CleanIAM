using System.Security.Cryptography;
using System.Text;
using Identity.Infrastructure;
using NSec.Cryptography;

namespace Identity.Application.Services;

public class PasswordService : IPasswordService
{
    private readonly Argon2id _argon;

    public PasswordService()
    {
        // Configuration for `Argon2id` algorithm is based on OWASP recommendations:
        // https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html
        var argon = PasswordBasedKeyDerivationAlgorithm.Argon2id(new Argon2Parameters()
        {
            MemorySize = 19 * 1024 * 1024, // 19MB
            NumberOfPasses = 2, // iteration count of 2
            DegreeOfParallelism = 1 // Single degree of parallelism
        });
        _argon = argon;
    }

    public bool Compare(string password, PasswordHash hash)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        var hashBytes = _argon.DeriveBytes(passwordBytes, hash.Salt, 128);

        return hashBytes.SequenceEqual(hash.Hash);
    }

    public PasswordHash Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(32);

        var passwordBytes = Encoding.UTF8.GetBytes(password);

        var hash = _argon.DeriveBytes(passwordBytes, salt, 128);

        return new PasswordHash(hash, salt);
    }
}