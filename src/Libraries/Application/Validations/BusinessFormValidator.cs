using System;
using Application.DTOs.BusinessDtos;
using Application.DTOs.CreateDialogDtos;
using Domain.Entities.FormProcessing.ValueObjects;
using FluentValidation;

namespace Application.Validations
{
	public class CreateBusinessFormValidator:AbstractValidator<CreateBusinessFormDto>
    {
        public CreateBusinessFormValidator()
        {
            RuleFor(x => x.FormElements.Count).GreaterThanOrEqualTo(1).WithMessage("Atleast one or more form elements must be configured");
            RuleFor(x => x.BusinessId).NotEmpty();
            RuleForEach(x => x.FormElements).SetValidator(new FormElementValidator());
        }
    }


    public class UpdateBusinessFormValidator : AbstractValidator<UpdateBusinessFormDto>
    {
        public UpdateBusinessFormValidator()
        {
            RuleFor(x => x.FormElements.Count).GreaterThanOrEqualTo(1).WithMessage("Atleast one or more form elements must be configured");
            RuleFor(x => x.BusinessId).NotEmpty();
            RuleForEach(x => x.FormElements).SetValidator(new FormElementValidator());
        }
    }




    public class FormElementValidator : AbstractValidator<FormElement>
    {
        public FormElementValidator()
        {
            RuleFor(x => x.Position).LessThanOrEqualTo(100).GreaterThan(0).WithMessage("Position should be above 0 & less than 100");
            RuleFor(x => x.Key).NotNull().WithMessage("The key property must have a value");
        }
    }




}

