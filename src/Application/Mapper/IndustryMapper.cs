using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapper
{
    public class IndustryMapper : Profile
    {
        public IndustryMapper()
        {
            CreateMap<CreateIndustryDto, Industry>().ReverseMap();
            CreateMap<IndustryDto, Industry>().ReverseMap();
        }
    }
}
