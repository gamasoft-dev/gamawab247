using Newtonsoft.Json;

namespace BillProcessorAPI.Dtos.Flutterwave
{
    public record WebHookNotificationWrapper
    {
        
        public string WebGuid { get; set; }
      
        public string ResponseCode { get; set; }
     
        public string ResponseDesc { get; set; }
       
        public string ReceiptNumber { get; set; }
    
        public string State { get; set; }
    
        public string Status { get; set; }
        
        public string TransID { get; set; }
        
        public string TransCode { get; set; }
        
        public string StatusMessage { get; set; }
        
        public string PropertyAddress { get; set; }

        
        public string TransactionReference { get; set; }
       
        public dynamic PaymentRef { get; set; }

    }


    public record Card
    {
        [JsonProperty("first_6digits")]
        public string first_6digits { get; set; }
        [JsonProperty("last_4digits")]
        public string last_4digits { get; set; }
        [JsonProperty("issuer")]
        public string issuer { get; set; }
        [JsonProperty("country")]
        public string country { get; set; }
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("expiry")]
        public string expiry { get; set; }
    }

    public record Customer
    {
        [JsonProperty("id")]
        public int id { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("phone_number")]
        public object phone_number { get; set; }
        [JsonProperty("email")]
        public string email { get; set; }
        [JsonProperty("created_at")]
        public DateTime created_at { get; set; }
    }

    public record FlutterwaveResponseData
    {
        [JsonProperty("id")]
        public int id { get; set; }
        [JsonProperty("tx_ref")]
        public string tx_ref { get; set; }
        [JsonProperty("flw_ref")]
        public string flw_ref { get; set; }
        [JsonProperty("device_fingerprint")]
        public string device_fingerprint { get; set; }
        [JsonProperty("amount")]
        public decimal amount { get; set; }
        [JsonProperty("currency")]
        public string currency { get; set; }
        [JsonProperty("charged_amount")]
        public decimal charged_amount { get; set; }
        [JsonProperty("app_fee")]
        public double app_fee { get; set; }
        [JsonProperty("merchant_fee")]
        public int merchant_fee { get; set; }
        [JsonProperty("processor_response")]
        public string processor_response { get; set; }
        [JsonProperty("auth_model")]
        public string auth_model { get; set; }
        [JsonProperty("ip")]
        public string ip { get; set; }
        [JsonProperty("narration")]
        public string narration { get; set; }
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("payment_type")]
        public string payment_type { get; set; }
        [JsonProperty("created_at")]
        public DateTime created_at { get; set; }
        [JsonProperty("account_id")]
        public int account_id { get; set; }
        [JsonProperty("customer")]
        public Customer customer { get; set; }

        [JsonProperty("receipt_url")]
        public string receipt_url { get; set; } = "";

        [JsonProperty("card")]
        public Card card { get; set; }
    }

}
