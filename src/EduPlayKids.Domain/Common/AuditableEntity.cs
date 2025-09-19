namespace EduPlayKids.Domain.Common;

/// <summary>
/// Auditable entity class that extends BaseEntity with audit information.
/// Used for entities that require detailed audit trails.
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    /// <summary>
    /// Gets or sets the identifier of the user who created the entity.
    /// Can be null for system-created entities.
    /// </summary>
    public int? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who last updated the entity.
    /// Can be null for system-updated entities.
    /// </summary>
    public int? UpdatedBy { get; set; }

    /// <summary>
    /// Gets or sets additional metadata about the entity changes.
    /// Stored as JSON for flexibility.
    /// </summary>
    public string? Metadata { get; set; }
}