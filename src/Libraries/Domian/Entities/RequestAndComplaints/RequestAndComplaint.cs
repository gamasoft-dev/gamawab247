using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Identities;
using Domain.Enums;

namespace Domain.Entities.RequestAndComplaints
{
    public class RequestAndComplaint
    {
        public Guid Id { get; set; }

        /// <summary>
        /// This is either user whatsapp username or phonenumber, instgram username etc
        /// </summary>
        public string CustomerId { get; set; }

        public string Subject { get; set; }

        public string Channel { get; set; }

        public string Detail { get; set; }

        public List<string> Responses { get; set; }

        /// <summary>
        /// This is the complaint or request unique identifier. This is generated for every ticker raised
        /// </summary>
        public string TicketId { get; set; }

        public Guid BusinessId { get; set; }

        public ERequestComplaintType Type { get; set; }

        public string CallBackUrl { get; set; }

        public DateTime? ResolutionDate { get; set; }

        public string ResolutionStatus { get; set; } = EResolutionStatus.Pending.ToString();

        public Guid? TreatedById { get; set; }

        public User TreatedBy { get; set; }

        //TODO: Implement a unique 16 digit alphanumeric generator for ticketId
        public static string GenerateTicketId()
        {

            Random random = new Random();
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            // Generate a random alphabet character
            char firstChar = alphabet[random.Next(alphabet.Length)];

            // Generate 15 random digits
            string digits = "";
            for (int i = 0; i < 15; i++)
            {
                digits += random.Next(10).ToString();
            }

            // Combine the alphabet character and the digits
            string randomNumber = $"{firstChar}{digits}";

            return randomNumber;
        }        
    }
}
