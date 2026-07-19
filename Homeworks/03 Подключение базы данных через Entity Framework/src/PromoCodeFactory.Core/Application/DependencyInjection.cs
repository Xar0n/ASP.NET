using Microsoft.Extensions.DependencyInjection;
using PromoCodeFactory.Core.Application.Abstractions;
using PromoCodeFactory.Core.Application.Services;

namespace PromoCodeFactory.Core.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddServices();
        return services;
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IPreferenceService, PreferenceService>();
        services.AddScoped<IPromoCodeService, PromoCodeService>();
    }
}
