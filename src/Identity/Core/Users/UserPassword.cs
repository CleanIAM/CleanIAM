namespace Identity.Core.Users;

public class HashedPassword
{
    public byte[] Hash { get; set; }
    public byte[] Salt { get; set; }
}