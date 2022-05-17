using TranslatorBot.Entities.Domain;

namespace TranslatorBot.Data.MongoDb.Repositories.Common;

public interface IDataContext {
    Task<TEntity> GetByIdAsync<TEntity>(string collection, Guid Id) where TEntity : PersistableEntity;
    Task<IList<TEntity>> ListAllAsync<TEntity>(string collection, int skip = 0, int limit = int.MaxValue) where TEntity : PersistableEntity;
    Task CreateAsync<TEntity>(string collection, TEntity entity) where TEntity : PersistableEntity;
    Task UpdateAsync<TEntity>(string collection, TEntity entity) where TEntity : PersistableEntity;
}