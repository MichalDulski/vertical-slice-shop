using API.Infrastructure;
using DotNext;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Baskets;

[ApiController]
[Route("[controller]")]
public class GetBasketController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public GetBasketController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<Results<NotFound, Ok<GetBasket>>> Get([FromRoute] Guid id)
    {
        var request = new GetBasketQuery(id);

        var result = await _mediator.Send(request);

        return result.IsSuccessful ? TypedResults.Ok(result.Value) : TypedResults.NotFound();
    }
}

public record struct GetBasketQuery(Guid BasketId) : IRequest<Result<GetBasket, ErrorCodes>>;

public record struct GetBasket(Guid Id, IEnumerable<GetBasket.Product> Products, decimal Total)
{
    public record struct Product(Guid Id, string Name, int Quantity, decimal CostPerItem);
}

public class GetBasketQueryHandler : IRequestHandler<GetBasketQuery, Result<GetBasket, ErrorCodes>>
{
    private readonly ShopDbContext _dbContext;

    public GetBasketQueryHandler(ShopDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<Result<GetBasket, ErrorCodes>> Handle(GetBasketQuery request, CancellationToken cancellationToken)
    {
        var basketId = request.BasketId;
        
        var basket = await _dbContext.Baskets
            .Include(x => x.Products)
            .AsNoTracking()
            .Select(x => new GetBasket(x.Id, 
                x.Products.Select(y => new GetBasket.Product(y.Id, y.Name, y.Quantity, y.CostPerItem)),
                x.Total))
            .FirstOrDefaultAsync(x => x.Id == basketId, cancellationToken);

        if (basket == default)
            return new(ErrorCodes.NotFound);

        return new(basket);
    }
}
