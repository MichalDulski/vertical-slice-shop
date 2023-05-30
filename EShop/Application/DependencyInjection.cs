using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediator(x => x.ServiceLifetime = ServiceLifetime.Scoped);
        return services;
    }
}