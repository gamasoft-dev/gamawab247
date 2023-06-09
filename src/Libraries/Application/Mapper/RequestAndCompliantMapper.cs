using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.RequestAndComplaintDtos;
using AutoMapper;
using Domain.Entities.RequestAndComplaints;
using Org.BouncyCastle.Math.EC.Rfc7748;

namespace Application.Mapper
{
    public class RequestAndCompliantMapper :Profile
    {
        public RequestAndCompliantMapper()
        {
            CreateMap<RequestAndComplaint, RequestAndComplaintDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.LastTreatedBy, opt => opt.MapFrom(src => src.TreatedBy.FirstName))
                .ForMember(dest => dest.LastTreatedById, opt => opt.MapFrom(src => src.TreatedBy.Id))
                .AfterMap((src, dest) =>
                {
                    dest.Responses = src.ResponsList.Responses.ToList().OrderByDescending(x=>x.DateResponded).Take(3);
                });

                 //.ForMember(dest => dest.Responses, opt => opt.MapFrom(src => src.ResponsList.Responses.Select(x => x.Response)));


            CreateMap<CreateRequestAndComplaintDto, RequestAndComplaint>();
            CreateMap<UpdateRequestAndComplaintDto, RequestAndComplaint>();
        }
    }
}
