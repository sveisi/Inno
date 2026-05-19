using AutoMapper;
using Inno.Models;
using Inno.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Inno.MappingProfiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderListView>()
                .ForMember(d => d.CreatedByName, s => s.MapFrom(s => s.CreatedByUser.FullName))
                .ReverseMap();

            CreateMap<Order, OrderCreateView>()
               .ReverseMap();

            CreateMap<OrderItem, OrderItemView>()
                    .ForMember(d => d.ProductId, s => s.MapFrom(s => s.Product.Id))
                    .ForMember(d => d.ProductName, s => s.MapFrom(s => s.Product.Name))
                    .ReverseMap();
        }
    }
}