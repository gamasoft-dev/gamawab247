using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Cache
{
    public class RedisCacheConfig
    {
        public string Server { get; set; }
        public string InstanceName { get; set; }
        public int Port { get; set; }
        public double CacheExpirationTime { get; set; }
        public string Env { get; set; }
        public string Auth { get; set; }
    }
}