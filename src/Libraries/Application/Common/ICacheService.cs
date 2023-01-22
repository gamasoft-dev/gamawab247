using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface ICacheService
    {
        Task AddToCacheAsync<T>(string key, T payload);
        Task<T> ReadFromCacheAsync<T>(string key) where T : class;
        Task UpdateCache<T>(string key, T payload) where T : class;
        Task RemoveFromCache(string key);
    }
}
