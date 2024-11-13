using BillProcessorAPI.Helpers;
using BillProcessorAPI.Helpers.Revpay;
using BillProcessorAPI.Services.Interfaces;
using Domain.Common;
using Domain.Common.ShortLink.ValueObjects;
using Domain.Exceptions;
using Infrastructure.Http;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace BillProcessorAPI.Services.Implementations
{
    public class CutlyService : ICutlyService
    {
        private CutlyOptions _options;
        private readonly RevpayOptions _revpayOptions;
        private readonly IHttpService _httpService;
        private readonly ILogger<CutlyService> _logger;
        public CutlyService(IOptions<CutlyOptions> options, IHttpService httpService, IOptions<RevpayOptions> revpayOptions, ILogger<CutlyService> logger)
        {
            _options = options.Value;
            _httpService = httpService;
            _revpayOptions = revpayOptions.Value;
            _logger = logger;
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

        public async Task<SuccessResponse<string>> GenerateShortenedPaymentLink(string waId,string billCode)
        {
            var endPointParams = $"?billCode={billCode}&phoneNumber={waId}";
            var paymentLink = $"{_revpayOptions.BillCodePaymentPageLink}{endPointParams}";
            var shortLink = await ShortLink(paymentLink);
            _logger.LogInformation($"system generated payment link: {paymentLink}");
            return new SuccessResponse<string>
            {
                Data = shortLink,
                Message = "Successful",
                Success = true,
            };
        }

    }
}
