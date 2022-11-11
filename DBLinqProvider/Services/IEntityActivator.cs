namespace DBLinqProvider.Services;

public interface IEntityActivator<out TEntity>
{
    TEntity CreateInstance(IDictionary<string , object> properties);
}
