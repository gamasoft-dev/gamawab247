namespace BillProcessorAPI.Entities.FlutterwaveEntities
{
    public class PaymentTransaction
    {
        public int Id { get; set; }
        public string TrxReference { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; } = "NGN";
        public string RedirectUrl { get; set; }
        public Customer Customer { get; set; }
        public Customization Customization { get; set; }


    }

    public class Customization
    {
        public string Title { get; set; }
        public string Logo { get; set; }
    }

    public class Customer
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    }
}
