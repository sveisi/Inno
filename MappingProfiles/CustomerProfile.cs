using AutoMapper;
using Inno.Models;
using Inno.ViewModels;

namespace Inno.MappingProfiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerView>()
                .ReverseMap()
                .ForMember(d => d.CreditBalance, s => s.Ignore());

            CreateMap<Customer, CustomerListView>()
               .ForMember(d => d.CityName, s => s.MapFrom(s => s.City.Name))
               .ForMember(d => d.CityEnName, s => s.MapFrom(s => s.City.EnName))
               .ReverseMap();
        }
    }
}