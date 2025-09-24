using System.ComponentModel.DataAnnotations;
using EduPlayKids.Domain.Common;

namespace EduPlayKids.Domain.Entities;

/// <summary>
/// Represents an audit log entry for compliance tracking and child safety monitoring.
/// Essential for COPPA compliance, security monitoring, and data protection requirements.
/// Tracks all significant actions and data access in the EduPlayKids application.
/// </summary>
public class AuditLog : BaseEntity
{
    /// <summary>
    /// Gets or sets the foreign key to the user this audit log relates to.
    /// Links audit events to the responsible parent/guardian account.
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the child this audit log relates to.
    /// Links audit events to the specific child learner when applicable.
    /// </summary>
    public int? ChildId { get; set; }

    /// <summary>
    /// Gets or sets the action that was performed.
    /// Standardized action identifiers for consistent audit tracking.
    /// Examples: LOGIN, LOGOUT, DATA_ACCESS, SETTINGS_CHANGE, CONTENT_VIEW, etc.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category of the audit event.
    /// High-level classification for audit event organization.
    /// Categories: Authentication, Data_Access, Privacy, Safety, Content, Settings, Billing
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the severity level of the audit event.
    /// Indicates the importance and potential risk level.
    /// Levels: Info, Warning, Error, Critical, Security
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Severity { get; set; } = "Info";

    /// <summary>
    /// Gets or sets the detailed description of the audit event.
    /// Human-readable explanation of what occurred.
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the audit log details.
    /// Alias for Description property for backward compatibility.
    /// </summary>
    public string? Details => Description;

    /// <summary>
    /// Gets the audit log event date.
    /// Alias for CreatedAt property for backward compatibility.
    /// </summary>
    public DateTime EventDate => CreatedAt;

    /// <summary>
    /// Gets or sets the entity type that was affected by the action.
    /// References the domain entity involved in the audit event.
    /// Examples: User, Child, Activity, Session, Settings, etc.
    /// </summary>
    [StringLength(50)]
    public string? EntityType { get; set; }

    /// <summary>
    /// Gets or sets the ID of the entity that was affected.
    /// References the specific entity instance involved.
    /// </summary>
    public int? EntityId { get; set; }

    /// <summary>
    /// Gets or sets the previous values before the action (for change tracking).
    /// JSON representation of the entity state before modification.
    /// Critical for compliance and rollback capabilities.
    /// </summary>
    public string? OldValues { get; set; }

    /// <summary>
    /// Gets or sets the new values after the action (for change tracking).
    /// JSON representation of the entity state after modification.
    /// Enables audit trail reconstruction and compliance verification.
    /// </summary>
    public string? NewValues { get; set; }

    /// <summary>
    /// Gets or sets the IP address from which the action was performed.
    /// Important for security monitoring and geographic compliance.
    /// Anonymized for COPPA compliance when related to children.
    /// </summary>
    [StringLength(45)] // IPv6 max length
    public string? IpAddress { get; set; }

