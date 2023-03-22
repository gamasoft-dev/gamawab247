using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Domain.Common;
using Domain.Common.ShortLink.ValueObjects;
using Domain.Exceptions;
using Infrastructure.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure.ShortLink
{
    public class CutlyService : ICutlyService
    {
        private CutlyOptions _options;
        private readonly IHttpService _httpService;
        public CutlyService(IOptions<CutlyOptions> options, IHttpService httpService)
        {
            _options = options.Value;
            _httpService = httpService;
        }
        public string ShortLink(string link)
        {
            var parameter = $"?key={_options.Key}&short={link}";
            var fullUrl = $"{_options.BaseUrl}/{parameter}";
            try
            {
                IDictionary<string, string> dictNew = new Dictionary<string, string>();
                var header = new RequestHeader(dictNew);
                var shortLink = link;

                var cutlyResponse = _httpService.Post<CutlyResponse, string>(fullUrl, header, link);
                if (cutlyResponse.Result.Status == 200)
                {
                    shortLink = cutlyResponse.Result.Data.Url.ShortLink;
                }
                return shortLink;
            }
            catch (Exception ex)
            {

                throw new BackgroundException("an error occured while shortening the link", ex.InnerException);
            }
        }
            
    }
}
