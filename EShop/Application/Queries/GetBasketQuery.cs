using Application.Contracts;
using Application.DTO;
using DotNext;
using Mediator;

namespace Application.Queries;

public record struct GetBasketQuery(Guid BasketId) : IRequest<Result<GetBasket, ErrorCodes>>;

public class GetBasketQueryHandler : IRequestHandler<GetBasketQuery, Result<GetBasket, ErrorCodes>>
{
    private readonly IBasketRepository _repository;

    public GetBasketQueryHandler(IBasketRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<Result<GetBasket, ErrorCodes>> Handle(GetBasketQuery request, CancellationToken cancellationToken)
    {
        var basket = await _repository.GetBasketOrDefaultAsync(request.BasketId, cancellationToken);

        if (basket == default)
            return new(ErrorCodes.NotFound);

        var products = basket.Products
            .Select(x => new GetBasket.Product(x.Id, x.Name, x.Quantity, x.CostPerItem))
            .ToList();
        
        var dto = new GetBasket(basket.Id, products, basket.Total);
        
        return new(dto);
    }
}