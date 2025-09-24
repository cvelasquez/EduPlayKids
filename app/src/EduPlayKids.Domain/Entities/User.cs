using System.ComponentModel.DataAnnotations;
using EduPlayKids.Domain.Common;

namespace EduPlayKids.Domain.Entities;

/// <summary>
/// Represents a parent or guardian user in the EduPlayKids application.
/// Manages authentication, profile information, and child relationships.
/// Implements COPPA-compliant parent verification requirements.
/// </summary>
public class User : AuditableEntity
{
    /// <summary>
    /// Gets or sets the user's email address.
    /// Used for account identification and communication.
    /// </summary>
    [Required]
    [StringLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's full name.
    /// Required for COPPA compliance and account verification.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the hashed password for authentication.
    /// Stored using secure hashing algorithms.
    /// </summary>
    [Required]
    [StringLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the 4-digit PIN for parental controls.
    /// Used to access parent dashboard and premium features.
    /// </summary>
    [Required]
    [StringLength(4, MinimumLength = 4)]
    public string ParentalPin { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's preferred language (Spanish/English).
    /// Affects app interface and audio instructions.
    /// </summary>
    [Required]
    [StringLength(10)]
    public string PreferredLanguage { get; set; } = "es"; // Default to Spanish

    /// <summary>
    /// Gets or sets the user's country code for localization.
    /// Used for curriculum alignment and cultural adaptation.
    /// </summary>
    [StringLength(3)]
    public string? CountryCode { get; set; }

    /// <summary>
    /// Gets or sets the user's timezone for session tracking.
    /// Important for usage analytics and screen time management.
    /// </summary>
    [StringLength(50)]
    public string? TimeZone { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user has verified their email.
    /// Required for COPPA compliance and account security.
    /// </summary>
    public bool IsEmailVerified { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the user has an active premium subscription.
    /// Affects feature access and content limitations.
    /// </summary>
    public bool IsPremium { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the user account is active.
    /// Inactive accounts cannot log in or access the application.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the date when the user's premium subscription expires.
    /// Null for non-premium users or unlimited subscriptions.
    /// </summary>
    public DateTime? PremiumExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the date when the user last accessed the application.
    /// Used for engagement analytics and account activity tracking.
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Gets or sets the date when the user's free trial period expires.
    /// All new users get a 3-day free trial period.
    /// </summary>
    public DateTime FreeTrialExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether parental controls are enabled.
    /// Affects content access and usage time limitations.
    /// </summary>
    public bool ParentalControlsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the daily usage limit in minutes for all children.
    /// Zero means no limit. Used for healthy screen time management.
    /// </summary>
    public int DailyUsageLimitMinutes { get; set; } = 60; // Default 1 hour

    /// <summary>
    /// Navigation property: Collection of children associated with this user.
    /// Supports multi-child families with individual progress tracking.
    /// </summary>
    public virtual ICollection<Child> Children { get; set; } = new List<Child>();

    /// <summary>
    /// Navigation property: Collection of audit logs for this user.
    /// Required for COPPA compliance and security monitoring.
    /// </summary>
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    /// <summary>
    /// Navigation property: User's subscription information.
    /// Manages premium features and billing information.
    /// </summary>
    public virtual Subscription? Subscription { get; set; }

    /// <summary>
    /// Initializes a new instance of the User class.
    /// Sets default values for new user registration.
    /// </summary>
    public User()
    {
        FreeTrialExpiresAt = DateTime.UtcNow.AddDays(3);
        PreferredLanguage = "es";
        ParentalControlsEnabled = true;
        DailyUsageLimitMinutes = 60;
    }

    /// <summary>
    /// Determines if the user's free trial is still active.
    /// </summary>
    /// <returns>True if the free trial is active, false otherwise.</returns>
    public bool IsInFreeTrial()
    {
        return DateTime.UtcNow <= FreeTrialExpiresAt;
    }

    /// <summary>
    /// Determines if the user has access to premium features.
    /// Considers both free trial and premium subscription status.
    /// </summary>
    /// <returns>True if the user has premium access, false otherwise.</returns>
    public bool HasPremiumAccess()
    {
        return IsInFreeTrial() || (IsPremium && (PremiumExpiresAt == null || DateTime.UtcNow <= PremiumExpiresAt));
    }

    /// <summary>
    /// Gets the name (alias for FullName for ViewModel compatibility).
    /// </summary>
    public string Name => FullName;

    /// <summary>
    /// Validates the provided PIN against the stored parental PIN.
    /// </summary>
    /// <param name="pin">The PIN to validate.</param>
    /// <returns>True if the PIN is correct, false otherwise.</returns>
    public bool ValidateParentalPin(string pin)
    {
        return ParentalPin == pin;
    }
}