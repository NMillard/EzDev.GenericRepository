using Microsoft.EntityFrameworkCore;

namespace EzDev.GenericRepository;

/// <summary>
/// Provides extension and listening points. Default implementation of each delegate is to do nothing.
/// </summary>
/// <typeparam name="TEntity">The aggregate type which the repository operates on.</typeparam>
public class RepositoryEvents<TEntity> {
    /// <summary>
    /// Triggered when performing the save operation, both when adding and updating an entity.
    /// </summary>
    public Func<TEntity, Task> OnBeforeSaving { get; set; } = _ => Task.CompletedTask;
    /// <summary>
    /// Triggered when the add and update save operation succeeded. 
    /// </summary>
    public Func<TEntity, Task> OnSaved { get; set; } = _ => Task.CompletedTask;
    /// <summary>
    /// Triggered when the add and update save operation failed.
    /// </summary>
    public Func<TEntity, DbUpdateException, Task> OnSavingFailed { get; set; } = (_,_) => Task.CompletedTask;
}