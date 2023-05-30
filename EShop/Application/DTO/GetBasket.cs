namespace Application.DTO;

public record struct GetBasket(Guid Id, List<GetBasket.Product> Products, decimal Total)
{
    public record struct Product(Guid Id, string Name, int Quantity, decimal CostPerItem);
}