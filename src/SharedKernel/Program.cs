using SharedKernel;

var builder = WebApplication.CreateBuilder(args);
string[] assemblies = ["SharedKernel"];

builder.Host.UseProjects(assemblies);
// Add databases to allow EF Core to work with the database
builder.Services.AddDatabases(builder.Configuration);



var app = builder.Build();
app.Run();