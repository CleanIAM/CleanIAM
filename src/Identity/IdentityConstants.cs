namespace Identity;

public static class IdentityConstants
{
    /// <summary>
    /// Required delay between email verification emails
    /// </summary>
    public static readonly TimeSpan VerificationEmailDelay = TimeSpan.FromSeconds(30);

    /// <summary>
    /// The name of the claim used to store the signin request id, for the purpose of signin flow
    /// </summary>
    public static readonly string SigninRequestClaimName = "signin_request";
}