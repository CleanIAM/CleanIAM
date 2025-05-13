using CleanIAM.Identity.Core.Users;
using CleanIAM.Identity.Infrastructure.Services;
using CleanIAM.SharedKernel;
using Marten;
using CleanIAM.SharedKernel.Core;
using CleanIAM.Tenants.Core;
using CleanIAM.Users.Core;
using MfaConfig = CleanIAM.Identity.Core.Users.MfaConfig;

namespace DbConfig;

public static class SeedDbMarten
{
    public static async Task SeedMartenDb(this IApplicationBuilder app)
    {
        await using var scope = app.ApplicationServices.CreateAsyncScope();

        var store = scope.ServiceProvider.GetRequiredService<IDocumentStore>();

        var defaultTenantSession = store.LightweightSession(SharedKernelConstants.DefaultTenantId.ToString());

        await SeedUsers(defaultTenantSession);
        await SeedDefaultTenant(defaultTenantSession);

        await defaultTenantSession.SaveChangesAsync();
    }


    private static async Task SeedDefaultTenant(IDocumentSession session)
    {

        // Generate default tenant
        var newDefaultTenant = new Tenant
        {
            Id = SharedKernelConstants.DefaultTenantId,
            Name = "Default Tenant"
        };

        session.Store(newDefaultTenant);
    }

    private static async Task SeedUsers(IDocumentSession session)
    {
        var passwordHasher = new PasswordHasher();

        session.Store(
            new IdentityUser
            {
                Id = SharedKernelConstants.DefaultTenantId,
                FirstName = "Master",
                LastName = "Admin",
                Email = "master@admin.com",
                HashedPassword = passwordHasher.Hash("Asdfasdf1."),
                IsInvitePending = false,
                EmailVerified = true,
                Roles = [UserRole.User, UserRole.Admin, UserRole.MasterAdmin],
                TenantId = SharedKernelConstants.DefaultTenantId,
                TenantName = "Default Tenant",
                IsMFAEnabled = false,
                MfaConfig = new MfaConfig
                {
                    IsMfaConfigured = false,
                    TotpSecretKey = ""
                },
            }
        );
        session.Store(
            new User
            {
                Id = SharedKernelConstants.DefaultTenantId,
                FirstName = "Master",
                LastName = "Admin",
                Email = "master@admin.com",
                IsInvitePending = false,
                EmailVerified = true,
                Roles = [UserRole.User, UserRole.Admin, UserRole.MasterAdmin],
                TenantId = SharedKernelConstants.DefaultTenantId,
                TenantName = "Default Tenant",
                IsMFAEnabled = false,
                MfaConfig = new CleanIAM.Users.Core.MfaConfig
                {
                    IsMfaConfigured = false,
                    TotpSecretKey = ""
                },
            }
        );
    }
}