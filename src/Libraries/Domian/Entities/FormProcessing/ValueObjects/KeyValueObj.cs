
using System;
namespace Domain.Entities.FormProcessing.ValueObjects
{
    /// <summary>
    /// Object that holds infor of the required header key and value for form submission.
    /// </summary>
    public class KeyValueObj
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public bool IsValueResolvedAtRuntime { get; set; } = true;
    }
}

