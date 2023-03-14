using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public class TicketIdGenerator
    {
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
