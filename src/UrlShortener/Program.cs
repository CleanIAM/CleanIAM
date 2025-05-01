using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Razor;
using SharedKernel;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseLamar();

string[] assemblies = ["UrlShortener"];
builder.Host.UseProjects(assemblies);

builder.Services.AddLogging();
builder.Services.AddControllersWithViews();
builder.Services.AddMarten(builder.Configuration);

// Configure Razor view locations
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.ViewLocationFormats.Clear();
    options.ViewLocationFormats.Add("/API/Views/{1}/{0}.cshtml");
    options.ViewLocationFormats.Add("/API/Views/Shared/{0}.cshtml");
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();