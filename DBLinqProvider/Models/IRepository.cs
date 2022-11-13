using System.Collections;

namespace DBLinqProvider.Models;

public interface IRepository<TEntity>
{
    public Task<IEnumerable> ExecuteQueryAsync(string query);
}
