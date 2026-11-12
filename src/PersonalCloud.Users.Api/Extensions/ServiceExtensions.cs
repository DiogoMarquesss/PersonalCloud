using PersonalCloud.Users.Api.Services;
using PersonalCloud.Users.Api.Services.Intrerfaces;

namespace PersonalCloud.Users.Api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}