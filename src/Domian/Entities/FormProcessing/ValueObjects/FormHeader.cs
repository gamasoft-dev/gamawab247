
using System;
namespace Domain.Entities.FormProcessing.ValueObjects
{
    /// <summary>
    /// Object that holds infor of the required header key and value for form submission.
    /// </summary>
    public class FormHeader
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}

