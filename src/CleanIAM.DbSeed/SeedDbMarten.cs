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

        var session = scope.ServiceProvider.GetRequiredService<IDocumentSession>();


        SeedUsers(session);
        SeedDefaultTenant(session);

        await session.SaveChangesAsync();
    }


    private static void SeedDefaultTenant(IDocumentSession session)
    {
        // Generate default tenant
        var defaultTenant = new Tenant
        {
            Id = SharedKernelConstants.DefaultTenantId,
            Name = "Default Tenant"
        };

        session.Store(defaultTenant);
    }

    private static void SeedUsers(IDocumentSession session)
    {
        var passwordHasher = new PasswordHasher();

        session.Store(
            new IdentityUser
            {
                Id = Guid.NewGuid(),
                FirstName = "Master",
                LastName = "Admin",
                Email = "master@admin.com",
                HashedPassword = passwordHasher.Hash("Asdfasdf1."),
                IsInvitePending = false,
                EmailVerified = true,
                Roles = [UserRole.User, UserRole.Admin, UserRole.MasterAdmin],
                TenantId = Guid.Empty,
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
                Id = Guid.NewGuid(),
                FirstName = "Master",
                LastName = "Admin",
                Email = "master@admin.com",
                IsInvitePending = false,
                EmailVerified = true,
                Roles = [UserRole.User, UserRole.Admin, UserRole.MasterAdmin],
                TenantId = Guid.Empty,
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