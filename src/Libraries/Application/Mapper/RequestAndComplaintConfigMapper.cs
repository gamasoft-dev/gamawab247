using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Util;
using Application.DTOs.RequestAndComplaintDtos;
using AutoMapper;
using Domain.Entities.RequestAndComplaints;

namespace Application.Mapper
{
    public class RequestAndComplaintConfigMapper: Profile
    {
        public RequestAndComplaintConfigMapper()
        {
            CreateMap<RequestAndComplaintConfig, RequestAndComplaintConfigDto>().ReverseMap();
            CreateMap<RequestAndComplaintConfig, CreateRequestAndComplaintConfigDto>().ReverseMap();
            CreateMap<RequestAndComplaintConfig, UpdateRequestAndComplaintConfigDto>().ReverseMap();
        }
    }
}
