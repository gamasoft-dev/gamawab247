using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapper
{
    public class BroadcastMessageMapper : Profile
    {
        public BroadcastMessageMapper()
        {
            CreateMap<BroadcastMessage, BroadcastMessageDto>();
            CreateMap<CreateBroadcastMessageDto, BroadcastMessage>().ReverseMap();
        }
    }
}
