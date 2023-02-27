using System.Linq;
using Application.DTOs;
using Application.DTOs.CreateDialogDtos;
using Application.DTOs.OutboundMessageRequests;
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
            RuleFor(x => x.MessageTypeObject.Footer).Length(1, 60).WithMessage("Footer of a button message cannot be greater 60 characters");
            RuleFor(x => x.MessageTypeObject.Body).NotEmpty().Length(1, 1024).WithMessage("Body message cannot exceed 1024 characters");
            RuleFor(x => x.MessageTypeObject.Header).Length(1, 20).WithMessage("Header should not be more than 20 characters");
            RuleForEach(x => x.MessageTypeObject.ButtonAction.Buttons).SetValidator(new ReplyButtonValidator());

        }
    }

    public class CreateTextMesageValidator : AbstractValidator<CreateBusinessMessageDto<CreateTextMessageDto>>
    {
        public CreateTextMesageValidator()
        {
            RuleFor(x => x.Position).NotEmpty().GreaterThan(0);
            RuleFor(x => x.BusinessId).NotEmpty();
            RuleFor(x => x.MessageTypeObject).NotEmpty();
            RuleFor(x => x.MessageTypeObject.Footer).Length(1, 100).WithMessage("Footer of a button message cannot be greater 100 characters");
            RuleFor(x => x.MessageTypeObject.Header).Length(1, 100).WithMessage("Header should not be more than 100 characters");
           // RuleForEach(x => x.MessageTypeObject.Buttons).SetValidator(new ReplyButtonValidator());

        }
    }

    public class CreateListValidator: AbstractValidator<CreateBusinessMessageDto<CreateListMessageDto>>
    {
        public CreateListValidator()
        {
            RuleFor(x => x.Position).NotEmpty().GreaterThan(0);
            RuleFor(x => x.BusinessId).NotEmpty();
            RuleFor(x => x.MessageTypeObject).NotEmpty().NotNull().WithMessage("Message type object cannot be null");
            RuleForEach(x => x.MessageTypeObject.ListAction.Sections).SetValidator(new ListSectionValidator());
           // RuleFor(x => x.MessageTypeObject.ListAction.Button).NotEmpty();
            RuleFor(x => x.MessageTypeObject.Body).NotEmpty();
            RuleFor(x => x.MessageTypeObject.Header).Length(0, 60).WithMessage("List header cannot exceed 60 characters");
            RuleFor(x => x.MessageTypeObject.Footer).Length(0, 30).WithMessage("List footer cannot exceed 30 characters");
            RuleFor(x => x.MessageTypeObject.Body).Length(0,1024).WithMessage("Your list message’s body cannot be more than 1024 characters.");
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


    public class ListRowDtoValidator : AbstractValidator<RowDto>
    {
        public ListRowDtoValidator()
        {
            RuleFor(x => x.NextBusinessMessagePosition).NotEmpty().GreaterThan(0).WithMessage("Provide the next message postion for this list option");
            RuleFor(x => x.Description).Length(0, 72).WithMessage("List option desction message cannot be more than 72 characters");
            RuleFor(x => x.Title).Length(0, 24).WithMessage("List option title message cannot be more than 24 characters");

        }
    }

    public class ListSectionValidator : AbstractValidator<SectionDto>
    {
        public ListSectionValidator()
        {
            RuleFor(x => x.Rows.Count).LessThanOrEqualTo(10).GreaterThan(0).WithMessage("Section list rows must be above 1row and cannot exceed a total of 10 rows");
            RuleFor(x => x.Title).Length(0, 24).WithMessage("List Section title cannot exceed 24 characters");
            RuleFor(x => x.Description).Length(0, 72).WithMessage("List Section description cannot exceed 72 characters");

            RuleForEach(x => x.Rows).SetValidator(new ListRowDtoValidator());
        }
    }
}