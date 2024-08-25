using System.Net;
using BillProcessorAPI.Dtos.BroadcastMessage;
using BillProcessorAPI.Entities;
using BillProcessorAPI.Helpers;
using Infrastructure.Http;
using Infrastructure.ShortLink;
using Microsoft.Extensions.Options;

namespace BillProcessorAPI.Helpers.BroadcastMessage
{
    public class ReceiptBroadcast
    {

        public static async Task SendReceipt
            (BillTransaction transaction, BusinessesPhoneNumber phoneNumberOptions,
             ICutlyService cutlyService, ReceiptBroadcastConfig receiptBroadcastOptions,
             IHttpService httpService
            )
        {
            if (string.IsNullOrEmpty(phoneNumberOptions.LUC.PhoneNumber))
            {
                throw new RestException(HttpStatusCode.PreconditionFailed, "LUC business phone number not configured");
            }

            if (!string.IsNullOrEmpty(transaction.ReceiptUrl))
            {
                var shortReceiptUrl = await cutlyService.ShortLink(transaction.ReceiptUrl.ToString());

                var broadcastMessage = new CreateBroadcastMessageDto
                {
                    From = phoneNumberOptions.LUC.PhoneNumber,
                    Message = $"Please click on the link below to download your payment receipt.{Environment.NewLine}{Environment.NewLine}{shortReceiptUrl}",
                    To = transaction.PhoneNumber,
                    FullName= transaction.PayerName,
                    EmailAddress = transaction.Email
                };

                var gamawabsBroadcastUrl = receiptBroadcastOptions.Url;
                var postBroadcast = await httpService.Post<BroadcastMessageDto, CreateBroadcastMessageDto>(gamawabsBroadcastUrl, null, broadcastMessage);
                if (postBroadcast.Data.Id != Guid.Empty)
                {
                    transaction.isReceiptSent = true;
                }
            }
        }
    }
}
