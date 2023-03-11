using AutoMapper;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Entities;

namespace BillProcessorAPI.Mapper
{
	public class BillPayerMapper:Profile
	{
		public BillPayerMapper()
		{
			CreateMap<BillPayerInfo, BillPayerInfoDto>().ReverseMap();
			CreateMap<BillPayerInfo, BillRequestDto>().ReverseMap();
			CreateMap<BillPayerInfo, BillReferenceResponseDto>().ReverseMap();
		}
	}
}
