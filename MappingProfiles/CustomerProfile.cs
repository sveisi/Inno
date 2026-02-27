using AutoMapper;
using Inno.Models;
using Inno.ViewModels;

namespace Inno.Map
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerView>().ReverseMap();

            CreateMap<Customer, CustomerListView>()
               .ForMember(d => d.CityName, s => s.MapFrom(s => s.City.Name))
               .ForMember(d => d.CityEnName, s => s.MapFrom(s => s.City.EnName))
               .ReverseMap();
        }
    }
}