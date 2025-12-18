using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TradingCompanyDto
{
    public class Order : IDataErrorInfo, INotifyPropertyChanged
    {
        private int _orderId;
        private int _customerId;
        private int _statusId;
        private decimal _totalAmount;
        private DateTime _orderDate;

        public int OrderId { get => _orderId; set { _orderId = value; OnPropertyChanged(); } }
        public int CustomerId { get => _customerId; set { _customerId = value; OnPropertyChanged(); } }
        public int StatusId { get => _statusId; set { _statusId = value; OnPropertyChanged(); } }

        public decimal TotalAmount
        {
            get => _totalAmount;
            set { _totalAmount = value; OnPropertyChanged(); }
        }

        public DateTime OrderDate
        {
            get => _orderDate;
            set { _orderDate = value; OnPropertyChanged(); }
        }

        // IDataErrorInfo реалізація
        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                string error = null;
                switch (columnName)
                {
                    case nameof(OrderDate):
                        if (OrderDate > DateTime.Now)
                            error = "Order date cannot be in the future.";
                        break;

                    case nameof(TotalAmount):
                        if (TotalAmount < 0)
                            error = "Total amount cannot be negative.";
                        break;
                }
                return error;
            }
        }

        // INotifyPropertyChanged реалізація
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}