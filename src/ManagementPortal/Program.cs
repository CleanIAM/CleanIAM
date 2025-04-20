using System.Linq.Expressions;
using Mapster;
using Lamar.Microsoft.DependencyInjection;
using ManagementPortal;
using Microsoft.AspNetCore.Mvc.Razor;
using SharedKernel;

TypeAdapterConfig.GlobalSettings.Compiler = exp => exp.CompileWithDebugInfo();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseLamar();


string[] assemblies = ["ManagementPortal"];

builder.Host.UseProjects(assemblies);

builder.Services.AddRouting(options => options.LowercaseUrls = true);


// Configure views locations
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.ViewLocationFormats.Clear();
    options.ViewLocationFormats.Add("/Api/Views/{1}/{0}.cshtml");
    options.ViewLocationFormats.Add("/Api/Views/Shared/{0}.cshtml");
});

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDatabases(builder.Configuration);
builder.Services.AddOpenIddict(builder.Configuration);
builder.Services.AddOidcAuthentication(builder.Configuration);
builder.Services.AddUtils(builder.Configuration);

// Configure Mapster
MapsterConfig.Configure();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseStatusCodePagesWithReExecute("/error");
    app.UseExceptionHandler("/error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers().RequireAuthorization(); // This makes all controllers require authentication by default

app.Run();