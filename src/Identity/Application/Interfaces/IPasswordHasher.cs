namespace Identity.Application.Interfaces;

public record PasswordHash(byte[] Hash, byte[] Salt);

public interface IPasswordHasher
{
    bool Compare(string password, PasswordHash hash);
    PasswordHash Hash(string password);
}