using Application.DTOs;
using AutoMapper;
using Connect.Application.DTOs;
using Core.Entities;
using Core.Models;
using System.Net.Mail;

namespace Application.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterUserDto, AppUser>()
               .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => new MailAddress(src.Email).User));
            CreateMap<AddProductDto, Product>();
            CreateMap<ProductUpdateDto, Product>();
            CreateMap<Product, ProductToReturnDto>();
            CreateMap<ProductUpdateDto, Product>()
          .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<AddressDto, Address>().ReverseMap();

            CreateMap<Customer, UserProfileInfo>()
                .ForMember(dest => dest.Street, opt => opt.MapFrom(s => s.Address.Street))
                .ForMember(dest => dest.City, opt => opt.MapFrom(s => s.Address.City))
                .ForMember(dest => dest.State, opt => opt.MapFrom(s => s.Address.State));

            CreateMap<Seller, UserProfileInfo>()
               .ForMember(dest => dest.Street, opt => opt.MapFrom(s => s.Address.Street))
               .ForMember(dest => dest.City, opt => opt.MapFrom(s => s.Address.City))
               .ForMember(dest => dest.State, opt => opt.MapFrom(s => s.Address.State));



        }



    }
}
