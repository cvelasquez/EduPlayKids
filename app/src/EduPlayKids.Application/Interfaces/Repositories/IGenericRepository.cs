using System.Linq.Expressions;
using EduPlayKids.Domain.Common;

namespace EduPlayKids.Application.Interfaces.Repositories;

/// <summary>
/// Generic repository interface providing standard CRUD operations for all entities.
/// Designed for mobile performance optimization with SQLite and offline-first functionality.
/// Implements async patterns for responsive UI and supports soft deletes for data recovery.
/// </summary>
/// <typeparam name="T">The entity type that inherits from BaseEntity</typeparam>
public interface IGenericRepository<T> where T : BaseEntity
{
    #region Async Query Operations

    /// <summary>
    /// Gets an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The entity if found, null otherwise</returns>
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities that are not soft deleted.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of all active entities</returns>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities based on a predicate condition.
    /// Optimized for mobile performance with query optimization.
    /// </summary>
    /// <param name="predicate">The condition to filter entities</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of entities matching the condition</returns>
    Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single entity based on a predicate condition.
    /// </summary>
    /// <param name="predicate">The condition to find the entity</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The first entity matching the condition, null if not found</returns>
    Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities with pagination support for large datasets.
    /// Essential for mobile performance when dealing with educational content.
    /// </summary>
    /// <param name="pageNumber">The page number (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Paginated collection of entities</returns>
    Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities with pagination and filtering support.
    /// </summary>
    /// <param name="predicate">The condition to filter entities</param>
    /// <param name="pageNumber">The page number (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Paginated and filtered collection of entities</returns>
    Task<IEnumerable<T>> GetPagedAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of entities that are not soft deleted.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Total count of active entities</returns>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of entities matching a predicate condition.
    /// </summary>
    /// <param name="predicate">The condition to count entities</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Count of entities matching the condition</returns>
    Task<int> GetCountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if any entity exists matching the predicate condition.
    /// </summary>
    /// <param name="predicate">The condition to check</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if any entity matches, false otherwise</returns>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    #endregion

    #region Async Command Operations

    /// <summary>
    /// Adds a new entity to the repository.
    /// Sets audit timestamps automatically.
    /// </summary>
    /// <param name="entity">The entity to add</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The added entity with generated ID</returns>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple entities in a single operation.
    /// Optimized for bulk operations like initial data seeding.
    /// </summary>
    /// <param name="entities">The entities to add</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The added entities with generated IDs</returns>
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity.
    /// Updates the audit timestamp automatically.
    /// </summary>
    /// <param name="entity">The entity to update</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The updated entity</returns>
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates multiple entities in a single operation.
    /// </summary>
    /// <param name="entities">The entities to update</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The updated entities</returns>
    Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a soft delete on the entity.
    /// Sets IsDeleted flag to true instead of physically removing the record.
    /// Essential for data recovery and audit compliance.
    /// </summary>
    /// <param name="entity">The entity to soft delete</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task SoftDeleteAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a soft delete on an entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to soft delete</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task SoftDeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Permanently removes an entity from the database.
    /// Use with caution - only for data that should never be recovered.
    /// </summary>
    /// <param name="entity">The entity to permanently delete</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task HardDeleteAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Permanently removes an entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to permanently delete</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task HardDeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores a soft-deleted entity.
    /// Sets IsDeleted flag back to false for data recovery.
    /// </summary>
    /// <param name="id">The ID of the entity to restore</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The restored entity if found, null otherwise</returns>
    Task<T?> RestoreAsync(int id, CancellationToken cancellationToken = default);

    #endregion

    #region Specialized Operations for Educational Content

    /// <summary>
    /// Gets entities ordered by a specified property.
    /// Useful for educational content ordering by difficulty, sequence, etc.
    /// </summary>
    /// <typeparam name="TKey">The type of the property to order by</typeparam>
    /// <param name="orderBy">Expression for the property to order by</param>
    /// <param name="isDescending">True for descending order, false for ascending</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Ordered collection of entities</returns>
    Task<IEnumerable<T>> GetOrderedAsync<TKey>(Expression<Func<T, TKey>> orderBy, bool isDescending = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities with include navigation properties.
    /// Essential for loading related educational content efficiently.
    /// </summary>
    /// <param name="includeProperties">Navigation properties to include</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of entities with loaded navigation properties</returns>
    Task<IEnumerable<T>> GetWithIncludeAsync(string includeProperties, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities with multiple include navigation properties.
    /// </summary>
    /// <param name="includeProperties">Array of navigation properties to include</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of entities with loaded navigation properties</returns>
    Task<IEnumerable<T>> GetWithIncludeAsync(string[] includeProperties, CancellationToken cancellationToken = default);

    #endregion

    #region Audit and Compliance Support

    /// <summary>
    /// Gets recently created entities within a specified time range.
    /// Useful for COPPA compliance and audit reporting.
    /// </summary>
    /// <param name="fromDate">Start date for the range</param>
    /// <param name="toDate">End date for the range</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of entities created within the date range</returns>
    Task<IEnumerable<T>> GetCreatedInRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recently updated entities within a specified time range.
    /// </summary>
    /// <param name="fromDate">Start date for the range</param>
    /// <param name="toDate">End date for the range</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of entities updated within the date range</returns>
    Task<IEnumerable<T>> GetUpdatedInRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets soft-deleted entities for audit and recovery purposes.
    /// Important for COPPA compliance and data restoration.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of soft-deleted entities</returns>
    Task<IEnumerable<T>> GetDeletedAsync(CancellationToken cancellationToken = default);

    #endregion
}