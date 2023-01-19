using System.Linq;
using Application.DTOs;
using Application.DTOs.CreateDialogDtos;
using FluentValidation;

namespace Application.Validations;

public class CreateBusinessMessageValidator
{
    public class CreateButtonValidator: AbstractValidator<CreateBusinessMessageDto<CreateButtonMessageDto>>
    {
        public CreateButtonValidator()
        {
            RuleFor(x => x.Position).NotEmpty().GreaterThan(0);
            RuleFor(x => x.BusinessId).NotEmpty();
            RuleFor(x => x.MessageTypeObject).NotEmpty();
            RuleFor(x => x.MessageTypeObject.Body).NotEmpty();
            RuleFor(x => x.MessageTypeObject.ButtonAction.Buttons.Count).GreaterThan(0);
        }
    }
    
    public class CreateListValidator: AbstractValidator<CreateBusinessMessageDto<CreateListMessageDto>>
    {
        public CreateListValidator()
        {
            RuleFor(x => x.Position).NotEmpty().GreaterThan(0);
            RuleFor(x => x.BusinessId).NotEmpty();
            RuleFor(x => x.MessageTypeObject).NotEmpty();
            RuleFor(x => x.MessageTypeObject.ListAction.Sections.Count).GreaterThan(0);
            RuleFor(x => x.MessageTypeObject.ListAction.Button).NotEmpty();
            RuleFor(x => x.MessageTypeObject.Body).NotEmpty();
        }
    }
}