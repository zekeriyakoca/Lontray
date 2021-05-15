using AutoMapper;
using Catalog.API.Dtos;
using Catalog.API.Entities;
using Catalog.API.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Infrastructure.Configurations.AutoMapper
{
    public class AutoMappingProfiles : Profile
    {
        public AutoMappingProfiles()
        {
            CreateMap<CatalogItem, CreateCatalogItemDto>()
                .ReverseMap();
            CreateMap<CatalogItem, CatalogItemCreatedEvent>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CatalogItemId, opt => opt.MapFrom(src => src.Id));


            CreateMap<UpdateCatalogItemDto, CatalogItem>();

            CreateMap<CatalogItem, CatalogItemUpdatedEvent>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CatalogItemId, opt => opt.MapFrom(src => src.Id));
        }
    }
}
