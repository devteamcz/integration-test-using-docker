using Microsoft.EntityFrameworkCore;

namespace BareBones.Api.Persistence;

public abstract class EfRepositoryBase<TEntity,TId> where TEntity : class
{
    protected readonly BareBonesContext Context;

    protected readonly DbSet<TEntity> Entities;


    protected EfRepositoryBase(BareBonesContext context)
    {
        Context = context;
        Entities = context.Set<TEntity>();
    }


    public ValueTask<TEntity?> GetAsync(TId id)
    {
        return Context.FindAsync<TEntity>(id);
    }

    public Task SaveAsync(TEntity entity)
    {
        Context.Add(entity);
        
        return Context.SaveChangesAsync();
    }

    public Task UpdateAsync(TEntity entity)
    {
        return Context.SaveChangesAsync();
    }
}