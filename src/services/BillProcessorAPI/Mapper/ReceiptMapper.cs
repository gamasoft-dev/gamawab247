using AutoMapper;
using BillProcessorAPI.Dtos.Common;
using BillProcessorAPI.Entities;

namespace BillProcessorAPI.Mapper
{
    public class ReceiptMapper:Profile
    {
        public ReceiptMapper()
        {
            CreateMap<Receipt, ReceiptDto>().ReverseMap();
            CreateMap<Receipt, ReceiptDetailsDto>();
            CreateMap<BillTransaction, Receipt>()
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Empty));
        }
    }
}
