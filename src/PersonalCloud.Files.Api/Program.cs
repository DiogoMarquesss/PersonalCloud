
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using PersonalCloud.Files.Api.Services;
using PersonalCloud.Files.Api.Services.Interfaces;
using PersonalCloud.Files.Data.Context;
using PersonalCloud.Common.Configuration;
using Npgsql;
using System.IO;

DotEnv.Load();
var builder = WebApplication.CreateBuilder(args);

var filesConnection = new NpgsqlConnectionStringBuilder
{
    Host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost",
    Port = int.Parse(Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432"),
    Database = RequiredEnvironmentVariable("FILES_DB"),
    Username = RequiredEnvironmentVariable("POSTGRES_USER"),
    Password = RequiredEnvironmentVariable("POSTGRES_PASSWORD")
}.ConnectionString;
builder.Configuration["Storage:BasePath"] = RequiredEnvironmentVariable("STORAGE_BASE_PATH");

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<FileDbContext>(options =>
    options.UseNpgsql(
        filesConnection
    ));

// Services
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ILocalFileStorage, LocalFileStorageService>();

// Shared Data Protection
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(
        new DirectoryInfo(RequiredEnvironmentVariable("DATA_PROTECTION_PATH"))
    )
    .SetApplicationName(Environment.GetEnvironmentVariable("DATA_PROTECTION_APPLICATION_NAME") ?? "PersonalCloud");

// Cookie Authentication
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "PersonalCloud.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;

        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;

        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };

        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

// Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static string RequiredEnvironmentVariable(string name) =>
    Environment.GetEnvironmentVariable(name) is { Length: > 0 } value
        ? value
        : throw new InvalidOperationException($"Required environment variable '{name}' is missing.");
