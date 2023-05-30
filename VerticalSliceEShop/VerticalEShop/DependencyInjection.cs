using API.Features.Baskets;
using API.Infrastructure;
using DotNext;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace API;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationCore(this IServiceCollection services, string connectionString)
    {
        services.AddMediator(x => x.ServiceLifetime = ServiceLifetime.Scoped);
        services.AddSingleton<IPipelineBehavior<AddBasketCommand, Result<BasketAdded, ErrorCodes>>, AddBasketValidator>();
        
        services.AddDbContext<ShopDbContext>(x => x.UseSqlite(connectionString));
        services.AddScoped<IAddBasketRepository, BasketRepository>();
        return services;
    }
}