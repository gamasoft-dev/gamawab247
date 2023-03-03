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
    public class PartnerMapper : Profile
    {
        public PartnerMapper()
        {
            CreateMap<Partner, PartnerDto>().ReverseMap();
            CreateMap<Partner, CreatePartnerDto>().ReverseMap();
            CreateMap<Partner, UpdatePartnerDto>().ReverseMap();
        }
    }
}
