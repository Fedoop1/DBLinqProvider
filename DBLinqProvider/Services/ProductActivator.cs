using DBLinqProvider.Models;

namespace DBLinqProvider.Services;

public class ProductActivator : IEntityActivator<Product>
{
    public Product CreateInstance(IDictionary<string, object> properties)
    {
        var productId = ValueOrDefault<int>(properties, nameof(Product.ProductId));
        var name = ValueOrDefault<string>(properties, nameof(Product.Name));
        var price = ValueOrDefault<decimal>(properties, nameof(Product.Price));

        return new Product(productId, name, price);
    }

    private static T? ValueOrDefault<T>(IDictionary<string, object> source, string property) =>
        source.TryGetValue(property, out var value) ? (T) value : default;
}
