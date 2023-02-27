using Application.DTOs;
using Application.DTOs.BusinessDtos;
using Application.DTOs.InboundMessageDto;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Identities;

namespace Application.Mapper
{
    public class BusinessMapper : Profile
    {
        public BusinessMapper()
        {
            CreateMap<Business, CreateBusinessDto>().ReverseMap();
            CreateMap<Business, BusinessDto>()
                .ForMember(dest => dest.IndustryId, opt => opt.MapFrom(src => src.Industry.Id))
                .ForMember(dest => dest.BusinessAdminEmail, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.IndustryName, opt => opt.MapFrom(src => src.Industry.Name))
                .ForMember(dest => dest.AdminId, opt => opt.MapFrom(src => src.AdminUser.Id))
                .ForMember(dest => dest.AdminEmail, opt => opt.MapFrom(src => src.AdminUser.Email))
                .ForMember(dest => dest.AdminFirstName, opt => opt.MapFrom(src => src.AdminUser.FirstName))
                .ForMember(dest => dest.AdminSurName, opt => opt.MapFrom(src => src.AdminUser.LastName))
                .ForMember(dest => dest.AdminPhoneNumber, opt => opt.MapFrom(src => src.AdminUser.PhoneNumber));
        

            CreateMap<BusinessDto, Business>().AfterMap((src, dest) =>
            {
                dest.Email = src.BusinessAdminEmail;
            });
            CreateMap<CreateBusinessDto, User>().AfterMap((src, dest) =>
            {
                dest.FirstName = src.AdminFirstName;
                dest.LastName = src.AdminLastName;
                dest.PhoneNumber = src.AdminPhoneNumber;
                dest.Email = src.BusinessAdminEmail;
            });
            
            CreateMap<BusinessMessageSettings, CreateBusinessSetupDto>().ReverseMap();
            CreateMap<dynamic, TextNotificationDto>().ReverseMap();

        }
    }
}
