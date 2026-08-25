using System.ComponentModel.DataAnnotations.Schema;

namespace TawakalApi.app.Models.Entities;

[Table("T_a_partners")]
public class PartnerEntity
{
    public int Id { get; set; }
    public string ClientId { get; set; } = string.Empty;

    // [Column("PartnerUsername")]
    public string PartnerUserName { get; set; } = string.Empty;
    public string PartnerName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // --- Current Active Secret ---
    public string CurrentSecretHash { get; set; } = string.Empty;
    public DateTime CurrentSecretCreatedAt { get; set; } = DateTime.UtcNow;

    // --- Next / Pending Secret (Used during rotation overlap) ---
    public string? NextSecretHash { get; set; }
    public DateTime? NextSecretCreatedAt { get; set; }

    // -- Audit Field: Created By Portal User ---
    public long CreatedBy {get; set;}

    [ForeignKey("CreatedBy")]
    public PortalUserEntity? CreatedByUser {get; set;}
}