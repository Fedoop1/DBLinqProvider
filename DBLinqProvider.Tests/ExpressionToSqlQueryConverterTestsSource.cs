using System.Linq.Expressions;
using DBLinqProvider.Models;

namespace DBLinqProvider.Tests;
public static class ExpressionToSqlQueryConverterTestsSource
{
    public static IEnumerable<object[]> ExpressionsAndEquivalentSqlSource()
    {
        yield return new object[] { (Expression<Func<Product, bool>>)(p => p.Price > 100), "SELECT * FROM Products WHERE Product.Price > 100" };
        yield return new object[] { (Expression<Func<Product, bool>>)(p => p.Price < 100), "SELECT * FROM Products WHERE Product.Price < 100" };
        yield return new object[] { (Expression<Func<Product, bool>>)(p => p.Price == 100), "SELECT * FROM Products WHERE Product.Price = 100" };
        yield return new object[] { (Expression<Func<Product, bool>>)(p => p.Price < 100 && p.Price > 50), "SELECT * FROM Products WHERE Product.Price < 100 AND Product.Price > 50" };
    }
}
