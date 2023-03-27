using Newtonsoft.Json;

namespace BillProcessorAPI.Dtos.Flutterwave
{
    public class FCreateTransactionRequestDto
    {

        [JsonProperty("tx_ref")]
        public string Tx_ref { get; set; }
        [JsonProperty("amount")]
        public decimal Amount { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; } = "NGN";
        [JsonProperty("redirect_url")]
        public string Redirect_url { get; set; }
        [JsonProperty("customer")]
        public CustomerDto Customer { get; set; }
        [JsonProperty("customization")]
        public CustomizationDto Customization { get; set; }

    }

    public class CustomerDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class CustomizationDto
    {
        public string Title { get; set; }
        public string Logo { get; set; }
    }

   
}
