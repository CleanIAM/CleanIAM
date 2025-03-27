namespace Identity.Core.Users;

public class HashedPassword(byte[] hash, byte[] salt)

{
    public byte[] Hash { get; set; } = hash;
    public byte[] Salt { get; set; } = salt;
}