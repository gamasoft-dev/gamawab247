using System;
using Application.DTOs.CreateDialogDtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapper;

public class OutboundMessageMapper: Profile
{
    public OutboundMessageMapper()
    {
        // map BusinessMessageDto<BaseInteractiveDto> to Inbound Message
        CreateMap<BusinessMessageDto<BaseInteractiveDto>, OutboundMessage>().AfterMap((src, dest) =>
        {
            dest.Body = src.MessageTypeObject.Body;
            dest.From = "GamaSoft";
            dest.Name = src.Name;
            dest.Type = src.MessageType;
            dest.BusinessMessageId = src.Id;
            dest.IsFirstMessageSent = src.Position == 1;
        });
        
        CreateMap<BusinessMessageDto<ButtonMessageDto>, OutboundMessage>().AfterMap((src, dest) =>
        {
            dest.Body = src.MessageTypeObject.Body;
            dest.From = "GamaSoft";
            dest.Name = src.Name;
            dest.Type = src.MessageType;
            dest.BusinessMessageId = src.Id;
            dest.IsFirstMessageSent = src.Position == 1;
        });
        
        CreateMap<BusinessMessageDto<ListMessageDto>, OutboundMessage>().AfterMap((src, dest) =>
        {
            dest.Body = src.MessageTypeObject.Body;
            dest.From = "GamaSoft";
            dest.Name = src.Name;
            dest.Type = src.MessageType;
            dest.BusinessMessageId = src.Id;
            dest.IsFirstMessageSent = src.Position == 1;
        });
        
        CreateMap<BusinessMessageDto<TextMessageDto>, OutboundMessage>().AfterMap((src, dest) =>
        {
            dest.Body = src.MessageTypeObject.Body;
            dest.From = "GamaSoft";
            dest.Name = src.Name;
            dest.Type = src.MessageType;
            dest.BusinessMessageId = src.Id;
            dest.IsFirstMessageSent = src.Position == 1;
        });
    }
}