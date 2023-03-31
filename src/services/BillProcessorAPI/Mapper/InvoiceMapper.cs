using AutoMapper;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Dtos.Common;
using BillProcessorAPI.Entities;
using BillProcessorAPI.Enums;

namespace BillProcessorAPI.Mapper
{
    public class InvoiceMapper : Profile
    {
        public InvoiceMapper()
        {
            CreateMap<Invoice, InvoiceDetailsDto>();
            CreateMap<Invoice, InvoiceDto>();
            CreateMap<Invoice, PaymentConfirmationResponse>();
        }
    
    }
}
