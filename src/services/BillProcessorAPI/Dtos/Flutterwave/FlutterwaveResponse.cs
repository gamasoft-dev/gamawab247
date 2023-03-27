using Newtonsoft.Json;

namespace BillProcessorAPI.Dtos.Flutterwave
{
    public class FlutterwaveResponse<T> where T : class
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public class LinkData
    {
        [JsonProperty("link")]
        public string Link { get; set; }
    }
}
