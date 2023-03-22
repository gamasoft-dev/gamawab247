using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCustomization.ShortLinkService
{
    public interface ICutlyService
    {
        string ShortLink(string link);
    }
}
