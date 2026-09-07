using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TawakalApi.app.Models.Entities;

[Table("T_a_partners")]
[Index(nameof(PartnerEmail), IsUnique =true)]
[Index(nameof(PartnerUserName), IsUnique =true)]
public class PartnerEntity
{
    public long Id { get; set; }
    public string ClientId { get; set; } = string.Empty;

    // [Column("PartnerUsername")]
    public string PartnerEmail { get; set; } = string.Empty;
    public string PartnerUserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PartnerName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // --- Current Active Secret ---
    public string CurrentSecretHash { get; set; } = string.Empty;
    public DateTime CurrentSecretCreatedAt { get; set; } = DateTime.UtcNow;

    // --- Next / Pending Secret (Used during rotation overlap) ---
    public string? NextSecretHash { get; set; }
    public DateTime? NextSecretCreatedAt { get; set; }

    // -- Audit Field: Created By Portal User ---
    public long CreatedBy { get; set; }

    [Column("MustChangePassword")]
    [Required]
    public bool MustChangePassword { get; set; } = true; // Default to 1 on creation



    [Required]
    [StringLength(45, ErrorMessage = "Location code cannot be longer than 45 characters.")]
    public string LocationCode { get; set; } = string.Empty;

}