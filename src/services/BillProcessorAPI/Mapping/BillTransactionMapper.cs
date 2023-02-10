using AutoMapper;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Entities;

namespace BillProcessorAPI.Mapping
{
    public class CreateUserBillTransactionMapper : Profile
    {
        public CreateUserBillTransactionMapper()
        {
            CreateMap<CreateUserBillTransactionInputDto, BillPayerInfo>().ReverseMap();
            CreateMap<CreateUserBillTransactionInputDto, BillTransaction>().ReverseMap();
            CreateMap<ChargesInputDto, ChargesResponseDto>().ReverseMap();
            CreateMap<ChargesInputDto, BillCharge>().ReverseMap();
            CreateMap<CreateBillChargeInputDto, BillCharge>().ReverseMap();
        }
    }
}
