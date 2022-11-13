using System.Data.SqlClient;
using DBLinqProvider.Models;
using DBLinqProvider.QueryProvider;
using DBLinqProvider.Services;

namespace DBLinqProvider.Tests;

[TestFixture]
internal class SqlCollectionTests
{
    private const string ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DBLinqProvider";
    private readonly string tableName = $"products_{Guid.NewGuid():N}";

    private readonly Product[] productsSource =
    {
        new (1, "Product1", 1),
        new (2, "Product2", 2),
        new (3, "Product3", 3),
        new (4, "Product4", 4),
        new (5, "Product5", 5),
    };

    private SqlCollection<Product> productsCollection;

    [OneTimeSetUp]
    public void InitDatabase()
    {
        using var connection = new SqlConnection(ConnectionString);

        connection.Open();

        var transaction = connection.BeginTransaction();

        try
        {
            new SqlCommand(
                    $"CREATE TABLE [dbo].[{tableName}] ([ProductId] INT NOT NULL PRIMARY KEY, [Name] NVARCHAR(50) NULL, [Price] DECIMAL NULL)", connection)
            { Transaction = transaction }
                .ExecuteScalar();

            foreach (var product in productsSource)
            {
                new SqlCommand($"INSERT INTO [dbo].[{tableName}] (ProductId, Name, Price) VALUES ({product.ProductId},'{product.Name}',{product.Price})", connection) { Transaction = transaction }.ExecuteScalar();
            }
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }

        this.productsCollection = new SqlCollection<Product>(ConnectionString, new ProductActivator(), tableName);

        transaction.Commit();
        connection.Close();
    }

    [OneTimeTearDown]
    public void ClearDatabase()
    {
        using var connection = new SqlConnection(ConnectionString);

        connection.Open();

        var transaction = connection.BeginTransaction();

        try
        {
            new SqlCommand(
                    $"DROP TABLE [dbo].[{tableName}]", connection)
            {
                Transaction = transaction
            }.ExecuteScalar();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }

        transaction.Commit();
        connection.Close();
    }

    [Test]
    public void SqlCollection_WhereTest()
    {
        var expectedResult = this.productsSource.Where(p => p.Price > 3);
        var actualResult = this.productsCollection.Where(p => p.Price > 3);

        var array = this.productsCollection.ToArray();

        CollectionAssert.AreEquivalent(expectedResult, actualResult);
    }

    // TODO: Add Select tests
}
