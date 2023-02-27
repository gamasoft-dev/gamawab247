using System;
using Application.AutofacDI;
using Application.Helpers;

namespace Application.Services.Interfaces.FormProcessing
{
    public interface IFormElementValidatorService: IAutoDependencyService
    {
        string ValidatorName { get; }

        void ValidateFormElementInput(string input, string formElementName); // this method throws an exception if validation fails
    }
}

