using System.Linq.Expressions;
using DBLinqProvider.Models;
using DBLinqProvider.QueryProvider;

namespace DBLinqProvider.Tests;

[TestFixture]
internal class ExpressionToSqlQueryTests
{
    [Test, TestCaseSource(typeof(ExpressionToSqlQueryConverterTestsSource), 
        nameof(ExpressionToSqlQueryConverterTestsSource.ExpressionsAndEquivalentSqlSource))]
    public void ExpressionConvertTest(Expression<Func<Product, bool>> expression, string expectedResult)
    {
        var expressionConverter = new ExpressionToSqlQueryConverter();

        var sql = expressionConverter.Convert<Product>(expression);

        Assert.IsTrue(sql == expectedResult);
    }
}
