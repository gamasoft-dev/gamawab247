using Application.DTOs;
using Application.Services.Interfaces;
using Domain.Common;
using Infrastructure.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public class WebHookRegistrationHelper : IWebHookRegistrationHelper
    {
        private readonly Dialog360Settings _dialog360Settings;
        private readonly IHttpService _httpService;
        public WebHookRegistrationHelper(IOptions<Dialog360Settings> options, IHttpService httpService)
        {
            _dialog360Settings = options.Value;
            _httpService = httpService;
        }
        public async Task<bool> RegisterBusinessWebHookUrl(string webHookUrl, string apiKey)
        {
            if (string.IsNullOrEmpty(webHookUrl))
                throw new RestException(System.Net.HttpStatusCode.BadRequest, "Parameter cannot be empty");

            const string endpoint = "v1/configs/webhook";
            var url = $"{_dialog360Settings.BaseUrl}/{endpoint}";

            //  retrieve api key from business settings.
            IDictionary<string, string> dictNew = new Dictionary<string, string>();
            dictNew.Add(_dialog360Settings?.AuthorizationName, apiKey);

            var header = new RequestHeader(dictNew);
            var request = new WebhookRequestDto
            {
                url = webHookUrl
            };

            var httpResult = await _httpService.Post<WebhookRequestDto, WebhookRequestDto>(fullUrl: url, header: header, request: request);
            if (httpResult.Data !=null)
                return true;
            return false;
        }
    }
}
