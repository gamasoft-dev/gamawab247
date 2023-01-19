using System;
using System.Linq;
using Application.DTOs.CreateDialogDtos;
using Application.DTOs.InteractiveMesageDto;
using Application.DTOs.InteractiveMesageDto.CreateMessageRequestDto;
using Application.DTOs.OutboundMessageRequests;
using AutoMapper;
using Domain.Entities.DialogMessageEntitties;
using Domain.Entities.DialogMessageEntitties.ValueObjects;

namespace Application.Mapper.Interactive_Message
{
    public class ReplyMessageMapper: Profile
    {
        public ReplyMessageMapper()
        {
            CreateMap<CreateBusinessMessageDto<CreateButtonMessageDto>,
                ReplyButtonMessage>().AfterMap((src,
                dest) =>
            {
                dest.Body = src?.MessageTypeObject?.Body;
                dest.Footer = src?.MessageTypeObject?.Footer;
                dest.Header = src?.MessageTypeObject?.Header;
            });

                
            CreateMap<ButtonMessageDto, ReplyButtonMessage>().ReverseMap();
            
            CreateMap<CreateButtonMessageDto, ReplyButtonMessage>();
            CreateMap<ButtonActionDto, ButtonAction>().ReverseMap();
           
            CreateMap<ButtonDto, Button>().AfterMap((src, dest) =>
            {
                dest.Title = src?.reply?.title;
                dest.Description = src?.reply?.ToString();
                dest.NextMessagePosition = src?.NextBusinessMessagePosition ?? 0;
            });

            CreateMap<Button, ButtonDto>().AfterMap((src, dest) =>
            {
                dest.type = src.Type;
                dest.reply = new ReplyDto
                {
                    id = src.Id.ToString(),
                    title = src.Title
                };
                dest.NextBusinessMessagePosition = src.NextMessagePosition;
            });

            CreateMap<CreateBusinessMessageDto<CreateButtonMessageDto>,
                CreateBusinessMessageDto<BaseCreateMessageDto>>().ReverseMap();

            CreateMap<CreateButtonMessageDto, BaseCreateMessageDto>().ReverseMap();
            CreateMap<ButtonMessageDto, BaseInteractiveDto>().ReverseMap();

            CreateMap<ReplyDto, Button>().ReverseMap().AfterMap((src, dest) =>
            {
                dest.id = src.Id.ToString();
            });

            CreateMap<ReplyButtonMessage, ButtonMessageDto>().ReverseMap();
        }
    }
}