using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TawakalApi.app.Models.Entities;


[Table("T_a_portal_users")]
[Index(nameof(Email), IsUnique = true)]
public class PortalUserEntity
{
    [Key]
    public long Id { get; set; }

    [Column("FirstName")]
    [Required, MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Column("MiddleName")]
    [Required, MaxLength(50)]
    public string MiddleName { get; set; } = string.Empty;

    [Column("LastName")]
    [Required, MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Column("Email")]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Column("PasswordHash")]
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("Role")]
    [Required, MaxLength(50)]
    public string Role { get; set; } = "USER"; // "ADMIN" or "USER"

    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // --- Self-Referencing Foreign Key for Audit ---
    public long? CreatedByUserId { get; set; } // Nullable so the first user can have a null value
    [Column("MustChangePassword")]
    [Required]
    public bool MustChangePassword { get; set; } = true; // Default to 1 on creation
}