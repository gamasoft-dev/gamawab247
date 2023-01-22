using System.Linq;
using Application.DTOs.InboundMessageDto;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapper;

public class InboundMessageMapper: Profile
{
    public InboundMessageMapper()
    {
        // map received messages to inbound message entities
        CreateMap<ButtonReplyNotificationDto, InboundMessage>().AfterMap((src, dest) =>
        {
            //dest.Body = src?.Messages?.FirstOrDefault()?.ButtonReply?.button_reply?.title;
            //dest.From = src?.Messages?.FirstOrDefault()?.From;
            //dest.WhatsAppId = src?.Contacts?.FirstOrDefault()?.wa_id;
            //dest.Type = src?.Messages?.FirstOrDefault()?.ButtonReply?.type;
            //dest.Wa_Id = dest.From;
            //dest.ContextMessageId = src?.Messages?.FirstOrDefault()?.context.Id;
            //dest.MsgOptionId = src?.Messages?.FirstOrDefault()?.ButtonReply?.button_reply?.id;
            dest.Body = src?.Messages?.FirstOrDefault()?.interactive.button_reply.title;
            dest.From = src?.Messages?.FirstOrDefault()?.From;
            dest.WhatsAppId = src?.Contacts?.FirstOrDefault()?.wa_id;
            dest.Type = src?.Messages?.FirstOrDefault()?.interactive.type;
            dest.Wa_Id = dest.From;
            dest.ContextMessageId = src?.Messages?.FirstOrDefault()?.context.Id;
            dest.MsgOptionId = src?.Messages?.FirstOrDefault()?.interactive.button_reply.id;
        });
        
        CreateMap<ListReplyNotificationDto, InboundMessage>().AfterMap((src, dest) =>
        {
            dest.Body = src?.Messages?.FirstOrDefault()?.Interactive.list_reply.title;
            dest.From = src?.Messages?.FirstOrDefault()?.From;
            dest.WhatsAppId = src?.Contacts?.FirstOrDefault()?.wa_id;
            dest.Type = src?.Messages?.FirstOrDefault()?.Interactive.type;
            dest.Wa_Id = dest.From;
            dest.ContextMessageId = src?.Messages?.FirstOrDefault()?.context.Id;
            dest.MsgOptionId = src?.Messages?.FirstOrDefault()?.Interactive.list_reply.id;
        });
        
        CreateMap<TextNotificationDto, InboundMessage>().AfterMap((src, dest) =>
        {
            dest.Body = src?.Messages?.FirstOrDefault()?.text.body;
            dest.From = src?.Messages?.FirstOrDefault()?.From;
            dest.WhatsAppId = src?.Contacts?.FirstOrDefault()?.wa_id;
            dest.Type = src?.Messages?.FirstOrDefault()?.Type;
            dest.Wa_Id = dest.From;
            dest.ContextMessageId = src?.Messages?.FirstOrDefault()?.context.Id;
            dest.MsgOptionId = src?.Messages?.FirstOrDefault()?.context.Id;
        });
    }
}