using System;
using Application.DTOs.CreateDialogDtos;
using Application.DTOs.RequestAndComplaintDtos;
using Domain.Enums;
using FluentValidation;

namespace Gamawabs247API.Validations
{
	public class RequestAndComplaintValidator: AbstractValidator<SimpleUpdateRequestAndComplaint>
    {
		public RequestAndComplaintValidator()
		{
            RuleFor(x => x.Response).NotEmpty().WithMessage("Response must be provided to resolve a request or complain");
            RuleFor(x => x.ResolutionStatus).NotEmpty().WithMessage("Desired resolution status must be provided");

            RuleFor(x => x.ResolutionStatus).IsEnumName(typeof(EResolutionStatus), true);

        }
    }
}

