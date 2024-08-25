using Application.DTOs;
using AutoMapper;
using Domain.Entities.Identities;

namespace Application.Mapper
{
    public class RolesMapper : Profile
    {
        public RolesMapper()
        {
            CreateMap<Role,RoleDto>();
        }
    }
}
