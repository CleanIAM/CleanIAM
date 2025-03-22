using System.Security.Cryptography;
using System.Text;
using Identity.Infrastructure;
using NSec.Cryptography;

namespace Identity.Application.Services;

public class PasswordService : IPasswordService
{
    private readonly Argon2id _argon;

    // https://www.rfc-editor.org/rfc/rfc9106.html#name-parameter-choice
    private const int SaltSize = 16;
    private const int HashSize = 128;

    public PasswordService()
    {
        // Configuration for `Argon2id` algorithm is based on OWASP recommendations:
        // https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html
        var argon = PasswordBasedKeyDerivationAlgorithm.Argon2id(new Argon2Parameters()
        {
            MemorySize = 19456, // 19MB (value is in kibibytes)
            NumberOfPasses = 2, // iteration count of 2
            DegreeOfParallelism = 1 // Single degree of parallelism
        });
        _argon = argon;
    }

    public PasswordHash Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        var passwordBytes = Encoding.UTF8.GetBytes(password);

        var hash = _argon.DeriveBytes(passwordBytes, salt, 128);

        return new PasswordHash(hash, salt);
    }

    public bool Compare(string password, PasswordHash hash)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        var hashBytes = _argon.DeriveBytes(passwordBytes, hash.Salt, 128);
        return CryptographicOperations.FixedTimeEquals(hashBytes, hash.Hash);
    }
}