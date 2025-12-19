using System.ComponentModel.DataAnnotations;

namespace TradingCompanyWeb.Models
{
    public class OrderViewModel : IValidatableObject
    {
        public int OrderId { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (OrderDate > DateTime.Now)
            {
                yield return new ValidationResult("Order date cannot be in the future.", new[] { nameof(OrderDate) });
            }
        }


        [Required(ErrorMessage = "Customer ID is required.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Please select an order status.")]
        public int StatusId { get; set; }

        [Required(ErrorMessage = "Total amount is mandatory.")]
        [Range(0.01, 1000000.00, ErrorMessage = "Amount must be between $0.01 and $1,000,000.")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Order date is required.")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }
  
        public bool IsValidDate() => OrderDate <= DateTime.Now;
    }
}