using Domain;
using Domain.Entities;

namespace Application.Repositories;

public interface IBasketRepository
{
    Task AddBasketAsync(Basket basket, CancellationToken cancellationToken);

    Task<Basket?> GetBasketOrDefaultAsync(Guid basketId, CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}