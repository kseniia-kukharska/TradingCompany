using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradingCompanyDalef.Models;

public partial class OrderDetail
{
    [Key]
    [Column("OrderDetailId")]
    public int OrderDetailId { get; set; }

    [Column("OrderId")]
    public int OrderId { get; set; }

    [Column("ProductId")]
    public int ProductId { get; set; }

    public int Quantity { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("OrderDetails")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("OrderDetailId")]
    [InverseProperty("OrderDetail")]
    public virtual Product OrderDetailNavigation { get; set; } = null!;
}
