using System;
using System.Collections.Generic;
using System.Linq;
using Application.Exceptions;
using Application.Services.Interfaces.FormProcessing;

namespace Application.Services.Implementations.FormProcessing.Validations
{
    /// <summary>
    /// This is a strategy pattern service that handles form validation by validatorKey
    /// </summary>
    public class FormValidationStrategyService: IFormValidationStrategyService
    {
        private readonly IEnumerable<IFormElementValidatorService> _formElementValidationServices;

        public FormValidationStrategyService(IEnumerable<IFormElementValidatorService> formElementValidationServices)
        {
            _formElementValidationServices = formElementValidationServices;
        }

        public void ValidateInput(string validatorName, string input, string formElementName)
        {
            var validatorService = _formElementValidationServices.FirstOrDefault(x => x.ValidatorName == validatorName);
            if (validatorService is null)
            {
                throw new FormConfigurationException(sendErrorPromptWithDefaultMessage: true);
            }

            validatorService.ValidateFormElementInput(input, formElementName);
        }
    }
}

