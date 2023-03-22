using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common.ShortLink.ValueObjects
{
    public class CutlyResponse
    {
        public CutlyObj Url { get; set; }

    }
    public class CutlyObj
    {
        public string Date { get; set; }
        public string ShortLink { get; set; }
        public string FullLink { get; set; }
        public string Title { get; set; }
    }
}
