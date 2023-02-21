using Application.DTOs;
using FluentValidation;

namespace Application.Validations
{
    public class UserValidator: AbstractValidator<CreateUserDTO>
    {
        public UserValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Password).MinimumLength(8).WithMessage("Password cannot be less than 8 characters");

            //RuleFor(x => x.Password).Matches(@"(?-i)(?=^.{8,}$)((?!.*\s)(?=.*[A-Z])(?=.*[a-z]))((?=(.*\d){1,})|(?=(.*\W){1,}))^.*$")
            //    .WithMessage(@"Password must be at least 8 characters, at least 1 upper case letters (A – Z), Atleast 1 lower case letters (a – z), Atleast 1 number (0 – 9) or non-alphanumeric symbol (e.g. @ '$%£! ')");
        }
    }
    public class UserLoginValidator : AbstractValidator<UserLoginDTO>
    {
        public UserLoginValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty(); 
        }
    }
    public class ResetPassowordValidator : AbstractValidator<ResetPasswordDTO>
    {
        public ResetPassowordValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
    public class VerifyTokenValidator : AbstractValidator<VerifyTokenDTO>
    {
        public VerifyTokenValidator()
        {
            RuleFor(x => x.Token).NotEmpty();
        }
    }
    public class SetPasswordValidator : AbstractValidator<SetPasswordDTO>
    {
        public SetPasswordValidator()
        {
            RuleFor(x => x.Token).NotEmpty();
            RuleFor(x => x.Password).Matches(@"(?-i)(?=^.{8,}$)((?!.*\s)(?=.*[A-Z])(?=.*[a-z]))((?=(.*\d){1,})|(?=(.*\W){1,}))^.*$")
               .WithMessage(@"Password must be at least 8 characters, at least 1 upper case letters (A – Z), Atleast 1 lower case letters (a – z), Atleast 1 number (0 – 9) or non-alphanumeric symbol (e.g. @ '$%£! ')");
        }
    }
}