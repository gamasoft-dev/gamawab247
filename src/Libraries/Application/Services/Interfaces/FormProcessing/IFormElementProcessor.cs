using Application.Helpers;
using System;
using System.Text.RegularExpressions;

namespace Application.Services.Interfaces.FormProcessing
{
    public interface IFormElementProcessor : IAutoDependencyService
    {
        string name { get; }
        FormProcessorResult DateValidator(dynamic formValue);

    }

    public class FormDateProcessor : IFormElementProcessor
    {
        public string name => throw new NotImplementedException();

        public FormProcessorResult DateValidator(dynamic formValue)
        {
            Regex regex = new Regex(@"(((0|1)[0-9]|2[0-9]|3[0-1])\/(0[1-9]|1[0-2])\/((19|20)\d\d))$");
            bool isValid = regex.IsMatch(formValue.Trim());

            if (!isValid)
            {
                return new FormProcessorResult
                {
                    Message = "Enter correct date format, 'dd/mm/yyy'",
                    isSuccessful = false
                };
            }
            return formValue;
        }
    }

    public class FormProcessorResult
    {
        public  string Message { get; set; }
        public bool isSuccessful { get; set; }
    }

}

