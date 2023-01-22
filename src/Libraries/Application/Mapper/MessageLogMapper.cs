using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapper
{
    public class MessageLogMapper : Profile
    {
        public MessageLogMapper()
        {
            CreateMap<MessageLog, MessageLogDto>().ReverseMap();
            CreateMap<CreateMessageLogDto, MessageLog>().ReverseMap();
            CreateMap<UpdateMessageLogDto, MessageLog>().ReverseMap();
        } 
    }
}
