namespace BillProcessorAPI.Dtos.Paythru
{
    public class PaythruLoginResponseDto
    {
        public int code { get; set; }
        public string message { get; set; }
        public string data { get; set; }
        public bool successful { get; set; }
    }
}
