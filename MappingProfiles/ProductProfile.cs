using AutoMapper;
using Inno.Models;
using Inno.ViewModels;

namespace Inno.Map
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductView>().ReverseMap();

            CreateMap<Product, ProductListView>()
               .ForMember(d => d.CategoryName, s => s.MapFrom(s => s.Category.Name))
               .ForMember(d => d.CategoryEnName, s => s.MapFrom(s => s.Category.EnName))
               .ForMember(d => d.UnitName, s => s.MapFrom(s => s.Unit.Name))
               .ForMember(d => d.UnitEnName, s => s.MapFrom(s => s.Unit.EnName))
               .ForMember(d => d.ColorName, s => s.MapFrom(s => s.Color.Name))
               .ForMember(d => d.ColorEnName, s => s.MapFrom(s => s.Color.EnName))
               .ReverseMap();
        }
    }
}