    /// <summary>
    /// Gets or sets the user agent string of the client application.
    /// Device and application information for security and compatibility tracking.
    /// </summary>
    [StringLength(500)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// Gets or sets the application version when the action occurred.
    /// Critical for correlating issues with specific app releases.
    /// </summary>
    [StringLength(20)]
    public string? AppVersion { get; set; }

    /// <summary>
    /// Gets or sets the platform where the action occurred.
    /// Values: Android, iOS, Windows, Web
    /// </summary>
    [StringLength(20)]
    public string? Platform { get; set; }

    /// <summary>
    /// Gets or sets the device identifier (anonymized for privacy).
    /// COPPA-compliant device tracking without personal identification.
    /// </summary>
    [StringLength(100)]
    public string? DeviceId { get; set; }

    /// <summary>
    /// Gets or sets the session ID when the action occurred.
    /// Links audit events to specific user sessions for context.
    /// </summary>
    [StringLength(100)]
    public string? SessionId { get; set; }

    /// <summary>
    /// Gets or sets the request ID for tracing distributed operations.
    /// Enables correlation across multiple system components.
    /// </summary>
    [StringLength(100)]
    public string? RequestId { get; set; }

    /// <summary>
    /// Gets or sets additional context data as JSON.
    /// Flexible structure for action-specific information:
    /// - API endpoints accessed
    /// - Query parameters
    /// - Business context
    /// - Compliance-relevant details
    /// </summary>
    public string? ContextData { get; set; }

    /// <summary>
    /// Gets or sets compliance-related metadata as JSON.
    /// Specific data required for regulatory compliance:
    /// - COPPA consent status
    /// - Data retention requirements
    /// - Geographic jurisdiction
    /// - Privacy policy version
    /// </summary>
    public string? ComplianceData { get; set; }

    /// <summary>
    /// Gets or sets the result or outcome of the action.
    /// Values: Success, Failure, Partial, Blocked, Unauthorized
    /// </summary>
    [StringLength(20)]
    public string Result { get; set; } = "Success";

    /// <summary>
    /// Gets or sets the error message if the action failed.
    /// Detailed error information for debugging and compliance.
    /// </summary>
    [StringLength(1000)]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the duration of the action in milliseconds.
    /// Performance monitoring and anomaly detection.
    /// </summary>
    public int? DurationMs { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this audit log has been reviewed.
    /// For compliance review workflows and incident investigation.
    /// </summary>
    public bool IsReviewed { get; set; } = false;

    /// <summary>
    /// Gets or sets the ID of the user who reviewed this audit log.
    /// Tracks compliance review responsibility.
    /// </summary>
    public int? ReviewedBy { get; set; }

    /// <summary>
    /// Gets or sets the date when this audit log was reviewed.
    /// Compliance review timestamp.
    /// </summary>
    public DateTime? ReviewedAt { get; set; }

    /// <summary>
    /// Gets or sets notes added during audit review.
    /// Compliance officer or administrator notes.
    /// </summary>
    [StringLength(1000)]
    public string? ReviewNotes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this log requires special attention.
    /// Flags suspicious or high-risk activities for investigation.
    /// </summary>
    public bool RequiresAttention { get; set; } = false;

    /// <summary>
    /// Gets or sets data retention requirements for this audit log.
    /// How long this log must be retained for compliance.
    /// Values: Standard, Extended, Legal_Hold, Permanent
    /// </summary>
    [StringLength(20)]
    public string RetentionClass { get; set; } = "Standard";

    /// <summary>
    /// Gets or sets the date when this audit log can be safely deleted.
    /// Automatic data lifecycle management for compliance.
    /// </summary>
    public DateTime? RetentionExpiry { get; set; }

    /// <summary>
    /// Navigation property: The user this audit log relates to.
    /// </summary>
    public virtual User? User { get; set; }

    /// <summary>
    /// Navigation property: The child this audit log relates to.
    /// </summary>
    public virtual Child? Child { get; set; }

    /// <summary>
    /// Initializes a new instance of the AuditLog class.
    /// Sets default values for new audit log entries.
    /// </summary>
    public AuditLog()
    {
        Severity = "Info";
        Result = "Success";
        IsReviewed = false;
        RequiresAttention = false;
        RetentionClass = "Standard";

        // Set default retention expiry based on retention class
        SetDefaultRetentionExpiry();
    }

    /// <summary>
    /// Creates an audit log entry for authentication events.
    /// </summary>
    /// <param name="userId">The user ID attempting authentication.</param>
    /// <param name="action">The authentication action (LOGIN, LOGOUT, LOGIN_FAILED).</param>
    /// <param name="result">The result of the authentication attempt.</param>
    /// <param name="ipAddress">The client IP address.</param>
    /// <param name="userAgent">The client user agent.</param>
    /// <returns>A configured audit log entry.</returns>
    public static AuditLog CreateAuthenticationLog(int userId, string action, string result,
        string? ipAddress = null, string? userAgent = null)
    {
        return new AuditLog
        {
            UserId = userId,
            Action = action,
            Category = "Authentication",
            Severity = result == "Success" ? "Info" : "Warning",
            Description = $"User authentication: {action}",
            Result = result,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            RequiresAttention = result != "Success"
        };
    }

    /// <summary>
    /// Creates an audit log entry for data access events.
    /// </summary>
    /// <param name="userId">The user accessing the data.</param>
    /// <param name="childId">The child whose data is being accessed.</param>
    /// <param name="entityType">The type of entity being accessed.</param>
    /// <param name="entityId">The ID of the entity being accessed.</param>
    /// <param name="action">The access action performed.</param>
    /// <returns>A configured audit log entry.</returns>
    public static AuditLog CreateDataAccessLog(int? userId, int? childId, string entityType,
        int entityId, string action)
    {
        return new AuditLog
        {
            UserId = userId,
            ChildId = childId,
            Action = action,
            Category = "Data_Access",
            Severity = "Info",
            Description = $"Data access: {action} on {entityType}",
            EntityType = entityType,
            EntityId = entityId,
            RetentionClass = childId.HasValue ? "Extended" : "Standard" // Extended retention for child data
        };
    }

    /// <summary>
    /// Creates an audit log entry for privacy-related events.
    /// </summary>
    /// <param name="userId">The user ID involved in the privacy event.</param>
    /// <param name="childId">The child ID if applicable.</param>
    /// <param name="action">The privacy action performed.</param>
    /// <param name="description">Detailed description of the privacy event.</param>
    /// <returns>A configured audit log entry.</returns>
    public static AuditLog CreatePrivacyLog(int? userId, int? childId, string action, string description)
    {
        return new AuditLog
        {
            UserId = userId,
            ChildId = childId,
            Action = action,
            Category = "Privacy",
            Severity = "Info",
            Description = description,
            RetentionClass = "Legal_Hold", // Privacy events need extended retention
            RequiresAttention = true
        };
    }

    /// <summary>
    /// Creates an audit log entry for security events.
    /// </summary>
    /// <param name="userId">The user ID involved in the security event.</param>
    /// <param name="action">The security action or violation.</param>
    /// <param name="severity">The severity level of the security event.</param>
    /// <param name="description">Detailed description of the security event.</param>
    /// <param name="contextData">Additional context information.</param>
    /// <returns>A configured audit log entry.</returns>
    public static AuditLog CreateSecurityLog(int? userId, string action, string severity,
        string description, object? contextData = null)
    {
        return new AuditLog
        {
            UserId = userId,
            Action = action,
            Category = "Security",
            Severity = severity,
            Description = description,
            ContextData = contextData != null ? System.Text.Json.JsonSerializer.Serialize(contextData) : null,
            RetentionClass = "Legal_Hold",
            RequiresAttention = severity != "Info"
        };
    }

    /// <summary>
    /// Determines if this audit log is subject to COPPA compliance requirements.
    /// </summary>
    /// <returns>True if COPPA compliance applies, false otherwise.</returns>
    public bool IsSubjectToCoppa()
    {
        return ChildId.HasValue || Category == "Privacy" || Action.Contains("CHILD");
    }

    /// <summary>
    /// Marks this audit log as reviewed by a compliance officer.
    /// </summary>
    /// <param name="reviewerId">The ID of the reviewing user.</param>
    /// <param name="notes">Review notes or comments.</param>
    public void MarkAsReviewed(int reviewerId, string? notes = null)
    {
        IsReviewed = true;
        ReviewedBy = reviewerId;
        ReviewedAt = DateTime.UtcNow;
        ReviewNotes = notes;
        RequiresAttention = false; // Clear attention flag after review
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Flags this audit log as requiring special attention.
    /// </summary>
    /// <param name="reason">The reason for requiring attention.</param>
    public void FlagForAttention(string reason)
    {
        RequiresAttention = true;
        ReviewNotes = ReviewNotes != null ? $"{ReviewNotes}\nFlagged: {reason}" : $"Flagged: {reason}";
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the retention expiry date based on the retention class.
    /// </summary>
    private void SetDefaultRetentionExpiry()
    {
        var baseDate = DateTime.UtcNow;

        RetentionExpiry = RetentionClass switch
        {
            "Standard" => baseDate.AddYears(1),
            "Extended" => baseDate.AddYears(3),
            "Legal_Hold" => baseDate.AddYears(7),
            "Permanent" => null, // Never expires
            _ => baseDate.AddYears(1)
        };
    }

    /// <summary>
    /// Determines if this audit log can be safely deleted based on retention requirements.
    /// </summary>
    /// <returns>True if the log can be deleted, false otherwise.</returns>
    public bool CanBeDeleted()
    {
        if (RetentionExpiry == null) return false; // Permanent retention
        if (RequiresAttention && !IsReviewed) return false; // Pending review

        return DateTime.UtcNow >= RetentionExpiry;
    }

    /// <summary>
    /// Gets a summary of the audit log for display purposes.
    /// </summary>
    /// <returns>Audit log summary object.</returns>
    public object GetAuditSummary()
    {
        return new
        {
            Id,
            Action,
            Category,
            Severity,
            Description,
            EntityType,
            EntityId,
            Result,
            CreatedAt,
            UserId,
            ChildId,
            IpAddress = IpAddress?.Substring(0, Math.Min(IpAddress.Length, 10)) + "...", // Truncated for privacy
            Platform,
            IsReviewed,
            RequiresAttention,
            RetentionClass,
            CanBeDeleted = CanBeDeleted(),
            IsSubjectToCoppa = IsSubjectToCoppa()
        };
    }

    /// <summary>
    /// Sanitizes sensitive information from the audit log for export or sharing.
    /// </summary>
    /// <returns>Sanitized audit log data.</returns>
    public object GetSanitizedData()
    {
        return new
        {
            Id,
            Action,
            Category,
            Severity,
            Description,
            EntityType,
            Result,
            CreatedAt,
            Platform,
            // Exclude sensitive fields like IP address, user agents, etc.
            HasUserData = UserId.HasValue,
            HasChildData = ChildId.HasValue,
            IsReviewed,
            RequiresAttention
        };
    }
}