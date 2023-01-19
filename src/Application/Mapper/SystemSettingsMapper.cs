using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapper
{
    public class SystemSettingsMapper :Profile
    {
        public SystemSettingsMapper()
        {
            CreateMap<SystemSettings, SystemSettingsDto>().ReverseMap();

            CreateMap<CreateSystemSetttingsDto, SystemSettings>();
        }
    }
}
