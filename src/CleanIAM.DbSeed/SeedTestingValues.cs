using System.Security.Claims;
using CleanIAM.Identity.Infrastructure.Services;
using CleanIAM.SharedKernel;
using CleanIAM.SharedKernel.Core;
using CleanIAM.SharedKernel.Core.Database;
using CleanIAM.Tenants.Core;
using CleanIAM.Users.Core;
using Mapster;
using Marten;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using IdentityUser = CleanIAM.Identity.Core.Users.IdentityUser;
using MfaConfig = CleanIAM.Identity.Core.Users.MfaConfig;

namespace DbConfig;

public static class SeedTestingValues
{
    private static readonly string TestTenant1Id = "00000000-0000-0000-0000-000000000011";
    private static readonly string TestTenant2Id = "00000000-0000-0000-0000-000000000022";

    private static readonly string TestUser1Id = "00000000-0000-0000-0000-000000000111";
    private static readonly string TestUser2Id = "00000000-0000-0000-0000-000000000222";
    private static readonly string TestAdmin1Id = "00000000-0000-0000-0000-000000000333";
    private static readonly string TestAdmin2Id = "00000000-0000-0000-0000-000000000444";
    private static readonly string TestMasterAdminId = "00000000-0000-0000-0000-000000000555";

    public static async Task SeedTesting(this IApplicationBuilder app)
    {
        await using var scope = app.ApplicationServices.CreateAsyncScope();

        // Seed EF Core
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureCreatedAsync();


        // Seed marten
        var store = scope.ServiceProvider.GetRequiredService<IDocumentStore>();
        
        await SeedTestTenants(store);
        await SeedTestUsers(store, scope);
    }


    /// <summary>
    /// Seed test tenants
    /// </summary>
    private static async Task SeedTestTenants(this IDocumentStore store)
    {
        var session = store.LightweightSession();

        // Seed test tenants
        var testTenant1 = new Tenant
        {
            Id = Guid.Parse(TestTenant1Id),
            Name = "Test Tenant 1"
        };

        var testTenant2 = new Tenant
        {
            Id = Guid.Parse(TestTenant2Id),
            Name = "Test Tenant 2"
        };

        session.Store(testTenant1, testTenant2);
        await session.SaveChangesAsync();
    }

