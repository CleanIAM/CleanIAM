using System.Security.Cryptography;
using System.Text;
using Identity.Application.Interfaces;
using NSec.Cryptography;
using SharedKernel.Core.Users;

namespace Identity.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    // https://www.rfc-editor.org/rfc/rfc9106.html#name-parameter-choice
    private const int SaltSize = 16;
    private const int HashSize = 128;

    // Configuration for `Argon2id` algorithm is based on OWASP recommendations:
    // https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html
    private const int MemorySize = 19456; // 19MB (value is in kibibytes)
    private const int NumberOfPasses = 2; // iteration count of 2
    private const int DegreeOfParallelism = 1; // Single degree of parallelism

    private const int AlgorithmVersion = 0x13; // Argon2id version 0x13
    private readonly Argon2id _argon;


    public PasswordHasher()
    {
        var argon = PasswordBasedKeyDerivationAlgorithm.Argon2id(new Argon2Parameters
        {
            MemorySize = MemorySize,
            NumberOfPasses = NumberOfPasses,
            DegreeOfParallelism = DegreeOfParallelism
        });
        _argon = argon;
    }

    public HashedPassword Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        var passwordBytes = Encoding.UTF8.GetBytes(password);

        var hash = _argon.DeriveBytes(passwordBytes, salt, 128);

        var hashAlgorithmSignature =
            $"$argon2id$v={AlgorithmVersion}$m={MemorySize},t={NumberOfPasses},p={DegreeOfParallelism}";

        return new HashedPassword(hash, salt, hashAlgorithmSignature);
    }

    public bool Compare(string password, HashedPassword hash)
    {
        var hashAlgorithmSignature =
            $"$argon2id$v={AlgorithmVersion}$m={MemorySize},t={NumberOfPasses},p={DegreeOfParallelism}";

        // NOTE: If new the hash algorithm is used, here is the place to check it
        if (hash.HashAlgorithmSignature != hashAlgorithmSignature)
            throw new InvalidOperationException(
                $"Hash algorithm signature mismatch. Expected: {hashAlgorithmSignature}, Actual: {hash.HashAlgorithmSignature}");

        var passwordBytes = Encoding.UTF8.GetBytes(password);

        var hashBytes = _argon.DeriveBytes(passwordBytes, hash.Salt, 128);
        return CryptographicOperations.FixedTimeEquals(hashBytes, hash.Hash);
    }
}