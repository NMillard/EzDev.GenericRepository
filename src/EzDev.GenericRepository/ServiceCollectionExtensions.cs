using Microsoft.Extensions.DependencyInjection;

namespace EzDev.GenericRepository; 

/// <summary>
/// Extensions to easily register implementations of <see cref="EntityRepository{TEntity}"/> with the dependency container.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    /// Add a repository for the provided entity as a scoped service.
    /// </summary>
    /// <typeparam name="TEntity">The entity type the repository operates on.</typeparam>
    /// <typeparam name="TRepository"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddRepository<TEntity, TRepository>(this IServiceCollection services)
        where TEntity : class
        where TRepository : EntityRepository<TEntity> {
        services.AddScoped<EntityRepository<TEntity>, TRepository>();
        
        return services;
    }

    /// <summary>
    /// Add repository events for the provided entity as a scoped service.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configure"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static IServiceCollection WithEvents<TEntity>(this IServiceCollection services, Action<RepositoryEvents<TEntity>> configure) {
        var repositoryEvents = new RepositoryEvents<TEntity>();
        configure(repositoryEvents);
        
        services.AddScoped(_ => repositoryEvents);
        
        return services;
    }
}