using Coravel;
using DotNetEnv;
using Identity;
using Identity.Application.Interfaces;
using Identity.Infrastructure.Services;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Razor;
using SharedKernel;
using Wolverine.Http;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
// If in dev mode, load .env file
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

string[] assemblies = ["Identity"];
builder.Services.AddWolverineHttp();

builder.Services.AddLogging();
builder.Services.AddScoped<ISigninRequestService, SigninRequestService>();
builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();
builder.Services.AddTransient<IIdentityBuilderService, IdentityBuilderService>();
builder.Services.AddScoped<IMailService, CoravelMailService>();

// Register the Coravel's mailer service
builder.AddMailer();

builder.Host.UseProjects(assemblies);
builder.Services.AddControllersWithViews();
// Configure Razor view locations
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.ViewLocationFormats.Clear();
    options.ViewLocationFormats.Add("/API/Views/{1}/{0}.cshtml");
    options.ViewLocationFormats.Add("/API/Views/Shared/{0}.cshtml");
});


builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
    opts =>
    {
        opts.Cookie.SameSite = SameSiteMode.Lax;
        opts.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        opts.Cookie.HttpOnly = true;
    }
    );

builder.Services.AddDatabases(builder.Configuration);
builder.Services.AddOpenIddict(builder.Configuration);


var app = builder.Build();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllers();


await app.ConfigureOpenIddict();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.MapControllers();

app.MapGet("/", () => "Hello, world!");

app.Run();