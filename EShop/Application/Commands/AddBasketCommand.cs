using Application.DTO;
using Application.Repositories;
using Domain.Entities;
using DotNext;
using Mediator;

namespace Application.Commands;

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

public class AddBasketCommandHandler : IRequestHandler<AddBasketCommand, Result<BasketAdded, ErrorCodes>>
{
    private readonly IBasketRepository _repository;

    public AddBasketCommandHandler(IBasketRepository repository)
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
        
        Basket basket = new Basket(products);

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
        return new(basketAdded);
    }
}

