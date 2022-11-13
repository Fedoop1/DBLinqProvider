using System.Data.SqlClient;
using System.Reflection;
using DBLinqProvider.Models;
using DBLinqProvider.Services;

namespace DBLinqProvider.QueryProvider;
public class SqlRepository<TEntity> : IRepository<TEntity>
{
    private readonly int fieldsCount = typeof(TEntity).GetFields(BindingFlags.DeclaredOnly).Count();
    private readonly string connectionString;
    private readonly IEntityActivator<TEntity> entityActivator;

    public SqlRepository(string connectionString, IEntityActivator<TEntity> entityActivator)
    {
        this.connectionString = connectionString;
        this.entityActivator = entityActivator;
    }

    public async Task<IEnumerable<TEntity>> FindAsync(string query)
    {
        await using var connection = new SqlConnection(this.connectionString);
        var command = new SqlCommand(query, connection);
        await connection.OpenAsync();
        var reader = await command.ExecuteReaderAsync();

        if(!await reader.ReadAsync()) return Enumerable.Empty<TEntity>();
        
        var entityFields = new Dictionary<string, object>(this.fieldsCount);
        var result = new List<TEntity>();

        do
        {
            for (var colIndex = 0; colIndex < reader.FieldCount; colIndex++)
            {
                entityFields[reader.GetName(colIndex)] = reader.GetValue(colIndex);
            }

            result.Add(this.entityActivator.CreateInstance(entityFields));

        } while (await reader.ReadAsync());

        return result;
    }
}
