using System;
using Application.DTOs.CreateDialogDtos;
using AutoMapper;
using Domain.Entities.DialogMessageEntitties;
using Domain.Entities.DialogMessageEntitties.ValueObjects;
using System.Linq;

namespace Application.Mapper.Interactive_Message
{
    public  class ListMessageMapper: Profile
    {
        public ListMessageMapper()
        {
            CreateMap<CreateBusinessMessageDto<CreateListMessageDto>,
                ListMessage>().ReverseMap();
                
            CreateMap<BaseCreateMessageDto, ListMessage>();
            CreateMap<ListMessage, BaseInteractiveDto>().ReverseMap();

            CreateMap<CreateListMessageDto, ListMessage>()
                .ForMember(dest => dest.BusinessMessageId,
                    opt => opt.MapFrom(src => src.BusinessMessageId));
            
            CreateMap<UpdateListMessageDto, ListMessage>()
                .ForMember(dest => dest.BusinessMessageId,
                    opt => opt.MapFrom(src => src.BusinessMessageId)).ReverseMap();
            
            CreateMap<ListMessageDto, ListMessage>().ReverseMap();

            CreateMap<ListActionDto, ListAction>().ReverseMap();
            
            CreateMap<CreateListMessageDto, BaseCreateMessageDto>().ReverseMap();
            CreateMap<ListMessageDto, BaseInteractiveDto>().ReverseMap();

            CreateMap<CreateBusinessMessageDto<CreateListMessageDto>,
                CreateBusinessMessageDto<BaseCreateMessageDto>>().ReverseMap();

            CreateMap<SectionDto, Section>();
            CreateMap<Section, SectionDto>();
            CreateMap<RowDto, Row>().AfterMap((src, dest) =>
            {
                dest.Id = Guid.NewGuid().ToString();
                dest.NextMessagePosition = src.NextBusinessMessagePosition;
            });
            CreateMap<Row, RowDto>().AfterMap((src, dest) =>
            {
                dest.NextBusinessMessagePosition = src.NextMessagePosition;
            });
            
        }
    }
}
