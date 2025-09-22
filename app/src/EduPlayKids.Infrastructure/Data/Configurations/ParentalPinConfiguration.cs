using EduPlayKids.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduPlayKids.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the ParentalPin entity.
/// Configures database schema, indexes, and relationships for PIN authentication data.
/// </summary>
public class ParentalPinConfiguration : IEntityTypeConfiguration<ParentalPin>
{
    /// <summary>
    /// Configures the ParentalPin entity mapping.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<ParentalPin> builder)
    {
        // Table configuration
        builder.ToTable("ParentalPins");

        // Primary key
        builder.HasKey(p => p.Id);

        // Configure properties
        builder.Property(p => p.UserId)
            .IsRequired()
            .HasComment("Foreign key to the parent user");

        builder.Property(p => p.PinHash)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("Hashed PIN value for secure storage");

        builder.Property(p => p.Salt)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Salt used for PIN hashing");

        builder.Property(p => p.SecurityQuestion)
            .IsRequired()
            .HasMaxLength(500)
            .HasComment("Security question for PIN recovery");

        builder.Property(p => p.SecurityAnswerHash)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("Hashed security answer");

        builder.Property(p => p.SecurityAnswerSalt)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Salt used for security answer hashing");

        builder.Property(p => p.LastChangedAt)
            .IsRequired()
            .HasComment("When the PIN was last changed");

        builder.Property(p => p.LastSuccessfulVerificationAt)
            .HasComment("Last successful PIN verification timestamp");

        builder.Property(p => p.FailedAttempts)
            .IsRequired()
            .HasDefaultValue(0)
            .HasComment("Current consecutive failed attempts");

        builder.Property(p => p.LockedUntil)
            .HasComment("PIN entry locked until this timestamp");

        builder.Property(p => p.LastFailedAttemptAt)
            .HasComment("Timestamp of last failed attempt");

        builder.Property(p => p.TotalFailedAttempts)
            .IsRequired()
            .HasDefaultValue(0)
            .HasComment("Total failed attempts since creation");

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("Whether PIN is currently active");

        builder.Property(p => p.ComplexityLevel)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("Basic")
            .HasComment("PIN complexity level enforced");

        builder.Property(p => p.SecurityMetadata)
            .HasColumnType("TEXT")
            .HasComment("Additional security settings as JSON");

        // Configure relationships
        builder.HasOne(p => p.User)
            .WithMany() // A user can have multiple PINs (for PIN history if needed)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure indexes for performance and security
        builder.HasIndex(p => p.UserId)
            .HasDatabaseName("IX_ParentalPins_UserId");

        builder.HasIndex(p => new { p.UserId, p.IsActive })
            .HasDatabaseName("IX_ParentalPins_UserId_IsActive")
            .HasFilter("IsActive = 1");

        builder.HasIndex(p => p.LastFailedAttemptAt)
            .HasDatabaseName("IX_ParentalPins_LastFailedAttemptAt");

        builder.HasIndex(p => p.LockedUntil)
            .HasDatabaseName("IX_ParentalPins_LockedUntil")
            .HasFilter("LockedUntil IS NOT NULL");

        // Configure audit timestamps (inherited from AuditableEntity)
        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasComment("PIN creation timestamp");

        builder.Property(p => p.UpdatedAt)
            .IsRequired()
            .HasComment("Last update timestamp");

        builder.Property(p => p.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Soft delete flag");

        // Add database-level constraints for additional security
        builder.HasCheckConstraint("CK_ParentalPins_FailedAttempts", "FailedAttempts >= 0");
        builder.HasCheckConstraint("CK_ParentalPins_TotalFailedAttempts", "TotalFailedAttempts >= 0");
        builder.HasCheckConstraint("CK_ParentalPins_ComplexityLevel", "ComplexityLevel IN ('Basic', 'Enhanced', 'Maximum')");
    }
}