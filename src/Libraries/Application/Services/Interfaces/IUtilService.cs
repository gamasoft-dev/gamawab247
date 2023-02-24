using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.AutofacDI;
using Application.Helpers;

namespace Application.Services.Interfaces
{
    public interface IUtilService: IAutoDependencyService
    {
        Task<ICollection<string>> IntentExtractor(string sentence);
    }
}

