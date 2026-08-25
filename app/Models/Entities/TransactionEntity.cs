using System.ComponentModel.DataAnnotations.Schema;

namespace TawakalApi.app.Models.Entities;


[Table("Transactions")]
public class TransactionEntity
{
    public long Id {get; set;}
    public string PartnerUsername {get; set;} = string.Empty;
    public string ReferenceId {get; set;} = string.Empty;
    
    [Column("Amount", TypeName = "decimal(18,2)")]
    public decimal Amount {get; set;}
    public string Currency {get; set;} = string.Empty;
    public string ServiceCode {get; set;} = string.Empty;
    public string Status {get; set;} = string.Empty;
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
}