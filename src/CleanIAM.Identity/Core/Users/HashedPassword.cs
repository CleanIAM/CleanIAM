namespace CleanIAM.Identity.Core.Users;

/// <summary>
/// Structure representation of a hashed password
/// </summary>
public class HashedPassword(byte[] hash, byte[] salt, string hashAlgorithmSignature)

{
    /// <summary>
    /// Hash of the password
    /// </summary>
    public byte[] Hash { get; set; } = hash;

    /// <summary>
    /// Salt used to create the hash
    /// </summary>
    public byte[] Salt { get; set; } = salt;


    /// <summary>
    /// Algorithm signature to identify the hashing algorithm used to create the hash
    /// </summary>
    /// <remarks>
    /// If migrating application to a new algorithm, this value will help with the seamless migration
    /// </remarks>
    public string HashAlgorithmSignature { get; set; } = hashAlgorithmSignature;
}