using EduPlayKids.Application.Interfaces;
using EduPlayKids.Application.Interfaces.Repositories;
using EduPlayKids.Domain.Common;
using EduPlayKids.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace EduPlayKids.Infrastructure.Services;

/// <summary>
/// Service implementation for managing parental PIN authentication and security.
/// Provides COPPA-compliant PIN management with secure hashing and comprehensive audit logging.
/// </summary>
public class ParentalPinService : IParentalPinService
{
    private readonly IParentalPinRepository _pinRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ILogger<ParentalPinService> _logger;

    // Security constants
    private const int SALT_SIZE = 32; // 256-bit salt
    private const int HASH_SIZE = 32; // 256-bit hash
    private const int ITERATIONS = 10000; // PBKDF2 iterations
    private const int MAX_FAILED_ATTEMPTS_BEFORE_LOCKOUT = 3;
    private const int LOCKOUT_DURATION_MINUTES = 5;

    public ParentalPinService(
        IParentalPinRepository pinRepository,
        IUserRepository userRepository,
        IAuditLogRepository auditLogRepository,
        ILogger<ParentalPinService> logger)
    {
        _pinRepository = pinRepository ?? throw new ArgumentNullException(nameof(pinRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _auditLogRepository = auditLogRepository ?? throw new ArgumentNullException(nameof(auditLogRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Checks if a PIN has been set up for the current user.
    /// </summary>
    /// <returns>True if PIN is configured, false otherwise.</returns>
    public async Task<bool> IsPinSetupAsync()
    {
        try
        {
            // Get current user - in a real app this would come from authentication context
            // For now, we'll assume user ID 1 (this should be injected via authentication service)
            var currentUserId = await GetCurrentUserIdAsync();
            if (!currentUserId.HasValue)
            {
                _logger.LogWarning("No current user found for PIN setup check");
                return false;
            }

            var hasPin = await _pinRepository.HasActivePinAsync(currentUserId.Value);
            _logger.LogDebug("PIN setup check for user {UserId}: {HasPin}", currentUserId.Value, hasPin);

            return hasPin;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking PIN setup status");
            return false;
        }
    }

    /// <summary>
    /// Sets up a new PIN for parental controls.
    /// </summary>
    /// <param name="pin">4-digit PIN string.</param>
    /// <param name="securityQuestion">Security question for PIN reset.</param>
    /// <param name="securityAnswer">Answer to security question.</param>
    /// <returns>Result indicating success or failure with details.</returns>
    public async Task<Result<bool>> SetupPinAsync(string pin, string securityQuestion, string securityAnswer)
    {
        try
        {
            _logger.LogInformation("Setting up new parental PIN");

            // Validate PIN format
            var validationResult = ValidatePinFormat(pin);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            // Validate security question and answer
            if (string.IsNullOrWhiteSpace(securityQuestion) || securityQuestion.Length < 10)
            {
                return Result<bool>.Failure("Security question must be at least 10 characters long");
            }

            if (string.IsNullOrWhiteSpace(securityAnswer) || securityAnswer.Length < 3)
            {
                return Result<bool>.Failure("Security answer must be at least 3 characters long");
            }

            var currentUserId = await GetCurrentUserIdAsync();
            if (!currentUserId.HasValue)
            {
                return Result<bool>.Failure("No authenticated user found");
            }

            // Deactivate any existing PINs
            await _pinRepository.DeactivateAllPinsForUserAsync(currentUserId.Value);

            // Generate salt and hash for PIN
            var (pinHash, pinSalt) = HashPassword(pin);
            var (answerHash, answerSalt) = HashPassword(securityAnswer.ToLowerInvariant());

            // Create new PIN record
            var newPin = new ParentalPin
            {
                UserId = currentUserId.Value,
                PinHash = pinHash,
                Salt = pinSalt,
                SecurityQuestion = securityQuestion,
                SecurityAnswerHash = answerHash,
                SecurityAnswerSalt = answerSalt,
                LastChangedAt = DateTime.UtcNow,
                IsActive = true,
                ComplexityLevel = "Basic"
            };

            await _pinRepository.AddAsync(newPin);
            await _auditLogRepository.LogUserActionAsync(currentUserId.Value, "PIN Setup", "Parental PIN configured successfully", "ParentalPin");

            _logger.LogInformation("Parental PIN setup completed successfully for user {UserId}", currentUserId.Value);
            return Result<bool>.Success(true, "PIN setup completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting up parental PIN");
            return Result<bool>.Failure("Failed to setup PIN. Please try again.");
        }
    }

    /// <summary>
    /// Verifies the provided PIN against the stored PIN.
    /// </summary>
    /// <param name="pin">PIN to verify.</param>
    /// <returns>Result indicating if PIN is valid.</returns>
    public async Task<Result<bool>> VerifyPinAsync(string pin)
    {
        try
        {
            var currentUserId = await GetCurrentUserIdAsync();
            if (!currentUserId.HasValue)
            {
                return Result<bool>.Failure("No authenticated user found");
            }

            var pinRecord = await _pinRepository.GetActiveByUserIdAsync(currentUserId.Value);
            if (pinRecord == null)
            {
                _logger.LogWarning("PIN verification attempted but no active PIN found for user {UserId}", currentUserId.Value);
                return Result<bool>.Failure("No PIN configured");
            }

            // Check if PIN entry is locked
            if (pinRecord.IsCurrentlyLocked())
            {
                var remainingSeconds = pinRecord.GetLockoutRemainingSeconds();
                _logger.LogWarning("PIN verification attempted while locked for user {UserId}. Remaining: {Seconds}s",
                    currentUserId.Value, remainingSeconds);

                await _auditLogRepository.LogUserActionAsync(currentUserId.Value, "PIN Verification",
                    $"Verification attempted while locked (remaining: {remainingSeconds}s)", "ParentalPin");

                return Result<bool>.Failure($"PIN is locked. Try again in {remainingSeconds} seconds.");
            }

            // Verify PIN
            var isValid = VerifyPassword(pin, pinRecord.PinHash, pinRecord.Salt);

            if (isValid)
            {
                // Successful verification
                await _pinRepository.UpdateLastSuccessfulVerificationAsync(pinRecord.Id);
                await _auditLogRepository.LogUserActionAsync(currentUserId.Value, "PIN Verification",
                    "PIN verified successfully", "ParentalPin");

                _logger.LogInformation("PIN verified successfully for user {UserId}", currentUserId.Value);
                return Result<bool>.Success(true, "PIN verified successfully");
            }
            else
            {
                // Failed verification
                await _pinRepository.RecordFailedAttemptAsync(pinRecord.Id);
                await RecordFailedAttemptAsync(pin); // This calls the interface method

                // Refresh the record to get updated failure count
                pinRecord = await _pinRepository.GetActiveByUserIdAsync(currentUserId.Value);

                await _auditLogRepository.LogUserActionAsync(currentUserId.Value, "PIN Verification",
                    $"PIN verification failed (attempt #{pinRecord?.FailedAttempts ?? 0})", "ParentalPin");

                _logger.LogWarning("PIN verification failed for user {UserId}. Failed attempts: {FailedAttempts}",
                    currentUserId.Value, pinRecord?.FailedAttempts ?? 0);

                if (pinRecord?.IsCurrentlyLocked() == true)
                {
                    var lockoutMinutes = pinRecord.GetLockoutRemainingSeconds() / 60;
                    return Result<bool>.Failure($"Too many failed attempts. PIN locked for {lockoutMinutes} minutes.");
                }

                return Result<bool>.Failure("Invalid PIN. Please try again.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying PIN");
            return Result<bool>.Failure("PIN verification failed. Please try again.");
        }
    }

    /// <summary>
    /// Changes the existing PIN to a new one.
    /// </summary>
    /// <param name="currentPin">Current PIN for verification.</param>
    /// <param name="newPin">New PIN to set.</param>
    /// <returns>Result indicating success or failure.</returns>
    public async Task<Result<bool>> ChangePinAsync(string currentPin, string newPin)
    {
        try
        {
            _logger.LogInformation("Changing parental PIN");

            // First verify current PIN
            var verificationResult = await VerifyPinAsync(currentPin);
            if (!verificationResult.IsSuccess)
            {
                return Result<bool>.Failure("Current PIN is incorrect");
            }

            // Validate new PIN format
            var validationResult = ValidatePinFormat(newPin);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            // Check that new PIN is different
            if (currentPin == newPin)
            {
                return Result<bool>.Failure("New PIN must be different from current PIN");
            }

            var currentUserId = await GetCurrentUserIdAsync();
            if (!currentUserId.HasValue)
            {
                return Result<bool>.Failure("No authenticated user found");
            }

            var pinRecord = await _pinRepository.GetActiveByUserIdAsync(currentUserId.Value);
            if (pinRecord == null)
            {
                return Result<bool>.Failure("No active PIN found");
            }

            // Generate new hash and salt
            var (newPinHash, newPinSalt) = HashPassword(newPin);

            // Update PIN
            pinRecord.UpdatePin(newPinHash, newPinSalt);
            await _pinRepository.UpdateAsync(pinRecord);

            await _auditLogRepository.LogUserActionAsync(currentUserId.Value, "PIN Change",
                "PIN changed successfully", "ParentalPin");

            _logger.LogInformation("PIN changed successfully for user {UserId}", currentUserId.Value);
            return Result<bool>.Success(true, "PIN changed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing PIN");
            return Result<bool>.Failure("Failed to change PIN. Please try again.");
        }
    }

    /// <summary>
    /// Resets the PIN using security question verification.
    /// </summary>
    /// <param name="securityAnswer">Answer to the security question.</param>
    /// <param name="newPin">New PIN to set.</param>
    /// <returns>Result indicating success or failure.</returns>
    public async Task<Result<bool>> ResetPinAsync(string securityAnswer, string newPin)
    {
        try
        {
            _logger.LogInformation("Resetting parental PIN via security question");

            var currentUserId = await GetCurrentUserIdAsync();
            if (!currentUserId.HasValue)
            {
                return Result<bool>.Failure("No authenticated user found");
            }

            var pinRecord = await _pinRepository.GetActiveByUserIdAsync(currentUserId.Value);
            if (pinRecord == null)
            {
                return Result<bool>.Failure("No PIN configured");
            }

            // Verify security answer
            var isAnswerValid = VerifyPassword(securityAnswer.ToLowerInvariant(),
                pinRecord.SecurityAnswerHash, pinRecord.SecurityAnswerSalt);

            if (!isAnswerValid)
            {
                await _auditLogRepository.LogUserActionAsync(currentUserId.Value, "PIN Reset",
                    "PIN reset failed - incorrect security answer", "ParentalPin");
                _logger.LogWarning("PIN reset failed - incorrect security answer for user {UserId}", currentUserId.Value);
                return Result<bool>.Failure("Incorrect security answer");
            }

            // Validate new PIN format
            var validationResult = ValidatePinFormat(newPin);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            // Generate new hash and salt
            var (newPinHash, newPinSalt) = HashPassword(newPin);

            // Update PIN and reset security counters
            pinRecord.UpdatePin(newPinHash, newPinSalt);
            await _pinRepository.UpdateAsync(pinRecord);

            await _auditLogRepository.LogUserActionAsync(currentUserId.Value, "PIN Reset",
                "PIN reset successfully via security question", "ParentalPin");

            _logger.LogInformation("PIN reset successfully for user {UserId}", currentUserId.Value);
            return Result<bool>.Success(true, "PIN reset successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting PIN");
            return Result<bool>.Failure("Failed to reset PIN. Please try again.");
        }
    }

    /// <summary>
    /// Gets the security question for PIN reset.
    /// </summary>
    /// <returns>Security question string, or null if not set.</returns>
    public async Task<string?> GetSecurityQuestionAsync()
    {
        try
        {
            var currentUserId = await GetCurrentUserIdAsync();
            if (!currentUserId.HasValue)
            {
                return null;
            }

            var pinRecord = await _pinRepository.GetActiveByUserIdAsync(currentUserId.Value);
            return pinRecord?.SecurityQuestion;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting security question");
            return null;
        }
    }

    /// <summary>
    /// Validates PIN format and security requirements.
    /// </summary>
    /// <param name="pin">PIN to validate.</param>
    /// <returns>Validation result with error message if invalid.</returns>
    public Result<bool> ValidatePinFormat(string pin)
    {
        if (string.IsNullOrWhiteSpace(pin))
        {
            return Result<bool>.Failure("PIN is required");
        }

        if (pin.Length != 4)
        {
            return Result<bool>.Failure("PIN must be exactly 4 digits");
        }

        if (!pin.All(char.IsDigit))
        {
            return Result<bool>.Failure("PIN must contain only numbers");
        }

        // Basic security checks
        if (pin == "0000" || pin == "1111" || pin == "2222" || pin == "3333" ||
            pin == "4444" || pin == "5555" || pin == "6666" || pin == "7777" ||
            pin == "8888" || pin == "9999")
        {
            return Result<bool>.Failure("PIN cannot be all the same digit");
        }

        if (pin == "1234" || pin == "4321" || pin == "0123" || pin == "9876")
        {
            return Result<bool>.Failure("PIN cannot be a simple sequence");
        }

        return Result<bool>.Success(true, "PIN format is valid");
    }

    /// <summary>
    /// Records a failed PIN attempt for security purposes.
    /// </summary>
    /// <param name="attemptedPin">The PIN that was attempted (for audit log).</param>
    /// <returns>Task representing the async operation.</returns>
    public async Task RecordFailedAttemptAsync(string attemptedPin)
    {
        try
        {
            var currentUserId = await GetCurrentUserIdAsync();
            if (currentUserId.HasValue)
            {
                // Log failed attempt (without storing the actual PIN for security)
                await _auditLogRepository.LogUserActionAsync(currentUserId.Value, "PIN Failed Attempt",
                    "Failed PIN attempt recorded", "ParentalPin");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording failed PIN attempt");
        }
    }

    /// <summary>
    /// Gets the number of recent failed attempts.
    /// </summary>
    /// <returns>Number of failed attempts in the last hour.</returns>
    public async Task<int> GetRecentFailedAttemptsAsync()
    {
        try
        {
            var currentUserId = await GetCurrentUserIdAsync();
            if (!currentUserId.HasValue)
            {
                return 0;
            }

            var pinRecord = await _pinRepository.GetActiveByUserIdAsync(currentUserId.Value);
            return pinRecord?.FailedAttempts ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent failed attempts");
            return 0;
        }
    }

    /// <summary>
    /// Checks if the PIN entry is temporarily locked due to too many failed attempts.
    /// </summary>
    /// <returns>True if locked, false if available for entry.</returns>
    public async Task<bool> IsPinEntryLockedAsync()
    {
        try
        {
            var currentUserId = await GetCurrentUserIdAsync();
            if (!currentUserId.HasValue)
            {
                return false;
            }

            var pinRecord = await _pinRepository.GetActiveByUserIdAsync(currentUserId.Value);
            return pinRecord?.IsCurrentlyLocked() ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking PIN entry lock status");
            return false;
        }
    }

    /// <summary>
    /// Gets the remaining lockout time in seconds.
    /// </summary>
    /// <returns>Seconds until PIN entry is available again, 0 if not locked.</returns>
    public async Task<int> GetLockoutRemainingSecondsAsync()
    {
        try
        {
            var currentUserId = await GetCurrentUserIdAsync();
            if (!currentUserId.HasValue)
            {
                return 0;
            }

            var pinRecord = await _pinRepository.GetActiveByUserIdAsync(currentUserId.Value);
            return pinRecord?.GetLockoutRemainingSeconds() ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lockout remaining time");
            return 0;
        }
    }

    #region Private Helper Methods

    /// <summary>
    /// Gets the current user ID (placeholder implementation).
    /// In a real application, this would come from authentication context.
    /// </summary>
    /// <returns>Current user ID or null if not authenticated.</returns>
    private async Task<int?> GetCurrentUserIdAsync()
    {
        try
        {
            // TODO: Replace with actual authentication service
            // For now, return the first user or create a default user
            var users = await _userRepository.GetAllAsync();
            var firstUser = users.FirstOrDefault();

            if (firstUser != null)
            {
                return firstUser.Id;
            }

            // Create a default user for development/testing
            var defaultUser = new User
            {
                FullName = "Default Parent",
                Email = "parent@eduplaykids.com",
                PreferredLanguage = "en"
            };

            await _userRepository.AddAsync(defaultUser);
            return defaultUser.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user ID");
            return null;
        }
    }

    /// <summary>
    /// Hashes a password using PBKDF2 with a random salt.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>Tuple containing the hash and salt as base64 strings.</returns>
    private (string hash, string salt) HashPassword(string password)
    {
        // Generate a random salt
        var salt = new byte[SALT_SIZE];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Hash the password
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            ITERATIONS,
            HashAlgorithmName.SHA256,
            HASH_SIZE);

        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    /// <summary>
    /// Verifies a password against a stored hash and salt.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="storedHash">The stored hash as base64 string.</param>
    /// <param name="storedSalt">The stored salt as base64 string.</param>
    /// <returns>True if password matches, false otherwise.</returns>
    private bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        try
        {
            var salt = Convert.FromBase64String(storedSalt);
            var hash = Convert.FromBase64String(storedHash);

            var testHash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                ITERATIONS,
                HashAlgorithmName.SHA256,
                HASH_SIZE);

            return CryptographicOperations.FixedTimeEquals(hash, testHash);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying password");
            return false;
        }
    }

    #endregion
}