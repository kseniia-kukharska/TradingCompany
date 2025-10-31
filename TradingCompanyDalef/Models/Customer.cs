using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradingCompanyDalef.Models;

public partial class Customer
{
    [Key]
    [Column("CustomerId")]
    public int CustomerId { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [StringLength(25)]
    public string Phone { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    [InverseProperty("Customer")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
