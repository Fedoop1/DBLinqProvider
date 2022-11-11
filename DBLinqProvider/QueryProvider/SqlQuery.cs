using System.Collections;
using System.Linq.Expressions;

namespace DBLinqProvider.QueryProvider;

public class SqlQuery<TEntity> : IQueryable<TEntity>
{
    public SqlQuery(Expression expression, IQueryProvider provider)
    {
        Provider = provider;
        Expression = expression;
    }

    public IEnumerator<TEntity> GetEnumerator() =>
        Provider.Execute<IEnumerable<TEntity>>(Expression).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        ((IEnumerable)Provider.Execute(Expression)).GetEnumerator();

    public Type ElementType { get; }
    public Expression Expression { get; }
    public IQueryProvider Provider { get; }
}
