namespace DBLinqProvider.Models;

public interface IRepository<TEntity>
{
    public Task<IEnumerable<TEntity>> FindAsync(string query);
}
