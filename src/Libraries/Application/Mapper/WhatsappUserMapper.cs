using System;
using Application.DTOs;
using Application.DTOs.InboundMessageDto;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapper
{
    public class WhatsappUserMapper : Profile
    {
        public WhatsappUserMapper()
        {
            CreateMap<WhatsappUser, UpsertWhatsappUserDto>().ReverseMap();
            CreateMap<WhatsappUser, WhatsappUserDto>().ReverseMap();
            CreateMap<Contact, UpsertWhatsappUserDto>().AfterMap((src, dest) =>
            {

                dest.Name = src?.profile?.name;
                dest.PhoneNumber = src.wa_id;
                dest.WaId = src.wa_id;
                dest.LastMessageTime = DateTime.UtcNow;
            });
        }
    }
}
