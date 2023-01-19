using System;
using System.Text.RegularExpressions;
using Application.Exceptions;
using Application.Services.Interfaces.FormProcessing;
using Domain.Enums;

namespace Application.Services.Implementations.FormProcessing.Validations
{
    public class DateFormElementValidatorService: IFormElementValidatorService
    {
        
        public string ValidatorName => EValidatorName.DATE_INPUT_VALIDATOR.ToString();

        public void ValidateFormElementInput(string input, string formElementName)
        {
            string errorMessage = $"Your date input {input} could be validated correctly. " +
                   $"Please review and enter your {formElementName} in the format [dd/mm/yyyy]";

            DateTime now = DateTime.UtcNow;
            try
            {
               
                Regex regex = new Regex(@"(((0|1)[0-9]|2[0-9]|3[0-1])\/(0[1-9]|1[0-2])\/((19|20)\d\d))$");
                bool isValid = regex.IsMatch(input.Trim());

                if (!isValid)
                {
                    throw new FormInputValidationException(errorMessage,
                        nameof(DateFormElementValidatorService.ValidateFormElementInput));
                }

               //var tempInput = input.Trim('/');
                if(int.TryParse(input.Trim('/'), out int inputInt))
                    throw new FormInputValidationException(errorMessage,
                        nameof(DateFormElementValidatorService.ValidateFormElementInput));

                var tempInput = input.Split('/');
                if(tempInput.Length != 3)
                    throw new FormInputValidationException(errorMessage,
                        nameof(DateFormElementValidatorService.ValidateFormElementInput));

                DateOnly date = new DateOnly(int.Parse(tempInput[2]),
                    int.Parse(tempInput[1]),
                    int.Parse(tempInput[0]));

                //if(date < new DateOnly(now.Year - 18, now.Month, now.Day))
                //{
                //    throw new FormInputValidationException("You are below the required age, confirm your age", null);
                //}
            }
            catch
            {
                throw new FormInputValidationException(errorMessage, nameof(DateFormElementValidatorService));
            }
        }
    }
}

