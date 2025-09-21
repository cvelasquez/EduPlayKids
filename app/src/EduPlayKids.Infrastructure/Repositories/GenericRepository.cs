using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EduPlayKids.Application.Interfaces.Repositories;
using EduPlayKids.Domain.Common;
using EduPlayKids.Infrastructure.Data.Context;

namespace EduPlayKids.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation providing standard CRUD operations for all entities.
/// Optimized for mobile SQLite performance with comprehensive error handling and logging.
/// Implements async patterns for responsive UI in educational applications for children.
/// </summary>
/// <typeparam name="T">The entity type that inherits from BaseEntity</typeparam>
public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly EduPlayKidsDbContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly ILogger<GenericRepository<T>> _logger;

    /// <summary>
    /// Initializes a new instance of the GenericRepository class.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="logger">Logger for error handling and debugging</param>
    public GenericRepository(EduPlayKidsDbContext context, ILogger<GenericRepository<T>> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<T>();
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region Async Query Operations

    /// <inheritdoc />
    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting entity {EntityType} with ID {Id}", typeof(T).Name, id);

            var entity = await _dbSet
                .Where(e => !e.IsDeleted && e.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                _logger.LogDebug("Entity {EntityType} with ID {Id} not found", typeof(T).Name, id);
            }

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity {EntityType} with ID {Id}", typeof(T).Name, id);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting all entities of type {EntityType}", typeof(T).Name);

            return await _dbSet
                .Where(e => !e.IsDeleted)
                .OrderBy(e => e.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting entities of type {EntityType} with predicate", typeof(T).Name);

            return await _dbSet
                .Where(e => !e.IsDeleted)
                .Where(predicate)
                .OrderBy(e => e.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entities of type {EntityType} with predicate", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting single entity of type {EntityType} with predicate", typeof(T).Name);

            return await _dbSet
                .Where(e => !e.IsDeleted)
                .Where(predicate)
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting single entity of type {EntityType} with predicate", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidatePaginationParameters(pageNumber, pageSize);

            _logger.LogDebug("Getting paged entities of type {EntityType} - Page {PageNumber}, Size {PageSize}",
                typeof(T).Name, pageNumber, pageSize);

            return await _dbSet
                .Where(e => !e.IsDeleted)
                .OrderBy(e => e.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<T>> GetPagedAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidatePaginationParameters(pageNumber, pageSize);

            _logger.LogDebug("Getting filtered paged entities of type {EntityType} - Page {PageNumber}, Size {PageSize}",
                typeof(T).Name, pageNumber, pageSize);

            return await _dbSet
                .Where(e => !e.IsDeleted)
                .Where(predicate)
                .OrderBy(e => e.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting filtered paged entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting count of entities of type {EntityType}", typeof(T).Name);

            return await _dbSet
                .Where(e => !e.IsDeleted)
                .CountAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting count of entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task<int> GetCountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting count of entities of type {EntityType} with predicate", typeof(T).Name);

            return await _dbSet
                .Where(e => !e.IsDeleted)
                .Where(predicate)
                .CountAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting count of entities of type {EntityType} with predicate", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking existence of entity of type {EntityType} with predicate", typeof(T).Name);

            return await _dbSet
                .Where(e => !e.IsDeleted)
                .Where(predicate)
                .AnyAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of entity of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    #endregion

    #region Async Command Operations

    /// <inheritdoc />
    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entity);

            _logger.LogDebug("Adding entity of type {EntityType}", typeof(T).Name);

            // Set audit timestamps
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.IsDeleted = false;

            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding entity of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entities);

            var entityList = entities.ToList();
            _logger.LogDebug("Adding {Count} entities of type {EntityType}", entityList.Count, typeof(T).Name);

            var utcNow = DateTime.UtcNow;
            foreach (var entity in entityList)
            {
                entity.CreatedAt = utcNow;
                entity.UpdatedAt = utcNow;
                entity.IsDeleted = false;
            }

            await _dbSet.AddRangeAsync(entityList, cancellationToken);
            return entityList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding range of entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entity);

            _logger.LogDebug("Updating entity of type {EntityType} with ID {Id}", typeof(T).Name, entity.Id);

            // Update audit timestamp
            entity.UpdatedAt = DateTime.UtcNow;

            _dbSet.Update(entity);
            return Task.FromResult(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating entity of type {EntityType} with ID {Id}", typeof(T).Name, entity?.Id);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entities);

            var entityList = entities.ToList();
            _logger.LogDebug("Updating {Count} entities of type {EntityType}", entityList.Count, typeof(T).Name);

            var utcNow = DateTime.UtcNow;
            foreach (var entity in entityList)
            {
                entity.UpdatedAt = utcNow;
            }

            _dbSet.UpdateRange(entityList);
            return Task.FromResult<IEnumerable<T>>(entityList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating range of entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual Task SoftDeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entity);

            _logger.LogDebug("Soft deleting entity of type {EntityType} with ID {Id}", typeof(T).Name, entity.Id);

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error soft deleting entity of type {EntityType} with ID {Id}", typeof(T).Name, entity?.Id);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Soft deleting entity of type {EntityType} with ID {Id}", typeof(T).Name, id);

            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity != null)
            {
                await SoftDeleteAsync(entity, cancellationToken);
            }
            else
            {
                _logger.LogWarning("Entity of type {EntityType} with ID {Id} not found for soft delete", typeof(T).Name, id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error soft deleting entity of type {EntityType} with ID {Id}", typeof(T).Name, id);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual Task HardDeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entity);

            _logger.LogDebug("Hard deleting entity of type {EntityType} with ID {Id}", typeof(T).Name, entity.Id);

            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error hard deleting entity of type {EntityType} with ID {Id}", typeof(T).Name, entity?.Id);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task HardDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Hard deleting entity of type {EntityType} with ID {Id}", typeof(T).Name, id);

            var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
            if (entity != null)
            {
                await HardDeleteAsync(entity, cancellationToken);
            }
            else
            {
                _logger.LogWarning("Entity of type {EntityType} with ID {Id} not found for hard delete", typeof(T).Name, id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error hard deleting entity of type {EntityType} with ID {Id}", typeof(T).Name, id);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task<T?> RestoreAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Restoring entity of type {EntityType} with ID {Id}", typeof(T).Name, id);

            var entity = await _dbSet
                .Where(e => e.IsDeleted && e.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

            if (entity != null)
            {
                entity.IsDeleted = false;
                entity.UpdatedAt = DateTime.UtcNow;
                _logger.LogDebug("Entity of type {EntityType} with ID {Id} restored successfully", typeof(T).Name, id);
            }
            else
            {
                _logger.LogWarning("Entity of type {EntityType} with ID {Id} not found in deleted state", typeof(T).Name, id);
            }

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring entity of type {EntityType} with ID {Id}", typeof(T).Name, id);
            throw;
        }
    }

    #endregion

    #region Specialized Operations for Educational Content

    /// <inheritdoc />
    public virtual async Task<IEnumerable<T>> GetOrderedAsync<TKey>(Expression<Func<T, TKey>> orderBy, bool isDescending = false, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting ordered entities of type {EntityType}", typeof(T).Name);

            var query = _dbSet.Where(e => !e.IsDeleted);

            query = isDescending
                ? query.OrderByDescending(orderBy)
                : query.OrderBy(orderBy);

            return await query.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting ordered entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<T>> GetWithIncludeAsync(string includeProperties, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting entities of type {EntityType} with include: {IncludeProperties}", typeof(T).Name, includeProperties);

            var query = _dbSet.Where(e => !e.IsDeleted);

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                query = query.Include(includeProperties);
            }

            return await query.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entities of type {EntityType} with include", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<T>> GetWithIncludeAsync(string[] includeProperties, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting entities of type {EntityType} with multiple includes", typeof(T).Name);

            var query = _dbSet.Where(e => !e.IsDeleted);

            if (includeProperties?.Length > 0)
            {
                foreach (var includeProperty in includeProperties)
                {
                    if (!string.IsNullOrWhiteSpace(includeProperty))
                    {
                        query = query.Include(includeProperty);
                    }
                }
            }

            return await query.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entities of type {EntityType} with multiple includes", typeof(T).Name);
            throw;
        }
    }

    #endregion

    #region Audit and Compliance Support

    /// <inheritdoc />
    public virtual async Task<IEnumerable<T>> GetCreatedInRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting entities of type {EntityType} created between {FromDate} and {ToDate}", typeof(T).Name, fromDate, toDate);

            return await _dbSet
                .Where(e => !e.IsDeleted && e.CreatedAt >= fromDate && e.CreatedAt <= toDate)
                .OrderBy(e => e.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entities of type {EntityType} created in range", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<T>> GetUpdatedInRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting entities of type {EntityType} updated between {FromDate} and {ToDate}", typeof(T).Name, fromDate, toDate);

            return await _dbSet
                .Where(e => !e.IsDeleted && e.UpdatedAt >= fromDate && e.UpdatedAt <= toDate)
                .OrderBy(e => e.UpdatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entities of type {EntityType} updated in range", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<T>> GetDeletedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting deleted entities of type {EntityType}", typeof(T).Name);

            return await _dbSet
                .Where(e => e.IsDeleted)
                .OrderBy(e => e.UpdatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting deleted entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Validates pagination parameters to ensure they are within acceptable ranges.
    /// </summary>
    /// <param name="pageNumber">The page number to validate</param>
    /// <param name="pageSize">The page size to validate</param>
    /// <exception cref="ArgumentException">Thrown when parameters are invalid</exception>
    private static void ValidatePaginationParameters(int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
        {
            throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));
        }

        if (pageSize < 1 || pageSize > 1000)
        {
            throw new ArgumentException("Page size must be between 1 and 1000", nameof(pageSize));
        }
    }

    #endregion
}