using DBLinqProvider.Attributes;

namespace DBLinqProvider.Models;

[Table("Products")]
public record Product(int ProductId, string Name, decimal Price);