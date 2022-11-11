using System.Linq.Expressions;
using DBLinqProvider.Models;

namespace DBLinqProvider.QueryProvider;
public class SqlQueryProvider<TEntity> : IQueryProvider
{
    private readonly IRepository<TEntity> repository;
    private readonly ExpressionToSqlQueryConverter expressionConverter;

    public SqlQueryProvider(IRepository<TEntity> repository)
    {
        this.repository = repository;
        this.expressionConverter = new ExpressionToSqlQueryConverter();
    }

    public IQueryable CreateQuery(Expression expression) => CreateQuery<object>(expression);

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression) =>
        new SqlQuery<TElement>(expression, this);

    public object? Execute(Expression expression) => Execute<object>(expression);

    public TResult Execute<TResult>(Expression expression)
    {
        var query = expressionConverter.Convert<TEntity>(expression);

        return (TResult)repository.FindAsync(query).Result;
    }
}
