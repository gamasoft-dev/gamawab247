using Application.DTOs.CreateDialogDtos;
using AutoMapper;

namespace Application.Mapper;

public class GenericMapper: Profile
{
    public GenericMapper()
    {
        CreateMap(typeof(CreateBusinessMessageDto<>), typeof(BusinessMessageDto<>)).ReverseMap();

    }
}