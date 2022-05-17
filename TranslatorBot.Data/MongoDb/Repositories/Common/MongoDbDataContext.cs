using MongoDB.Driver;
using TranslatorBot.Entities.Domain;

namespace TranslatorBot.Data.MongoDb.Repositories.Common;

public class MongoDbDataContext : IDataContext {
    private readonly IMongoDatabase _database;

    public MongoDbDataContext(IMongoDatabase database) {
        _database = database;
    }

    public IMongoCollection<TEntity> GetCollection<TEntity>(string collection) {
        return _database.GetCollection<TEntity>(collection);
    }

    public async Task<TEntity> GetByIdAsync<TEntity>(string collection, Guid id) where TEntity : PersistableEntity {
        return await GetCollection<TEntity>(collection).Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IList<TEntity>> ListAllAsync<TEntity>(string collection, int skip = 0, int limit = int.MaxValue) where TEntity : PersistableEntity {
        return await GetCollection<TEntity>(collection).Find(x => true).Skip(skip).Limit(limit).ToListAsync();
    }

    public async Task CreateAsync<TEntity>(string collection, TEntity entity) where TEntity : PersistableEntity {
        if (entity is IAuditable auditable) {
            auditable.Audit.PerformCreateAudit(DateTime.UtcNow);
        }
        await GetCollection<TEntity>(collection).InsertOneAsync(entity);
    }

    public async Task UpdateAsync<TEntity>(string collection, TEntity entity) where TEntity : PersistableEntity {
        if (entity is IAuditable auditable) {
            auditable.Audit.PerformModifyAudit(DateTime.UtcNow);
        }
        await GetCollection<TEntity>(collection).FindOneAndReplaceAsync(x => x.Id == entity.Id, entity);
    }
}