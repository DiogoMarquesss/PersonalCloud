namespace PersonalCloud.Users.Api.Extensions;

public static class ControllerExtensions
{
    public static IServiceCollection AddApplicationControllers(
        this IServiceCollection services)
    {
        services.AddControllers();

        return services;
    }
}