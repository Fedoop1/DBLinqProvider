using DBLinqProvider.Attributes;

namespace DBLinqProvider.Models;

[Table("Products")]
public record Product
{
    public int ProductId { get; init; }
    public string Name { get; init; }
    public decimal Price { get; init; }

    public Product(int productId, string name, decimal price)
    {
        ProductId = productId;
        Name = name;
        Price = price;
    }

    public Product()
    {
        
    }
}