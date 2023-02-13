using AutoMapper;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Entities;

namespace BillProcessorAPI.Mapper
{
	public class BillTransactionMapper :Profile
	{
		public BillTransactionMapper()
		{
			CreateMap<BillTransaction, BillPaymentVerificationResponseDto>().ReverseMap();
			CreateMap<BillTransaction, BillPaymentVerificationRequestDto>().ReverseMap();

		}
	}
}
