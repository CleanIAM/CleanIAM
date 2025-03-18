namespace Identity.Infrastructure;

public record PasswordHash(byte[] Hash, byte[] Salt);

public interface IPasswordService
{
    bool Compare(string password, PasswordHash hash);
    PasswordHash Hash(string password);
}