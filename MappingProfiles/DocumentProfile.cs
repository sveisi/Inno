using AutoMapper;
using Inno.Models;
using Inno.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Inno.MappingProfiles
{
    public class DocumentProfile : Profile
    {
        public DocumentProfile()
        {
            CreateMap<Document, DocumentListView>()
                .ForMember(d => d.DocumentType, s => s.MapFrom(s => s.DocumentType.Name))
                .ForMember(d => d.StorageName, s => s.MapFrom(s => s.Storage.Name))
                .ForMember(d => d.ObverseName, s => s.MapFrom(s => s.Obverse.Name))
                .ForMember(d => d.CreatedByName, s => s.MapFrom(s => s.CreatedByUser.FullName))
                .ReverseMap();

            CreateMap<Document, PurchaseReceiptCreateView>()
               .ForMember(d => d.DocumentId, s => s.MapFrom(s => s.Id))
               .ForMember(d => d.Items, s => s.MapFrom(s => s.DocumentItems))
               .ReverseMap();

            CreateMap<DocumentItem, DocumentItemView>()
                    .ForMember(d => d.ProductId, s => s.MapFrom(s => s.SKU.Product.Id))
                    .ForMember(d => d.ProductName, s => s.MapFrom(s => s.SKU.Product.Name))
                    .ReverseMap();
        }
    }
}