using AutoMapper;
using BillProcessorAPI.Dtos.Common;
using BillProcessorAPI.Entities;
using BillProcessorAPI.Enums;

namespace BillProcessorAPI.Mapper
{
    public class InvoiceMapper : Profile
    {
        public InvoiceMapper()
        {
            CreateMap<Invoice, InvoiceDetailsDto>().ReverseMap();
            CreateMap<Invoice, InvoiceDto>().ReverseMap();
        }
    
    }
}
