// ReSharper disable VirtualMemberCallInConstructor
namespace Domain.Entities;

public class Basket
{
    private Basket(){}
    
    public Basket(ICollection<Product> products)
    {
        Products = products;
        Total = products.Sum(x => x.CostPerItem*x.Quantity);
    }
    public Basket(Guid id, ICollection<Product> products, decimal total)
        => (Id, Products, Total) = (id, products, total);
    
    public Guid Id { get; set; }
    public virtual ICollection<Product> Products { get; set; }
    public decimal Total { get; set; }
}