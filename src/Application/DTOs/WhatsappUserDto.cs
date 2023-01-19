using System;

namespace Application.DTOs
{
    public class UpsertWhatsappUserDto
    {
        public string WaId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime LastMessageTime { get; set; }
    }

    public class WhatsappUserDto
    {
        public string WaId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime LastMessageTime { get; set; }
    }
}
