using SharedKernel.Core.Users;

namespace Identity.Application.Interfaces;

/// <summary>
/// Interface for password hashing
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Compare password with hash
    /// </summary>
    /// <param name="password">password as a string</param>
    /// <param name="hash">Password hash to compare the password to</param>
    /// <returns>`True` if password matches the hast, `False` otherwise</returns>
    bool Compare(string password, HashedPassword hash);

    /// <summary>
    /// Hash password
    /// </summary>
    /// <param name="password">Password to hash</param>
    /// <returns>Hashed password</returns>
    HashedPassword Hash(string password);
}