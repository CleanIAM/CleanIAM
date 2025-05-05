using Coravel;
using DotNetEnv;
using Identity;
using Lamar.Microsoft.DependencyInjection;
using ManagementPortal;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Razor;
using SharedKernel;
using Wolverine.Http;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseLamar();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddLogging();

// Register the Coravel's mailer service
builder.AddMailer();

string[] assemblies =
[
    "Identity",
    "SharedKernel",
    "ManagementPortal",
    "UrlShortener"
];

builder.Services.AddWolverineHttp();
builder.Host.UseProjects(assemblies);
builder.Services.AddIdentityProject(builder.Configuration);
builder.Services.AddManagementPortal(builder.Configuration);

// Configure Razor view locations
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.ViewLocationFormats.Clear();
    options.ViewLocationFormats.Add("/Api/Views/{1}/{0}.cshtml");
    options.ViewLocationFormats.Add("/Api/Views/Shared/{0}.cshtml");
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(opts =>
    {
        opts.Cookie.SameSite = SameSiteMode.Lax;
        opts.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        opts.Cookie.HttpOnly = true;
    }
);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwagger("CleanIAM", assemblies);
builder.Services.AddDatabases(builder.Configuration);
builder.Services.AddOpenIddict(builder.Configuration);
builder.Services.AddUtils(builder.Configuration);

var app = builder.Build();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.UseHttpsRedirection();

app.MapControllers();

app.UseSharedKernel();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.Run();