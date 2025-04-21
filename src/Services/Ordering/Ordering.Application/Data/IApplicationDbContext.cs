namespace Ordering.Application.Data;
public interface IApplicationDbContext
{
    /// <summary>
    /// Adds an entity to the context.
    /// </summary>
    void Add<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Updates an entity in the context.
    /// </summary>
    void Update<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Removes an entity from the context.
    /// </summary>
    void Remove<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Retrieves all entities of a given type as a queryable.
    /// </summary>
    IQueryable<TEntity> Query<TEntity>() where TEntity : class;

    /// <summary>
    /// Saves changes to the database.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}