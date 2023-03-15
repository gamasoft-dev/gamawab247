using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using AutoMapper;
using Domain.Entities.RequestAndComplaints;

namespace Application.Mapper
{
    public class RequestAndCompliantMapper :Profile
    {
        public RequestAndCompliantMapper()
        {
            CreateMap<RequestAndComplaint, RequestAndComplaintDto>().ReverseMap();
            CreateMap<RequestAndComplaint, CreateRequestAndComplaintDto>().ReverseMap();
            CreateMap<RequestAndComplaint, UpdateRequestAndComplaintDto>().ReverseMap();
        }
    }
}
