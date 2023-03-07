using Application.DTOs.BusinessDtos;
using AutoMapper;
using Domain.Entities.FormProcessing;
using Domain.Entities.FormProcessing.ValueObjects;

namespace Application.Mapper
{
    public class BusinessFormMapper:Profile
    {
        public BusinessFormMapper()
        {
            CreateMap<BusinessForm, BusinessFormDto>()
                .ForMember(dest => dest.BusinessConversationId, mapTo => mapTo.MapFrom(src => src.BusinessConversationId))
                .ForMember(dest => dest.FormElements, mapTo => mapTo.MapFrom(src => src.FormElements))
                .ForMember(dest => dest.UrlMethodType, mapTo => mapTo.MapFrom(src => src.UrlMethodType))
                .ForMember(dest => dest.SubmissionUrl, mapTo => mapTo.MapFrom(src => src.SubmissionUrl))
                .ForMember(dest => dest.AuthHeaderElement, mapTo => mapTo.MapFrom(src => src.Headers))
                .ForMember(dest => dest.BusinessId, mapTo => mapTo.MapFrom(src => src.BusinessId));

            CreateMap<BusinessFormDto, BusinessForm>()
               .ForMember(dest => dest.BusinessConversationId, mapTo => mapTo.MapFrom(src => src.BusinessConversationId))
               .ForMember(dest => dest.FormElements, mapTo => mapTo.MapFrom(src => src.FormElements))
               .ForMember(dest => dest.UrlMethodType, mapTo => mapTo.MapFrom(src => src.UrlMethodType))
               .ForMember(dest => dest.SubmissionUrl, mapTo => mapTo.MapFrom(src => src.SubmissionUrl))
               .ForMember(dest => dest.Headers, mapTo => mapTo.MapFrom(src => src.AuthHeaderElement))
               .ForMember(dest => dest.BusinessId, mapTo => mapTo.MapFrom(src => src.BusinessId));

            CreateMap<BusinessForm, CreateBusinessFormDto>().ReverseMap();
            CreateMap<BusinessForm, UpdateBusinessFormDto>().ReverseMap();
            CreateMap<KeyValueObj, BusinessFormHeaderDto>().ReverseMap();
            CreateMap<FormResponseKvp, BusinessFormHeaderDto>().ReverseMap();
            //CreateMap<FormResponseKvp, BusinessFormHeaderDto>().ReverseMap();
            //CreateMap<BusinessForm, BusinessFormDto>().ReverseMap();
        }
    }
}
