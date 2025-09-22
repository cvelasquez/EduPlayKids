using EduPlayKids.Application.Interfaces.Repositories;
using EduPlayKids.Domain.Entities;
using EduPlayKids.Infrastructure.Data.Context;
using EduPlayKids.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EduPlayKids.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for ParentalPin entity operations.
/// Provides secure PIN data access with comprehensive security monitoring and COPPA compliance.
/// </summary>
public class ParentalPinRepository : GenericRepository<ParentalPin>, IParentalPinRepository
{
    public ParentalPinRepository(EduPlayKidsDbContext context, ILogger<ParentalPinRepository> logger)
        : base(context, logger)
    {
    }

    /// <summary>
    /// Gets the active PIN record for a specific user.
    /// </summary>
    /// <param name="userId">The user ID to get PIN for.</param>
    /// <returns>The active PIN record, or null if not found.</returns>
    public async Task<ParentalPin?> GetActiveByUserIdAsync(int userId)
    {
        try
        {
            _logger.LogDebug("Getting active PIN for user {UserId}", userId);

            var pin = await _context.ParentalPins
                .Where(p => p.UserId == userId && p.IsActive && !p.IsDeleted)
                .Include(p => p.User)
                .FirstOrDefaultAsync();

            if (pin != null)
            {
                _logger.LogDebug("Found active PIN for user {UserId}", userId);
            }
            else
            {
                _logger.LogDebug("No active PIN found for user {UserId}", userId);
            }

            return pin;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active PIN for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Checks if a user has an active PIN configured.
    /// </summary>
    /// <param name="userId">The user ID to check.</param>
    /// <returns>True if user has an active PIN.</returns>
    public async Task<bool> HasActivePinAsync(int userId)
    {
        try
        {
            _logger.LogDebug("Checking if user {UserId} has active PIN", userId);

            var hasPin = await _context.ParentalPins
                .AnyAsync(p => p.UserId == userId && p.IsActive && !p.IsDeleted);

            _logger.LogDebug("User {UserId} has active PIN: {HasPin}", userId, hasPin);
            return hasPin;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking active PIN for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Gets all PIN records for a user (including inactive ones for history).
    /// </summary>
    /// <param name="userId">The user ID to get PIN history for.</param>
    /// <returns>List of all PIN records for the user.</returns>
    public async Task<IEnumerable<ParentalPin>> GetPinHistoryAsync(int userId)
    {
        try
        {
            _logger.LogDebug("Getting PIN history for user {UserId}", userId);

            var history = await _context.ParentalPins
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            _logger.LogDebug("Found {Count} PIN records in history for user {UserId}", history.Count, userId);
            return history;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting PIN history for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Gets all currently locked PIN records.
    /// </summary>
    /// <returns>List of locked PIN records.</returns>
    public async Task<IEnumerable<ParentalPin>> GetLockedPinsAsync()
    {
        try
        {
            _logger.LogDebug("Getting all locked PIN records");

            var now = DateTime.UtcNow;
            var lockedPins = await _context.ParentalPins
                .Where(p => p.LockedUntil.HasValue && p.LockedUntil.Value > now && !p.IsDeleted)
                .Include(p => p.User)
                .ToListAsync();

            _logger.LogDebug("Found {Count} locked PIN records", lockedPins.Count);
            return lockedPins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting locked PIN records");
            throw;
        }
    }

    /// <summary>
    /// Gets PIN records with recent failed attempts for security monitoring.
    /// </summary>
    /// <param name="hoursBack">Hours to look back for failed attempts.</param>
    /// <returns>List of PIN records with recent failed attempts.</returns>
    public async Task<IEnumerable<ParentalPin>> GetPinsWithRecentFailedAttemptsAsync(int hoursBack = 24)
    {
        try
        {
            _logger.LogDebug("Getting PINs with failed attempts in last {Hours} hours", hoursBack);

            var cutoffTime = DateTime.UtcNow.AddHours(-hoursBack);
            var pinsWithFailures = await _context.ParentalPins
                .Where(p => p.LastFailedAttemptAt.HasValue &&
                           p.LastFailedAttemptAt.Value > cutoffTime &&
                           !p.IsDeleted)
                .Include(p => p.User)
                .OrderByDescending(p => p.LastFailedAttemptAt)
                .ToListAsync();

            _logger.LogDebug("Found {Count} PINs with recent failed attempts", pinsWithFailures.Count);
            return pinsWithFailures;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting PINs with recent failed attempts");
            throw;
        }
    }

    /// <summary>
    /// Deactivates all existing PINs for a user.
    /// </summary>
    /// <param name="userId">The user ID to deactivate PINs for.</param>
    /// <returns>Task representing the async operation.</returns>
    public async Task DeactivateAllPinsForUserAsync(int userId)
    {
        try
        {
            _logger.LogDebug("Deactivating all PINs for user {UserId}", userId);

            var activePins = await _context.ParentalPins
                .Where(p => p.UserId == userId && p.IsActive && !p.IsDeleted)
                .ToListAsync();

            foreach (var pin in activePins)
            {
                pin.IsActive = false;
                pin.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Deactivated {Count} PINs for user {UserId}", activePins.Count, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating PINs for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Cleans up expired lockouts automatically.
    /// </summary>
    /// <returns>Number of records cleaned up.</returns>
    public async Task<int> CleanupExpiredLockoutsAsync()
    {
        try
        {
            _logger.LogDebug("Cleaning up expired PIN lockouts");

            var now = DateTime.UtcNow;
            var expiredLockouts = await _context.ParentalPins
                .Where(p => p.LockedUntil.HasValue && p.LockedUntil.Value <= now && !p.IsDeleted)
                .ToListAsync();

            int cleanedCount = 0;
            foreach (var pin in expiredLockouts)
            {
                pin.LockedUntil = null;
                pin.FailedAttempts = 0;
                pin.UpdatedAt = now;
                cleanedCount++;
            }

            if (cleanedCount > 0)
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Cleaned up {Count} expired PIN lockouts", cleanedCount);
            }

            return cleanedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up expired lockouts");
            throw;
        }
    }

    /// <summary>
    /// Gets security statistics for monitoring and reporting.
    /// </summary>
    /// <param name="userId">Optional user ID to filter statistics.</param>
    /// <returns>Security statistics object.</returns>
    public async Task<object> GetSecurityStatisticsAsync(int? userId = null)
    {
        try
        {
            _logger.LogDebug("Getting security statistics for user {UserId}", userId?.ToString() ?? "all");

            var query = _context.ParentalPins.Where(p => !p.IsDeleted);
            if (userId.HasValue)
            {
                query = query.Where(p => p.UserId == userId.Value);
            }

            var now = DateTime.UtcNow;
            var last24Hours = now.AddHours(-24);
            var last7Days = now.AddDays(-7);

            var statistics = await query.GroupBy(p => 1).Select(g => new
            {
                TotalPins = g.Count(),
                ActivePins = g.Count(p => p.IsActive),
                LockedPins = g.Count(p => p.LockedUntil.HasValue && p.LockedUntil.Value > now),
                PinsWithRecentFailures = g.Count(p => p.LastFailedAttemptAt.HasValue && p.LastFailedAttemptAt.Value > last24Hours),
                TotalFailedAttempts = g.Sum(p => p.TotalFailedAttempts),
                FailedAttemptsLast24Hours = g.Sum(p => p.LastFailedAttemptAt.HasValue && p.LastFailedAttemptAt.Value > last24Hours ? p.FailedAttempts : 0),
                PinsCreatedLast7Days = g.Count(p => p.CreatedAt > last7Days),
                PinsChangedLast7Days = g.Count(p => p.LastChangedAt > last7Days),
                AverageAgeDays = g.Average(p => (now - p.LastChangedAt).TotalDays)
            }).FirstOrDefaultAsync();

            return statistics ?? (object)new
            {
                TotalPins = 0,
                ActivePins = 0,
                LockedPins = 0,
                PinsWithRecentFailures = 0,
                TotalFailedAttempts = 0,
                FailedAttemptsLast24Hours = 0,
                PinsCreatedLast7Days = 0,
                PinsChangedLast7Days = 0,
                AverageAgeDays = 0.0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting security statistics");
            throw;
        }
    }

    /// <summary>
    /// Updates the last successful verification timestamp.
    /// </summary>
    /// <param name="pinId">The PIN record ID.</param>
    /// <returns>Task representing the async operation.</returns>
    public async Task UpdateLastSuccessfulVerificationAsync(int pinId)
    {
        try
        {
            _logger.LogDebug("Updating last successful verification for PIN {PinId}", pinId);

            var pin = await _context.ParentalPins.FindAsync(pinId);
            if (pin != null)
            {
                pin.RecordSuccessfulVerification();
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated successful verification for PIN {PinId}", pinId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating successful verification for PIN {PinId}", pinId);
            throw;
        }
    }

    /// <summary>
    /// Records a failed attempt and updates lockout status if needed.
    /// </summary>
    /// <param name="pinId">The PIN record ID.</param>
    /// <returns>Task representing the async operation.</returns>
    public async Task RecordFailedAttemptAsync(int pinId)
    {
        try
        {
            _logger.LogDebug("Recording failed attempt for PIN {PinId}", pinId);

            var pin = await _context.ParentalPins.FindAsync(pinId);
            if (pin != null)
            {
                pin.RecordFailedAttempt();
                await _context.SaveChangesAsync();

                _logger.LogWarning("Recorded failed attempt for PIN {PinId}. Failed attempts: {FailedAttempts}, Locked until: {LockedUntil}",
                    pinId, pin.FailedAttempts, pin.LockedUntil);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording failed attempt for PIN {PinId}", pinId);
            throw;
        }
    }

    /// <summary>
    /// Resets failed attempt counters for a PIN record.
    /// </summary>
    /// <param name="pinId">The PIN record ID.</param>
    /// <returns>Task representing the async operation.</returns>
    public async Task ResetFailedAttemptsAsync(int pinId)
    {
        try
        {
            _logger.LogDebug("Resetting failed attempts for PIN {PinId}", pinId);

            var pin = await _context.ParentalPins.FindAsync(pinId);
            if (pin != null)
            {
                pin.FailedAttempts = 0;
                pin.LockedUntil = null;
                pin.LastFailedAttemptAt = null;
                pin.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Reset failed attempts for PIN {PinId}", pinId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting failed attempts for PIN {PinId}", pinId);
            throw;
        }
    }

    /// <summary>
    /// Gets all PIN records that should be updated based on age policy.
    /// </summary>
    /// <param name="maxAgeInDays">Maximum age in days before update is recommended.</param>
    /// <returns>List of PIN records that should be updated.</returns>
    public async Task<IEnumerable<ParentalPin>> GetPinsRequiringUpdateAsync(int maxAgeInDays = 180)
    {
        try
        {
            _logger.LogDebug("Getting PINs requiring update older than {Days} days", maxAgeInDays);

            var cutoffDate = DateTime.UtcNow.AddDays(-maxAgeInDays);
            var oldPins = await _context.ParentalPins
                .Where(p => p.IsActive && p.LastChangedAt < cutoffDate && !p.IsDeleted)
                .Include(p => p.User)
                .ToListAsync();

            _logger.LogDebug("Found {Count} PINs requiring update", oldPins.Count);
            return oldPins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting PINs requiring update");
            throw;
        }
    }

    /// <summary>
    /// Performs bulk security maintenance operations.
    /// </summary>
    /// <returns>Maintenance report object.</returns>
    public async Task<object> PerformSecurityMaintenanceAsync()
    {
        try
        {
            _logger.LogInformation("Performing PIN security maintenance");

            var expiredLockoutsCleared = await CleanupExpiredLockoutsAsync();
            var securityStats = await GetSecurityStatisticsAsync();
            var pinsRequiringUpdate = await GetPinsRequiringUpdateAsync();
            var recentFailures = await GetPinsWithRecentFailedAttemptsAsync();

            var maintenanceReport = new
            {
                MaintenancePerformedAt = DateTime.UtcNow,
                ExpiredLockoutsCleared = expiredLockoutsCleared,
                SecurityStatistics = securityStats,
                PinsRequiringUpdate = pinsRequiringUpdate.Count(),
                RecentFailuresCount = recentFailures.Count(),
                Recommendations = new List<string>()
            };

            _logger.LogInformation("Security maintenance completed. Cleared {ExpiredLockouts} lockouts",
                expiredLockoutsCleared);

            return maintenanceReport;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing security maintenance");
            throw;
        }
    }

    /// <summary>
    /// Gets audit trail for PIN security events.
    /// </summary>
    /// <param name="userId">The user ID to get audit trail for.</param>
    /// <param name="daysBack">Number of days to look back.</param>
    /// <returns>List of security events.</returns>
    public async Task<IEnumerable<object>> GetSecurityAuditTrailAsync(int userId, int daysBack = 30)
    {
        try
        {
            _logger.LogDebug("Getting security audit trail for user {UserId}, {Days} days back", userId, daysBack);

            var cutoffDate = DateTime.UtcNow.AddDays(-daysBack);
            var auditEvents = new List<object>();

            // Get PIN-related events
            var pinEvents = await _context.ParentalPins
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .Select(p => new
                {
                    EventType = "PIN",
                    PinId = p.Id,
                    Events = new[]
                    {
                        new { Type = "Created", Timestamp = (DateTime?)p.CreatedAt, Details = "PIN created" },
                        new { Type = "LastChanged", Timestamp = (DateTime?)p.LastChangedAt, Details = "PIN changed" },
                        new { Type = "LastSuccessfulVerification", Timestamp = p.LastSuccessfulVerificationAt, Details = "Successful verification" },
                        new { Type = "LastFailedAttempt", Timestamp = p.LastFailedAttemptAt, Details = $"Failed attempt (Total: {p.TotalFailedAttempts})" }
                    }.Where(e => e.Timestamp.HasValue && e.Timestamp.Value > cutoffDate)
                     .OrderByDescending(e => e.Timestamp)
                })
                .ToListAsync();

            foreach (var pinEvent in pinEvents)
            {
                auditEvents.AddRange(pinEvent.Events);
            }

            var sortedEvents = auditEvents.OrderByDescending(e => ((dynamic)e).Timestamp).ToList();

            _logger.LogDebug("Found {Count} security audit events for user {UserId}", sortedEvents.Count, userId);
            return sortedEvents;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting security audit trail for user {UserId}", userId);
            throw;
        }
    }
}