using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.PartnerContentDtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapper
{
    public class PartnerIntegrationMappers :Profile
    {
        public PartnerIntegrationMappers()
        {
            CreateMap<PartnerIntegrationDetails, PartnerContentIntegrationDto>().ReverseMap();
            CreateMap<PartnerIntegrationDetails, CreatePartnerContentIntegrationDto>().ReverseMap();
            CreateMap<PartnerIntegrationDetails, UpdatePartnerContentIntegrationDto>().ReverseMap();
        }
    }
}
