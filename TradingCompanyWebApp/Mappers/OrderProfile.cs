// Mappers/OrderProfile.cs
using AutoMapper;
using TradingCompanyWebApp.Models;
using TradingCompanyDto;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderViewModel>().ReverseMap();
    }
}