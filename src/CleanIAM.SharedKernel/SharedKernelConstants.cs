namespace CleanIAM.SharedKernel;

/// <summary>
/// This class contains constants used throughout the application.
/// </summary>
public class SharedKernelConstants
{
    /// <summary>
    /// The name of the claim used to store the tenant id, for the purpose of multi-tenancy
    /// </summary>
    public static readonly string TenantClaimName = "tenant";

    /// <summary>
    /// The name of issuer user for MFA
    /// </summary>
    public static readonly string MfaIssuer = "CleanIAM";

    /// <summary>
    /// Id of the dafult tenant 
    /// </summary>
    /// <remarks>Cannot be empty Guid since marten doesnt allow to have element with Enpty guid id</remarks>
    public static readonly Guid DefaultTenantId = new Guid("00000000-0000-0000-0000-000000000001");
}