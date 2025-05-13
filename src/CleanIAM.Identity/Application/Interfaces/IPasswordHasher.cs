using HashedPassword = CleanIAM.Identity.Core.Users.HashedPassword;
using Users_HashedPassword = CleanIAM.Identity.Core.Users.HashedPassword;

namespace CleanIAM.Identity.Application.Interfaces;

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
    bool Compare(string password, Users_HashedPassword hash);

    /// <summary>
    /// Hash password
    /// </summary>
    /// <param name="password">Password to hash</param>
    /// <returns>Hashed password</returns>
    Users_HashedPassword Hash(string password);
}