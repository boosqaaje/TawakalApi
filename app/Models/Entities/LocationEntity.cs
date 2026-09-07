using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TawakalApi.app.Models.Entities;

[Table("locations")]
public class LocationEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(45, ErrorMessage = "Location code cannot be longer than 45 characters.")]
    public string LocationCode { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Name { get; set; }

    public bool IsActive { get; set; } = true;
}