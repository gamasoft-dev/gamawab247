using Application.DTOs.BusinessDtos;
using AutoMapper;
using Domain.Entities.DialogMessageEntitties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapper
{
	public class BusinessConversationMapper :Profile
	{
		public BusinessConversationMapper()
		{
			CreateMap<BusinessConversation, BusinessConversationDto>().ReverseMap();
			CreateMap<BusinessConversation, CreateBusinessConversationDtoo>().ReverseMap();
			CreateMap<BusinessConversation, UpdateBusinessConversationDto>().ReverseMap();
		}
	}
}
