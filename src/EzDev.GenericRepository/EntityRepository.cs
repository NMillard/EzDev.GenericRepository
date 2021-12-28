using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace EzDev.GenericRepository;

/// <summary>
/// A generic repository used with aggregates, based on EntityFramework Core <see cref="DbContext"/>. 
/// </summary>
/// <typeparam name="TEntity">The aggregate type this repository operates on.</typeparam>
public abstract class EntityRepository<TEntity> where TEntity : class {
    protected readonly DbContext Context;
    
    protected RepositoryEvents<TEntity> Events { get; init; } = new();
    
    /// <summary>
    /// Provides the base query. Override in the constructor if more complex query is required, such as including
    /// nested objects, filtering, etc.
    /// </summary>
    protected IQueryable<TEntity> Entities { get; init; }
    
    /// <summary>
    /// This constructor defaults to having the <see cref="Entities"/> property query the provided <see cref="DbContext"/>'s set of <see cref="TEntity"/>
    /// with the AsNoTracking enabled.
    /// Override the <see cref="Entities"/> if the entity require special includes or filters.
    /// </summary>
    /// <param name="context">The context this repository performs queries against.</param>
    protected EntityRepository(DbContext context) {
        Context = context;
        Entities = context.Set<TEntity>().AsNoTracking().AsQueryable();
    }

    protected EntityRepository(DbContext context, RepositoryEvents<TEntity> events) : this(context)
        => Events = events;

    public virtual async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) 
        => await Entities.SingleOrDefaultAsync(predicate, cancellationToken);

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await Entities.Where(predicate).ToListAsync(cancellationToken);

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) => await Entities.ToListAsync(cancellationToken);
    
    
    public virtual async Task<bool> AddAsync(TEntity entity, CancellationToken cancellationToken = default) {
        await Context.AddAsync(entity, cancellationToken);
        bool result = await SaveAsync(entity, cancellationToken);

        return result;
    }
    
    public virtual async Task<bool> UpdateAsync(TEntity entity) {
        Context.Update(entity);
        bool result = await SaveAsync(entity);

        return result;
    }
    
    /// <summary>
    /// The saving operation performed when adding or updating an entity.
    /// </summary>
    /// <returns><c>true</c> if save succeeded, <c>false</c> if an exception was thrown. Use <see cref="RepositoryEvents{TEntity}.OnSavingFailed"/> to extract exception information.</returns>
    protected virtual async Task<bool> SaveAsync(TEntity entity, CancellationToken cancellationToken = default) {
        await Events.OnBeforeSaving(entity);

        try {
            await Context.SaveChangesAsync(cancellationToken);
            await Events.OnSaved(entity);
            return true;
        } catch (DbUpdateException due) {
            await Events.OnSavingFailed(entity, due);
            return false;
        }
    }
}