using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Services.Interfaces;

namespace Application.Services.Implementations
{
    public class UtilService: IUtilService
    {
        public UtilService()
        {
        }

        public async Task<ICollection<string>> IntentExtractor(string sentence)
        {
            throw new NotImplementedException();
        }
    }
}

