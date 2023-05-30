using API.Domain.Entities;
using API.Infrastructure;
using DotNext;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Baskets;

[ApiController]
[Route("[controller]")]
public class AddBasketsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public AddBasketsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<Results<ProblemHttpResult, ValidationProblem,  Ok<BasketAdded>>> Add([FromBody] AddBasketCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return result.IsSuccessful 
                ? TypedResults.Ok(result.Value) 
                : TypedResults.Problem("", "", (int)ErrorCodes.InternalServerError);
        }
        catch(ValidationException ex)
        {
            IDictionary<string, string[]> propertyErrors = ex.Errors.GroupBy(x => x.PropertyName)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(y => y.ErrorMessage).ToArray());
            
            return TypedResults.ValidationProblem(propertyErrors);
        }
    }
}

public class AddBasketValidator : IPipelineBehavior<AddBasketCommand, Result<BasketAdded, ErrorCodes>>
{
    class Validator : AbstractValidator<AddBasketCommand>
    {
        public Validator()
        {
			RuleFor(x => x.Products).NotEmpty();
        	RuleForEach(x => x.Products).ChildRules(product =>
        	{	
            	product.RuleFor(x => x.Name).NotEmpty();
            	product.RuleFor(x => x.Quantity).GreaterThan(0);
            	product.RuleFor(x => x.CostPerItem).GreaterThan(0);
        	});
		}
    }

    public async ValueTask<Result<BasketAdded, ErrorCodes>> Handle(AddBasketCommand message, CancellationToken cancellationToken, MessageHandlerDelegate<AddBasketCommand, Result<BasketAdded, ErrorCodes>> next)
    {
        var validator = new Validator();
        
        var validationResult = await validator.ValidateAsync(message, cancellationToken);
        
        if (!validationResult.IsValid) 
        {
            throw new ValidationException(validationResult.Errors);
        }

        return await next(message, cancellationToken);
    }
}

public record struct AddBasketCommand(List<AddBasketCommand.AddBasketProduct> Products) : IRequest<Result<BasketAdded, ErrorCodes>>
{
    public class AddBasketProduct
    {
        public AddBasketProduct(string name, int quantity, decimal costPerItem)
        {
            Name = name;
            Quantity = quantity;
            CostPerItem = costPerItem;
        }

        public string Name { get; }
        public int Quantity { get; }
        public decimal CostPerItem { get; }
    }
}

public record struct BasketAdded(Guid BasketId, decimal Total);

public class AddBasketCommandHandler : IRequestHandler<AddBasketCommand, Result<BasketAdded, ErrorCodes>>
{
    private readonly IAddBasketRepository _repository;

    public AddBasketCommandHandler(IAddBasketRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<Result<BasketAdded, ErrorCodes>> Handle(AddBasketCommand request, CancellationToken cancellationToken)
    {
        var products = request.Products.Select(x => new Product()
        {
            Name = x.Name,
            Quantity = x.Quantity,
            CostPerItem = x.CostPerItem
        }).ToList();
        
        var basket = new Basket(products);

        await _repository.AddBasketAsync(basket, cancellationToken);

        try
        {
            await _repository.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // handle error
            // ...

            return new(ErrorCodes.InternalServerError);
        }

        var basketAdded = new BasketAdded(basket.Id, basket.Total);
        return basketAdded;
    }
}

public interface IAddBasketRepository
{
    Task AddBasketAsync(Basket basket, CancellationToken cancellationToken);
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

public class BasketRepository : IAddBasketRepository
{
    private readonly ShopDbContext _dbContext;

    public BasketRepository(ShopDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddBasketAsync(Basket basket, CancellationToken cancellationToken) 
        => await _dbContext.Baskets.AddAsync(basket, cancellationToken);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        => await _dbContext.SaveChangesAsync(cancellationToken);
}