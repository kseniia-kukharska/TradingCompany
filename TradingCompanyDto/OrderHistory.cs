using System;

namespace TradingCompanyDto
{
    public class OrderHistory
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
        public DateTime ChangeDate { get; set; }
    }
}