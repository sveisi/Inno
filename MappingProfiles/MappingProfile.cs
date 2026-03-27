using AutoMapper;
using Inno.Models;
using Inno.ViewModels;

namespace Inno.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Attachment, AttachmentCreateView>().ReverseMap();
            CreateMap<Attachment, AttachmentView>().ReverseMap();

            CreateMap<Color, ColorView>().ReverseMap();

            CreateMap<Category, CategoryView>().ReverseMap();

            CreateMap<Region, RegionView>().ReverseMap();

            CreateMap<Storage, StorageView>().ReverseMap();
            CreateMap<StorageView, Storage>().ReverseMap();

            CreateMap<Location, LocationView>().ReverseMap();

            CreateMap<SKU, SKUView>()
                .ForMember(d => d.ProductName, s => s.Ignore())
                .ReverseMap();
            CreateMap<SKU, SKUListView>().ReverseMap();

            CreateMap<User, UserView>();

            CreateMap<User, UserEditView>()
                .ForMember(d => d.OrigUserName, s => s.MapFrom(s => s.UserName));
        }
    }
}