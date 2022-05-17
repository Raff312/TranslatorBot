using TranslatorBot.Entities.Domain;

namespace TranslatorBot.Entities.Data;

public interface IRepository<TEntity> where TEntity : PersistableEntity {
    Task<TEntity> GetByIdAsync(Guid id);
    Task<IList<TEntity>> ListAllAsync(int skip = 0, int limit = int.MaxValue);
    Task<TEntity> CreateAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
}