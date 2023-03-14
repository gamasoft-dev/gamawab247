using BillProcessorAPI.Dtos;
using BillProcessorAPI.Enums;
using FluentValidation;

namespace BillProcessorAPI.Validators
{
    public class TransactionValidator: AbstractValidator<TransactionVerificationInputDto>
    {
        public TransactionValidator()
        {
            RuleFor(x => x.TransactionReference).NotEmpty().WithMessage("TransactionReference is required");
            RuleFor(x => x.TransactionId).NotEmpty().WithMessage("TransactionId is required");
            RuleFor(x => x.AmountPaid).NotNull().WithMessage("AmountPaid is required");
            RuleFor(x => x.Status).NotEmpty().WithMessage("Status is required");
            RuleFor(x => x.BillNumber).NotEmpty().WithMessage("BillNumber is required");
        }
    }

    public class CreateUserBillTransactionInputValidator : AbstractValidator<CreateUserBillTransactionInputDto>
    {
        public CreateUserBillTransactionInputValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Address).NotEmpty();
            RuleFor(x => x.Email).EmailAddress().NotEmpty();
            RuleFor(x => x.PhoneNumber).NotEmpty();
            RuleFor(x => x.PropertyNumber).NotEmpty();
            RuleFor(x => x.GatewayType).IsInEnum();
            RuleFor(x => x.SystemReference).NotEmpty();
            RuleFor(x => x.BillAmount).NotEmpty();
            RuleFor(x => x.PrincipalPay).NotEmpty();
            RuleFor(x => x.TransactionCharge).NotEmpty();
            RuleFor(x => x.Channel).NotEmpty();
        }
    }

    public class ChargesInputValidator : AbstractValidator<ChargesInputDto>
    {
        public ChargesInputValidator()
        {
            RuleFor(x => x.ChannelModel).NotEmpty();
            RuleFor(x => x.MaxChargeAmount).NotEmpty();
            RuleFor(x => x.MinChargeAmount).NotEmpty();
            RuleFor(x => x.Amount).NotEmpty();
            RuleFor(x => x.PercentageCharge)
                .GreaterThan(0)
                .LessThanOrEqualTo(100);
        }
    }
}
