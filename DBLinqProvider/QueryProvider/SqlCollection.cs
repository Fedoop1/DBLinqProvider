using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using DBLinqProvider.Attributes;
using DBLinqProvider.Services;

namespace DBLinqProvider.QueryProvider;

public class SqlCollection<TEntity> : IQueryable<TEntity>
{
    private readonly SqlQueryProvider<TEntity> queryProvider;
    private readonly Expression expression;

    public SqlCollection(string connectionString, IEntityActivator<TEntity> activator, string? tableName = null)
    {
        this.expression = Expression.Constant(this);

        var repository = new SqlRepository<TEntity>(connectionString, activator);

        tableName ??= ((TableAttribute) typeof(TEntity).GetCustomAttribute(typeof(TableAttribute)))?.Name ??
                                  throw new Exception(
                                      $"{typeof(TEntity).Name} doesn't contain {nameof(TableAttribute)} attribute. Table can't be null");

        this.queryProvider = new SqlQueryProvider<TEntity>(repository, tableName);
    }

    public IEnumerator<TEntity> GetEnumerator() =>
        this.queryProvider.Execute<IEnumerable<TEntity>>(this.Expression).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public Type ElementType => typeof(TEntity);
    public Expression Expression => expression;
    public IQueryProvider Provider => this.queryProvider;
}
    