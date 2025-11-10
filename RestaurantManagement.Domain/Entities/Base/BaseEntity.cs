namespace RestaurantManagement.Domain.Entities.Base;

/// <summary>
/// Base entity class for all entities
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Primary key
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Audit entity for tracking changes
/// </summary>
public abstract class AuditEntity : BaseEntity
{
    /// <summary>
    /// User who created the record
    /// </summary>
    public int CreatedByUserId { get; set; }

    /// <summary>
    /// User who last modified the record
    /// </summary>
    public int? ModifiedByUserId { get; set; }
}
