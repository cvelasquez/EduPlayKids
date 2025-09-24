using System.ComponentModel.DataAnnotations;
using EduPlayKids.Domain.Common;

namespace EduPlayKids.Domain.Entities;

/// <summary>
/// Represents parental PIN authentication data for secure access to parental controls.
/// Implements COPPA-compliant security measures for child protection features.
/// </summary>
public class ParentalPin : AuditableEntity
{
    /// <summary>
    /// Gets or sets the foreign key to the user (parent) this PIN belongs to.
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the hashed PIN value.
    /// PIN is stored as a salted hash for security.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string PinHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the hashed PIN value (alias for test compatibility).
    /// </summary>
    public string HashedPin
    {
        get => PinHash;
        set => PinHash = value;
    }

    /// <summary>
    /// Gets or sets the salt used for PIN hashing.
    /// Unique salt per PIN for enhanced security.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Salt { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the security question for PIN recovery.
    /// Used when parents forget their PIN.
    /// </summary>
    [Required]
    [StringLength(500)]
    public string SecurityQuestion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the hashed answer to the security question.
    /// Stored as hash for additional security.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string SecurityAnswerHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the salt used for security answer hashing.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string SecurityAnswerSalt { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the PIN was last changed.
    /// Used for security policy enforcement.
    /// </summary>
    public DateTime LastChangedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time of the last successful PIN verification.
    /// Used for security monitoring and audit purposes.
    /// </summary>
    public DateTime? LastSuccessfulVerificationAt { get; set; }

    /// <summary>
    /// Gets or sets the number of consecutive failed attempts.
    /// Used for implementing security lockout policies.
    /// </summary>
    public int FailedAttempts { get; set; } = 0;

    /// <summary>
    /// Gets or sets the date and time when the PIN entry was last locked due to failed attempts.
    /// Null if never locked or currently not locked.
    /// </summary>
    public DateTime? LockedUntil { get; set; }

    /// <summary>
    /// Gets or sets the date and time of the last failed attempt.
    /// Used for calculating lockout periods and security monitoring.
    /// </summary>
    public DateTime? LastFailedAttemptAt { get; set; }

    /// <summary>
    /// Gets or sets the total number of failed attempts since PIN creation.
    /// Used for long-term security monitoring.
    /// </summary>
    public int TotalFailedAttempts { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating whether the PIN is currently active.
    /// Allows for disabling PIN without deletion for troubleshooting.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the PIN complexity level that was enforced when created.
    /// Values: Basic, Enhanced, Maximum
    /// </summary>
    [StringLength(20)]
    public string ComplexityLevel { get; set; } = "Basic";

    /// <summary>
    /// Gets or sets additional security metadata as JSON.
    /// Flexible storage for future security enhancements:
    /// - Device fingerprinting
    /// - Biometric backup options
    /// - Time-based restrictions
    /// - Geographic restrictions
    /// </summary>
    public string? SecurityMetadata { get; set; }

    /// <summary>
    /// Navigation property: The user (parent) this PIN belongs to.
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Initializes a new instance of the ParentalPin class.
    /// </summary>
    public ParentalPin()
    {
        LastChangedAt = DateTime.UtcNow;
        IsActive = true;
        ComplexityLevel = "Basic";
    }

    /// <summary>
    /// Determines if the PIN is currently locked due to failed attempts.
    /// </summary>
    /// <returns>True if locked, false if available for entry.</returns>
    public bool IsCurrentlyLocked()
    {
        return LockedUntil.HasValue && LockedUntil.Value > DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the remaining lockout time in seconds.
    /// </summary>
    /// <returns>Seconds until PIN entry is available again, 0 if not locked.</returns>
    public int GetLockoutRemainingSeconds()
    {
        if (!IsCurrentlyLocked()) return 0;

        var remaining = LockedUntil!.Value - DateTime.UtcNow;
        return Math.Max(0, (int)remaining.TotalSeconds);
    }

    /// <summary>
    /// Records a successful PIN verification.
    /// Resets failed attempt counters and updates verification timestamp.
    /// </summary>
    public void RecordSuccessfulVerification()
    {
        LastSuccessfulVerificationAt = DateTime.UtcNow;
        FailedAttempts = 0;
        LockedUntil = null;
        LastFailedAttemptAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Records a failed PIN attempt and implements lockout policy.
    /// </summary>
    public void RecordFailedAttempt()
    {
        FailedAttempts++;
        TotalFailedAttempts++;
        LastFailedAttemptAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        // Implement progressive lockout policy
        // 3 attempts: 1 minute lockout
        // 5 attempts: 5 minute lockout
        // 10+ attempts: 30 minute lockout
        if (FailedAttempts >= 10)
        {
            LockedUntil = DateTime.UtcNow.AddMinutes(30);
        }
        else if (FailedAttempts >= 5)
        {
            LockedUntil = DateTime.UtcNow.AddMinutes(5);
        }
        else if (FailedAttempts >= 3)
        {
            LockedUntil = DateTime.UtcNow.AddMinutes(1);
        }
    }

    /// <summary>
    /// Updates the PIN hash and related security information.
    /// </summary>
    /// <param name="newPinHash">New hashed PIN value.</param>
    /// <param name="newSalt">New salt for the PIN.</param>
    public void UpdatePin(string newPinHash, string newSalt)
    {
        PinHash = newPinHash;
        Salt = newSalt;
        LastChangedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        // Reset security counters when PIN is changed
        FailedAttempts = 0;
        LockedUntil = null;
        LastFailedAttemptAt = null;
    }

    /// <summary>
    /// Updates the security question and answer.
    /// </summary>
    /// <param name="newQuestion">New security question.</param>
    /// <param name="newAnswerHash">New hashed security answer.</param>
    /// <param name="newAnswerSalt">New salt for the security answer.</param>
    public void UpdateSecurityQuestion(string newQuestion, string newAnswerHash, string newAnswerSalt)
    {
        SecurityQuestion = newQuestion;
        SecurityAnswerHash = newAnswerHash;
        SecurityAnswerSalt = newAnswerSalt;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets security summary for audit and monitoring purposes.
    /// </summary>
    /// <returns>Security summary object.</returns>
    public object GetSecuritySummary()
    {
        return new
        {
            IsActive,
            ComplexityLevel,
            LastChanged = LastChangedAt,
            LastSuccessfulVerification = LastSuccessfulVerificationAt,
            FailedAttempts,
            TotalFailedAttempts,
            IsCurrentlyLocked = IsCurrentlyLocked(),
            LockoutRemainingSeconds = GetLockoutRemainingSeconds(),
            HasSecurityQuestion = !string.IsNullOrEmpty(SecurityQuestion)
        };
    }

    /// <summary>
    /// Determines if the PIN should be updated based on age and security policies.
    /// </summary>
    /// <param name="maxAgeInDays">Maximum age in days before PIN update is recommended.</param>
    /// <returns>True if PIN update is recommended.</returns>
    public bool ShouldUpdatePin(int maxAgeInDays = 180)
    {
        var age = DateTime.UtcNow - LastChangedAt;
        return age.TotalDays > maxAgeInDays;
    }

    /// <summary>
    /// Validates that the PIN meets current security requirements.
    /// </summary>
    /// <returns>True if PIN meets current security standards.</returns>
    public bool MeetsSecurityRequirements()
    {
        return IsActive &&
               !string.IsNullOrEmpty(PinHash) &&
               !string.IsNullOrEmpty(Salt) &&
               !string.IsNullOrEmpty(SecurityQuestion) &&
               !string.IsNullOrEmpty(SecurityAnswerHash);
    }
}