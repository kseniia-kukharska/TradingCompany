using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradingCompanyDalef.Models;

public partial class Status
{
    [Key]
    [Column("StatusId")]
    public int StatusId { get; set; }

    [StringLength(50)]
    public string StatusName { get; set; } = null!;

    [InverseProperty("Status")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
