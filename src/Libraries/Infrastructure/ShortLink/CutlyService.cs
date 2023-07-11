using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
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
        public async Task<string> ShortLink(string link)
        {
            //encode the url so that its been shortened appropriately without breaking the link
            var encodedUrl = UrlEncoder.Default.Encode(link);

            var parameter = $"?key={_options.Key}&short={encodedUrl}";
            var fullUrl = $"{_options.BaseUrl}/{parameter}";
            
            
            try
            {
                IDictionary<string, string> dictNew = new Dictionary<string, string>();
                var header = new RequestHeader(dictNew);
                var shortLink = encodedUrl;

                var cutlyResponse = await _httpService.Post<CutlyResponse, string>(fullUrl, header, encodedUrl);
                if (cutlyResponse.Status == 200)
                {
                    shortLink = cutlyResponse.Data.Url.ShortLink;
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
