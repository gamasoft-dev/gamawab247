using Application.Helpers;
using Application.AutofacDI;

namespace Application.Services.Interfaces.FormProcessing
{
    public interface IFormValidationStrategyService: IAutoDependencyService
    {
        void ValidateInput(string validatorKey, string input, string formElementName);
    }
}

