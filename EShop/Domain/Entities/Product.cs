namespace Domain.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public Guid BasketId { get; set; }
    public decimal CostPerItem { get; set; }
}