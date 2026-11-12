using PersonalCloud.Files.Api.Services;
using PersonalCloud.Files.Api.Services.Interfaces;

namespace PersonalCloud.Files.Api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<IFileService, FileService>();
        return services;
    }
}