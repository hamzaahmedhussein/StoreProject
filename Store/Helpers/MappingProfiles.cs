using API.DTOs;
using AutoMapper;
using Core.Entities.Identity;
using Core.Models;

namespace API.Helpers
{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d => d.ProductType, o => o.MapFrom(d => d.ProductType.Name))
                .ForMember(d => d.ProductBrand, o => o.MapFrom(d => d.ProductBrand.Name))
                .ForMember(d => d.PictureUrl, o => o.MapFrom <ProductUrlResolver>());

            CreateMap<AddressDto, Address>();


        }
    }
}
