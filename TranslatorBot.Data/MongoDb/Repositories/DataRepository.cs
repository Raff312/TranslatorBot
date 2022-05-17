using TranslatorBot.Data.MongoDb.Repositories.Common;
using TranslatorBot.Entities.Data;
using TranslatorBot.Entities.Domain;

namespace TranslatorBot.Data.MongoDb.Repositories;

public abstract class DataRepository<TEntity> : IRepository<TEntity> where TEntity : PersistableEntity {
    protected readonly IDataContext Context;
    protected readonly string Collection;

    protected DataRepository(IDataContext context, string collection) {
        Context = context;
        Collection = collection;
    }

    public async Task<TEntity> GetByIdAsync(Guid id) {
        return await Context.GetByIdAsync<TEntity>(Collection, id).ConfigureAwait(false);
    }

    public async Task<IList<TEntity>> ListAllAsync(int skip = 0, int limit = int.MaxValue) {
        return await Context.ListAllAsync<TEntity>(Collection, skip, limit).ConfigureAwait(false);
    }

    public async Task<TEntity> CreateAsync(TEntity entity) {
        await Context.CreateAsync(Collection, entity).ConfigureAwait(false);
        return entity;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity) {
        await Context.UpdateAsync(Collection, entity).ConfigureAwait(false);
        return entity;
    }
}