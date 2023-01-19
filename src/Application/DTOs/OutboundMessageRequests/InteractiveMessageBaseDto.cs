namespace Application.DTOs.InteractiveMesageDto.InboundMessageDto
{
    /// <summary>
    /// This a parent class of common properties in list and reply button interactive message
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class InteractiveMessageBaseDto<T>
    {
        public string recipient_type { get; set; }
        public string to { get; set; }
        public string type { get; set; }

        public virtual T interactive { get; set; }
    }
    public class Body
    {
        public string text { get; set; }
    }

    public class Footer
    {
        public string text { get; set; }
    }
    public class Header
    {
        public string type { get; set; }
        public string text { get; set; }
    }
}


