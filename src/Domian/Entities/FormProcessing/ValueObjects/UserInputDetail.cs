using System;
namespace Domain.Entities.FormProcessing.ValueObjects
{
    /// <summary>
    /// This is a value object of the users inputs relative to a form element or request
    /// eg. PayloadKey= firstName, UserValue= 'chuks'
    /// </summary>
    public class UserInputDetail
    {
        public int Id { get; set; }
        public string FormElementKey{ get; set; }
        public string UserValue { get; set; }
        public bool IsValidated { get; set; }
        public bool IsConfirmed { get; set; }
    }
}

