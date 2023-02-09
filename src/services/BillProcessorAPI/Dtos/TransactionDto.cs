using BillProcessorAPI.Enums;
using System.Text.Json.Serialization;

namespace BillProcessorAPI.Dtos
{
    public record CreateBillChargeInputDto
    {
        public string ChannelModel { get; set; }
        public decimal MaxChargeAmount { get; set; }
        public decimal MinChargeAmount { get; set; }
        public double PercentageCharge { get; set; }
    }

    public record ChargesInputDto : CreateBillChargeInputDto
    {
        public decimal Amount { get; set; }
    }

    public record ChargesResponseDto
    {
        public string ChannelModel { get; set; }
        public decimal MaxChargeAmount { get; set; }
        public decimal MinChargeAmount { get; set; }
        public decimal Amount { get; set; }
        public double PercentageCharge { get; set; }
        public decimal AmountCharge { get; set; }
    }

    public record TransactionVerificationInputDto
    {
        public string TransactionReference { get; set; }
        public string TransactionId { get; set; }
        public decimal AmountPaid { get; set; }
        public string Status { get; set; }
        public string BillNumber { get; set; }
    }

    public record CreateUserBillTransactionInputDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string BillNumber { get; set; }
        public string PropertyNumber { get; set; }
        public string Purpose { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DueDate { get; set; }
        public EGatewayType GatewayType { get; set; }
        public string SystemReference { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal BillAmount { get; set; }
        public decimal PrincipalPay { get; set; }
        public decimal TransactionCharge { get; set; }
        public EPaymentChannel Channel { get; set; }
    }

    public class Card
    {
        [JsonPropertyName("first_6digits")]
        public string First6digits { get; set; }

        [JsonPropertyName("last_4digits")]
        public string Last4digits { get; set; }

        [JsonPropertyName("issuer")]
        public string Issuer { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("expiry")]
        public string Expiry { get; set; }
    }

    public class Customer
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
    }

    public class Data
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("tx_ref")]
        public string TxRef { get; set; }

        [JsonPropertyName("flw_ref")]
        public string FlwRef { get; set; }

        [JsonPropertyName("device_fingerprint")]
        public string DeviceFingerprint { get; set; }

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("charged_amount")]
        public int ChargedAmount { get; set; }

        [JsonPropertyName("app_fee")]
        public double AppFee { get; set; }

        [JsonPropertyName("merchant_fee")]
        public int MerchantFee { get; set; }

        [JsonPropertyName("processor_response")]
        public string ProcessorResponse { get; set; }

        [JsonPropertyName("auth_model")]
        public string AuthModel { get; set; }

        [JsonPropertyName("ip")]
        public string Ip { get; set; }

        [JsonPropertyName("narration")]
        public string Narration { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("payment_type")]
        public string PaymentType { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("account_id")]
        public int AccountId { get; set; }

        [JsonPropertyName("card")]
        public Card Card { get; set; }

        [JsonPropertyName("meta")]
        public object Meta { get; set; }

        [JsonPropertyName("amount_settled")]
        public double AmountSettled { get; set; }

        [JsonPropertyName("customer")]
        public Customer Customer { get; set; }
    }

    public class TransactionVerificationResponseDto
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("data")]
        public Data Data { get; set; }
    }
}
