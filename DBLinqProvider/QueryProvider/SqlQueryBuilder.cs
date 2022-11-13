namespace DBLinqProvider.QueryProvider;

public class SqlQueryBuilder
{
    private readonly string tableName;

    private string? condition;
    private string selector = "*";

    private SqlQueryBuilder(string tableName)
    {
        this.tableName = tableName;
    }

    public SqlQueryBuilder AddSelector(string selector)
    {
        this.selector = selector;

        return this;
    }

    public SqlQueryBuilder AddCondition(string condition)
    {
        this.condition = condition;

        return this;
    }

    public string Build() => $"SELECT {this.selector} FROM {tableName}" + (this.condition is not null ? $" WHERE {condition}" : string.Empty);

    public static SqlQueryBuilder Create(string tableName) => new(tableName);
}