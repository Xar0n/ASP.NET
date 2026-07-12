using Microsoft.Extensions.DependencyInjection;
using PromoCodeFactory.Core.Application.Services;

namespace PromoCodeFactory.Core.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.InitServices();
        return services;
    }

    private static void InitServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
    }
}
