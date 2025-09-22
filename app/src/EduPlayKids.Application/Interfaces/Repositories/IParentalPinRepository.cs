using EduPlayKids.Domain.Entities;
using EduPlayKids.Domain.Common;

namespace EduPlayKids.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for managing ParentalPin entity data access.
/// Provides secure PIN storage and retrieval operations for parental controls.
/// </summary>
public interface IParentalPinRepository : IGenericRepository<ParentalPin>
{
    /// <summary>
    /// Gets the active PIN record for a specific user.
    /// </summary>
    /// <param name="userId">The user ID to get PIN for.</param>
    /// <returns>The active PIN record, or null if not found.</returns>
    Task<ParentalPin?> GetActiveByUserIdAsync(int userId);

    /// <summary>
    /// Checks if a user has an active PIN configured.
    /// </summary>
    /// <param name="userId">The user ID to check.</param>
    /// <returns>True if user has an active PIN.</returns>
    Task<bool> HasActivePinAsync(int userId);

    /// <summary>
    /// Gets all PIN records for a user (including inactive ones for history).
    /// </summary>
    /// <param name="userId">The user ID to get PIN history for.</param>
    /// <returns>List of all PIN records for the user.</returns>
    Task<IEnumerable<ParentalPin>> GetPinHistoryAsync(int userId);

    /// <summary>
    /// Gets all currently locked PIN records.
    /// Used for security monitoring and cleanup operations.
    /// </summary>
    /// <returns>List of locked PIN records.</returns>
    Task<IEnumerable<ParentalPin>> GetLockedPinsAsync();

    /// <summary>
    /// Gets PIN records with recent failed attempts for security monitoring.
    /// </summary>
    /// <param name="hoursBack">Hours to look back for failed attempts.</param>
    /// <returns>List of PIN records with recent failed attempts.</returns>
    Task<IEnumerable<ParentalPin>> GetPinsWithRecentFailedAttemptsAsync(int hoursBack = 24);

    /// <summary>
    /// Deactivates all existing PINs for a user.
    /// Used when setting up a new PIN to ensure only one active PIN per user.
    /// </summary>
    /// <param name="userId">The user ID to deactivate PINs for.</param>
    /// <returns>Task representing the async operation.</returns>
    Task DeactivateAllPinsForUserAsync(int userId);

    /// <summary>
    /// Cleans up expired lockouts automatically.
    /// Called during application startup or maintenance routines.
    /// </summary>
    /// <returns>Number of records cleaned up.</returns>
    Task<int> CleanupExpiredLockoutsAsync();

    /// <summary>
    /// Gets security statistics for monitoring and reporting.
    /// </summary>
    /// <param name="userId">Optional user ID to filter statistics.</param>
    /// <returns>Security statistics object.</returns>
    Task<object> GetSecurityStatisticsAsync(int? userId = null);

    /// <summary>
    /// Updates the last successful verification timestamp.
    /// Called after successful PIN verification.
    /// </summary>
    /// <param name="pinId">The PIN record ID.</param>
    /// <returns>Task representing the async operation.</returns>
    Task UpdateLastSuccessfulVerificationAsync(int pinId);

    /// <summary>
    /// Records a failed attempt and updates lockout status if needed.
    /// Implements progressive lockout policy.
    /// </summary>
    /// <param name="pinId">The PIN record ID.</param>
    /// <returns>Task representing the async operation.</returns>
    Task RecordFailedAttemptAsync(int pinId);

    /// <summary>
    /// Resets failed attempt counters for a PIN record.
    /// Called after successful verification or PIN reset.
    /// </summary>
    /// <param name="pinId">The PIN record ID.</param>
    /// <returns>Task representing the async operation.</returns>
    Task ResetFailedAttemptsAsync(int pinId);

    /// <summary>
    /// Gets all PIN records that should be updated based on age policy.
    /// </summary>
    /// <param name="maxAgeInDays">Maximum age in days before update is recommended.</param>
    /// <returns>List of PIN records that should be updated.</returns>
    Task<IEnumerable<ParentalPin>> GetPinsRequiringUpdateAsync(int maxAgeInDays = 180);

    /// <summary>
    /// Performs bulk security maintenance operations.
    /// Called during scheduled maintenance for security hygiene.
    /// </summary>
    /// <returns>Maintenance report object.</returns>
    Task<object> PerformSecurityMaintenanceAsync();

    /// <summary>
    /// Gets audit trail for PIN security events.
    /// </summary>
    /// <param name="userId">The user ID to get audit trail for.</param>
    /// <param name="daysBack">Number of days to look back.</param>
    /// <returns>List of security events.</returns>
    Task<IEnumerable<object>> GetSecurityAuditTrailAsync(int userId, int daysBack = 30);
}