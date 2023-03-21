using AutoMapper;
using BillProcessorAPI.Dtos;

namespace BillProcessorAPI.Mapper
{
    public class ChargeMapper : Profile
    {
        public ChargeMapper()
        {
            CreateMap<ChargesResponseDto, LucChargesInputDto>().ReverseMap();
        }
    }
}
