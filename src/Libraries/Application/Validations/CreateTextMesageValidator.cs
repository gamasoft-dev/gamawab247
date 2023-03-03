using Application.DTOs.CreateDialogDtos;
using FluentValidation;

namespace Application.Validations;

public class CreateTextMesageValidator : AbstractValidator<CreateBusinessMessageDto<CreateTextMessageDto>>
{
    public CreateTextMesageValidator()
    {
        RuleFor(x => x.Position).NotEmpty().GreaterThan(0);
        RuleFor(x => x.BusinessId).NotEmpty();
        When(x => x.MessageTypeObject != null, ()=>  RuleFor(x=> x.MessageTypeObject.Footer).Length(1, 100).WithMessage("Footer of a button message cannot be greater 100 characters"));
        When(x => x.MessageTypeObject != null, ()=> RuleFor(x =>x.MessageTypeObject.Header).Length(1, 100).WithMessage("Header should not be more than 100 characters"));
    }
}