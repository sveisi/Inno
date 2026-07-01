using AutoMapper;
using Inno.Models;
using Inno.ViewModels;

namespace Inno.MappingProfiles
{
    public class InventoryProfile : Profile
    {
        public InventoryProfile()
        {
            CreateMap<ProductKardex, ProductKardexView>();
        }
    }
}