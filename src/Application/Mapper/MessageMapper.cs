using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.InboundMessageDto;
using Application.DTOs.CreateDialogDtos;
using Domain.Entities.DialogMessageEntitties;

namespace Application.Mapper
{
    public class MessageMapper : Profile
    {
        public MessageMapper()
        {
            CreateMap<InboundMessage, TextNotificationDto>().ReverseMap();
            CreateMap<OutboundMessage, CreateMessageSessionDto>().ReverseMap();
            CreateMap<CreateBusinessMessageDto<BaseCreateMessageDto>, BusinessMessage>().ReverseMap();
        }
    }
}
