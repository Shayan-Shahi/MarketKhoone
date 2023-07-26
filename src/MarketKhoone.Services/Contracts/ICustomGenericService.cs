namespace MarketKhoone.Services.Contracts;

public interface ICustomGenericService<TEntity> where TEntity : class
{
    Task<TEntity> FindAsync(params object[] ids);

    Task AddAsync(TEntity entity);

    Task AddRangeAsync(IEnumerable<TEntity> entities);

    void Remove(TEntity entity);

    Task<TEntity> FindByIdAsync(long id);

    void RemoveRange(List<TEntity> entities);

    Task<bool> IsExistsBy(string propertyName1, string propertyName2, object propertyValue1, object propertyValue2);
}
