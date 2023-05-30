using Application.Repositories;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly ShopDbContext _dbContext;

    public BasketRepository(ShopDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddBasketAsync(Basket basket, CancellationToken cancellationToken) 
        => await _dbContext.Baskets.AddAsync(basket, cancellationToken);

    public async Task<Basket?> GetBasketOrDefaultAsync(Guid basketId, CancellationToken cancellationToken)
        => await _dbContext.Baskets
            .Include(x => x.Products)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == basketId, cancellationToken);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        => await _dbContext.SaveChangesAsync(cancellationToken);
}