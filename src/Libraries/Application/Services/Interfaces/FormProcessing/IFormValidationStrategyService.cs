using Application.AutofacDI;
using Application.Helpers;

namespace Application.Services.Interfaces.FormProcessing
{
    public interface IFormValidationStrategyService: IAutoDependencyService
    {
        void ValidateInput(string validatorKey, string input, string formElementName);
    }
}

