using EduPlayKids.Domain.Common;

namespace EduPlayKids.Application.Interfaces;

/// <summary>
/// Service interface for managing parental PIN authentication and security.
/// Provides COPPA-compliant PIN management for parental controls access.
/// </summary>
public interface IParentalPinService
{
    /// <summary>
    /// Checks if a PIN has been set up for the current user.
    /// </summary>
    /// <returns>True if PIN is configured, false otherwise.</returns>
    Task<bool> IsPinSetupAsync();

    /// <summary>
    /// Sets up a new PIN for parental controls.
    /// </summary>
    /// <param name="pin">4-digit PIN string.</param>
    /// <param name="securityQuestion">Security question for PIN reset.</param>
    /// <param name="securityAnswer">Answer to security question.</param>
    /// <returns>Result indicating success or failure with details.</returns>
    Task<Result<bool>> SetupPinAsync(string pin, string securityQuestion, string securityAnswer);

    /// <summary>
    /// Verifies the provided PIN against the stored PIN.
    /// </summary>
    /// <param name="pin">PIN to verify.</param>
    /// <returns>Result indicating if PIN is valid.</returns>
    Task<Result<bool>> VerifyPinAsync(string pin);

    /// <summary>
    /// Changes the existing PIN to a new one.
    /// </summary>
    /// <param name="currentPin">Current PIN for verification.</param>
    /// <param name="newPin">New PIN to set.</param>
    /// <returns>Result indicating success or failure.</returns>
    Task<Result<bool>> ChangePinAsync(string currentPin, string newPin);

    /// <summary>
    /// Resets the PIN using security question verification.
    /// </summary>
    /// <param name="securityAnswer">Answer to the security question.</param>
    /// <param name="newPin">New PIN to set.</param>
    /// <returns>Result indicating success or failure.</returns>
    Task<Result<bool>> ResetPinAsync(string securityAnswer, string newPin);

    /// <summary>
    /// Gets the security question for PIN reset.
    /// </summary>
    /// <returns>Security question string, or null if not set.</returns>
    Task<string?> GetSecurityQuestionAsync();

    /// <summary>
    /// Validates PIN format and security requirements.
    /// </summary>
    /// <param name="pin">PIN to validate.</param>
    /// <returns>Validation result with error message if invalid.</returns>
    Result<bool> ValidatePinFormat(string pin);

    /// <summary>
    /// Records a failed PIN attempt for security purposes.
    /// </summary>
    /// <param name="attemptedPin">The PIN that was attempted (for audit log).</param>
    /// <returns>Task representing the async operation.</returns>
    Task RecordFailedAttemptAsync(string attemptedPin);

    /// <summary>
    /// Gets the number of recent failed attempts.
    /// </summary>
    /// <returns>Number of failed attempts in the last hour.</returns>
    Task<int> GetRecentFailedAttemptsAsync();

    /// <summary>
    /// Checks if the PIN entry is temporarily locked due to too many failed attempts.
    /// </summary>
    /// <returns>True if locked, false if available for entry.</returns>
    Task<bool> IsPinEntryLockedAsync();

    /// <summary>
    /// Gets the remaining lockout time in seconds.
    /// </summary>
    /// <returns>Seconds until PIN entry is available again, 0 if not locked.</returns>
    Task<int> GetLockoutRemainingSecondsAsync();
}