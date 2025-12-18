// Models/OrderViewModel.cs
using System.ComponentModel.DataAnnotations;

public class OrderViewModel
{
    public int OrderId { get; set; }

    [Required(ErrorMessage = "Поле 'Дата замовлення' є обов'язковим")]
    public DateTime OrderDate { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Сума має бути більшою за 0")]
    public decimal TotalAmount { get; set; }

    [Required(ErrorMessage = "Виберіть статус")]
    public int StatusId { get; set; }

    public string? StatusName { get; set; } // Для відображення в списку
}