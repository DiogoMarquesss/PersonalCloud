
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using PersonalCloud.Users.Data.Context;
using PersonalCloud.Users.Data.Entities;
using PersonalCloud.Users.Api.Extensions;
using PersonalCloud.Common.Configuration;
using Npgsql;
using System.IO;

DotEnv.Load();
var builder = WebApplication.CreateBuilder(args);

var usersConnection = new NpgsqlConnectionStringBuilder
{
    Host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost",
    Port = int.Parse(Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432"),
    Database = RequiredEnvironmentVariable("USERS_DB"),
    Username = RequiredEnvironmentVariable("POSTGRES_USER"),
    Password = RequiredEnvironmentVariable("POSTGRES_PASSWORD")
}.ConnectionString;

// Database
builder.Services.AddDbContext<UsersDbContext>(options =>
    options.UseNpgsql(
        usersConnection
    )
);

// Shared Data Protection
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(
        new DirectoryInfo(RequiredEnvironmentVariable("DATA_PROTECTION_PATH"))
    )
    .SetApplicationName(Environment.GetEnvironmentVariable("DATA_PROTECTION_APPLICATION_NAME") ?? "PersonalCloud");

// Identity
builder.Services
    .AddIdentityCore<User>(options =>
    {
        options.User.RequireUniqueEmail = true;

        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<UsersDbContext>()
    .AddDefaultTokenProviders();

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

// Services
builder.Services.AddApplicationServices();

// Controllers
builder.Services.AddApplicationControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
