using AutoMapper;
using TradingCompanyDto;
using TradingCompanyWeb.Models;

namespace TradingCompanyWeb.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, OrderViewModel>().ReverseMap();
        }
    }
}