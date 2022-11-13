using System.Collections;
using System.Data;
using System.Data.SqlClient;
using DBLinqProvider.Models;

namespace DBLinqProvider.QueryProvider;
public class SqlRepository<TEntity> : IRepository<TEntity>
{
    private readonly string connectionString;

    public SqlRepository(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public async Task<IEnumerable> ExecuteQueryAsync(string query)
    {
        await using var connection = new SqlConnection(this.connectionString);
        var command = new SqlCommand(query, connection);
        await connection.OpenAsync();
        var reader = await command.ExecuteReaderAsync();

        if(!await reader.ReadAsync()) return Enumerable.Empty<object>();
        
        var result = new List<dynamic>();

        do
        {
            var record = GetRecord(reader);

            result.Add(record);

        } while (await reader.ReadAsync());

        return result;
    }

    private static dynamic GetRecord(IDataRecord dataReader)
    {
        if (dataReader.FieldCount == 1) return dataReader.GetValue(0);

        var entityFields = new Dictionary<string, object>(dataReader.FieldCount);

        for (var colIndex = 0; colIndex < dataReader.FieldCount; colIndex++)
        {
            entityFields[dataReader.GetName(colIndex)] = dataReader.GetValue(colIndex);
        }

        return CreateInstance(entityFields)!;
    }

    private static TEntity CreateInstance(IReadOnlyDictionary<string, object> properties)
    {
        var instance = Activator.CreateInstance<TEntity>();

        var propertiesToSet = instance.GetType().GetProperties().Where(p => properties.ContainsKey(p.Name));

        foreach (var property in propertiesToSet)
        {
            property.SetValue(instance, properties[property.Name]);
        }

        return instance;
    }
}