    private static async Task SeedTestUsers(this IDocumentStore store, AsyncServiceScope scope)
    {
        var session = store.LightweightSession();
        
        var passwordHasher = new PasswordHasher();
        // await GenerateAccessTokens(scope);

        // Seed test users
        IdentityUser[] testUsers =
        [
            new()
            {
                Id = Guid.Parse(TestUser1Id),
                FirstName = "TestUser1",
                LastName = "",
                Email = "user1@test.com",
                HashedPassword = null,
                IsInvitePending = false,
                EmailVerified = true,
                Roles = [UserRole.User],
                TenantId = Guid.Parse(TestTenant1Id),
                TenantName = "Test Tenant 1",
                IsMFAEnabled = false,
                MfaConfig = new MfaConfig()
                {
                    IsMfaConfigured = false,
                    TotpSecretKey = ""
                },
            },
            new()
            {
                Id = Guid.Parse(TestUser2Id),
                FirstName = "TestUser2",
                LastName = "",
                Email = "user2@test.com",
                HashedPassword = null,
                IsInvitePending = false,
                EmailVerified = true,
                Roles = [UserRole.User],
                TenantId = Guid.Parse(TestTenant2Id),
                TenantName = "Test Tenant 2",
                IsMFAEnabled = false,
                MfaConfig = new MfaConfig()
                {
                    IsMfaConfigured = false,
                    TotpSecretKey = ""
                },
            },
            new()
            {
                Id = Guid.Parse(TestAdmin1Id),
                FirstName = "TestAdmin1",
                LastName = "",
                Email = "admin1@test.com",
                HashedPassword = null,
                IsInvitePending = false,
                EmailVerified = true,
                Roles = [UserRole.User, UserRole.Admin],
                TenantId = Guid.Parse(TestTenant1Id),
                TenantName = "Test Tenant 1",
                IsMFAEnabled = false,
                MfaConfig = new MfaConfig
                {
                    IsMfaConfigured = false,
                    TotpSecretKey = ""
                },
            },
            new()
            {
                Id = Guid.Parse(TestAdmin2Id),
                FirstName = "TestAdmin2",
                LastName = "",
                Email = "admin2@test.com",
                HashedPassword = null,
                IsInvitePending = false,
                EmailVerified = true,
                Roles = [UserRole.User, UserRole.Admin],
                TenantId = Guid.Parse(TestTenant2Id),
                TenantName = "Test Tenant 2",
                IsMFAEnabled = false,
                MfaConfig = new MfaConfig
                {
                    IsMfaConfigured = false,
                    TotpSecretKey = ""
                },
            },
            new()
            {
                Id = Guid.Parse(TestMasterAdminId),
                FirstName = "TestMasterAdmin",
                LastName = "",
                Email = "masterAdmin@test.com",
                HashedPassword = null,
                IsInvitePending = false,
                EmailVerified = true,
                Roles = [UserRole.User, UserRole.Admin, UserRole.MasterAdmin],
                TenantId = Guid.Parse(TestTenant1Id),
                TenantName = "Test Tenant 1",
                IsMFAEnabled = false,
                MfaConfig = new MfaConfig
                {
                    IsMfaConfigured = false,
                    TotpSecretKey = ""
                },
            }
        ];

        var seededUsers = new List<SeedUser>();

        foreach (var user in testUsers)
        {
            await SeedTestUser(store, user);
            var accessToken = GenerateAccessToken(scope, user);
            var seedUser = new SeedUser
            {
                Id = user.Id,
                Name = user.FirstName,
                AccessToken = accessToken
            };
            seededUsers.Add(seedUser);
        }
        
        // Serialize seeded users to valid JS and write them to file
        var serializedUsers = "";
        foreach (var user in seededUsers)
        {
            serializedUsers += $"export const {user.Name} = {{ id: \"{user.Id}\", accessToken: \"{user.AccessToken}\" }};\n";
        }
        var filePath = Path.Combine(Directory.GetCurrentDirectory(),"../../tests/k6", "testUsers.js");
        await File.WriteAllTextAsync(filePath, serializedUsers);
        
    }



    private static async Task SeedTestUser(this IDocumentStore store, IdentityUser user)
            {
            var session = store.LightweightSession(user.TenantId.ToString());

            // Seed identity user
            session.Store(user);

            // Seed user from users slice
            session.Store(user.Adapt<User>());

            await session.SaveChangesAsync();
        }


        /// <summary>
        /// Generate access token for test user
        /// </summary>
        private static string GenerateAccessToken(AsyncServiceScope scope, IdentityUser user)
        {

            var optionsGetter = scope.ServiceProvider.GetRequiredService<IOptionsMonitor<OpenIddictServerOptions>>();

            var options = optionsGetter.CurrentValue;
            List<Claim> claims =
            [
                new("sub", user.Id.ToString()),
                new("scope", "openid profile roles email offline_access"),
                new(SharedKernelConstants.TenantClaimName, user.TenantId.ToString())
            ];

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim("role", role.ToString()));
            }
            
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                EncryptingCredentials = options.DisableAccessTokenEncryption
                    ? null
                    : options.EncryptionCredentials.First(),
                Expires = DateTime.UtcNow + TimeSpan.FromDays(30), // Set expiration time to month
                IssuedAt = DateTime.UtcNow,
                Issuer = "https://localhost:5000", // the URL your auth server is hosted on, with trailing slash
                SigningCredentials = options.SigningCredentials.First(),
                TokenType = OpenIddictConstants.JsonWebTokenTypes.AccessToken,
            };

            return options.JsonWebTokenHandler.CreateToken(descriptor);
        }


    }

    public class SeedUser
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string AccessToken { get; set; }
    }

