using Application.Repositories;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ShopDbContext>(x => x.UseSqlite(connectionString));
        services.AddScoped<IBasketRepository, BasketRepository>();
        
        return services;
    }
}