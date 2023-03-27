using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.RequestAndComplaintDtos;
using AutoMapper;
using Domain.Entities.RequestAndComplaints;

namespace Application.Mapper
{
    public class RequestAndCompliantMapper :Profile
    {
        public RequestAndCompliantMapper()
        {
            CreateMap<RequestAndComplaint, RequestAndComplaintDto>()
                 .ForMember(dest => dest.TreatedBy, opt => opt.MapFrom(src => src.TreatedBy.FirstName));
                 //.ForMember(dest => dest.Responses, opt => opt.MapFrom(src => src.ResponsList.Responses.Select(x => x.Response)));


            CreateMap<CreateRequestAndComplaintDto, RequestAndComplaint>();
            CreateMap<UpdateRequestAndComplaintDto, RequestAndComplaint>();
        }
    }
}
