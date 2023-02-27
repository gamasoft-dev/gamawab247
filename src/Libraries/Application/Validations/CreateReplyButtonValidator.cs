using System.Linq;
using Application.DTOs;
using Application.DTOs.CreateDialogDtos;
using Application.DTOs.OutboundMessageRequests;
using FluentValidation;

namespace Application.Validations;


public class CreateButtonValidator : AbstractValidator<CreateBusinessMessageDto<CreateButtonMessageDto>>
{
    public CreateButtonValidator()
    {
        RuleFor(x => x.Position).NotEmpty().GreaterThan(0);
        RuleFor(x => x.BusinessId).NotEmpty();
        RuleFor(x => x.MessageTypeObject).NotEmpty();
        RuleFor(x => x.MessageTypeObject.Footer).Length(1, 60).WithMessage("Footer of a button message cannot be greater 60 characters");
        RuleFor(x => x.MessageTypeObject.Body).NotEmpty().Length(1, 1024).WithMessage("Body message cannot exceed 1024 characters");
        RuleFor(x => x.MessageTypeObject.Header).Length(1, 20).WithMessage("Header should not be more than 20 characters");
        RuleForEach(x => x.MessageTypeObject.ButtonAction.Buttons).SetValidator(new ReplyButtonValidator());

    }
}

public class ReplyButtonValidator : AbstractValidator<ButtonDto>
{
    public ReplyButtonValidator()
    {
        RuleFor(x => x.NextBusinessMessagePosition).NotEmpty().GreaterThan(0).WithMessage("Provide the next message postion for this button option");
        RuleFor(x => x.reply).NotNull().WithMessage("reply object cannot be null");
        RuleFor(x => x.reply.title).Length(1, 20).WithMessage("Button message cannot be more than 20 characters");
    }
}