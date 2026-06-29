using AutoMapper;
using Inno.Models;
using Inno.ViewModels;
using System.Linq;

namespace Inno.MappingProfiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductView>()
                .ReverseMap()
                .ForMember(d => d.Images, s => s.Ignore());

            CreateMap<Product, ProductListView>()
               .ForMember(d => d.CategoryName, s => s.MapFrom(s => s.Category.Name))
               .ForMember(d => d.CategoryEnName, s => s.MapFrom(s => s.Category.EnName))
               .ForMember(d => d.UnitName, s => s.MapFrom(s => s.Unit.Name))
               .ForMember(d => d.UnitEnName, s => s.MapFrom(s => s.Unit.EnName))
               .ForMember(d => d.ColorName, s => s.MapFrom(s => s.Color.Name))
               .ForMember(d => d.ColorEnName, s => s.MapFrom(s => s.Color.EnName))
               .ForMember(d => d.ImageUrl, s => s.MapFrom(x => x.Images
                   .Where(i => !i.Attachment.IsTemporary)
                   .OrderBy(i => i.SortOrder)
                   .Select(i => i.Attachment.FileUrl)
                   .FirstOrDefault()))
               .ForMember(d => d.ThumbImageUrl, s => s.MapFrom(x => x.Images
                   .Where(i => !i.Attachment.IsTemporary)
                   .OrderBy(i => i.SortOrder)
                   .Select(i => i.Attachment.ThumbImageUrl)
                   .FirstOrDefault()))
               .ReverseMap()
               .ForMember(d => d.Images, s => s.Ignore());

            CreateMap<ProductImage, AttachmentItemView>()
                .ForMember(d => d.Id, s => s.MapFrom(x => x.AttachmentId))
                .ForMember(d => d.FileName, s => s.MapFrom(x => x.Attachment.FileName))
                .ForMember(d => d.FileUrl, s => s.MapFrom(x => x.Attachment.FileUrl))
                .ForMember(d => d.ThumbImageUrl, s => s.MapFrom(x => x.Attachment.ThumbImageUrl));
        }
    }
}