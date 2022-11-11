using System.Collections;
using System.Linq.Expressions;

namespace DBLinqProvider.QueryProvider;

public class SqlCollection<TEntity> : IQueryable<TEntity>
{
    public IEnumerator<TEntity> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Type ElementType { get; }
    public Expression Expression { get; }
    public IQueryProvider Provider { get; }
}
