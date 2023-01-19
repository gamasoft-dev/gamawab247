using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class JwtConfigSettings
    {
        public string Secret { get; set; }
        public int TokenLifespan { get; set; }
        public string ValidIssuer { get; set; }
    }
}