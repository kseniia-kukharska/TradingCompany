using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradingCompanyDalef.Models;

public partial class Order
{
    [Key]
    [Column("OrderId")]
    public int OrderId { get; set; }

    [Column("CustomerId")]
    public int CustomerId { get; set; }

    [Column("StatusId")]
    public int StatusId { get; set; }

    [Column(TypeName = "decimal(9, 2)")]
    public decimal TotalAmount { get; set; }

    public DateOnly OrderDate { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Orders")]
    public virtual Customer Customer { get; set; } = null!;

    [InverseProperty("Order")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [ForeignKey("StatusId")]
    [InverseProperty("Orders")]
    public virtual Status Status { get; set; } = null!;
}
