using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public static class BusinessSettingsUtility
    {
        public static string GenerateApiKey()
        {
            Random rnd = new Random();
            var randomNumber = (rnd.Next(100000, 999999)).ToString();
            return BCrypt.Net.BCrypt.GenerateSalt(int.Parse(randomNumber));
        }

        public static string GetFirstMessage(string botName, string botDescription, string whatsappUserName)
        {
            botDescription = botDescription ?? "Virtual Assistant";
            
            string initMessage = $"{GetGreeting()} {whatsappUserName}, \n \n" +
                                 $"My name is {botName}, {botDescription}";
            return initMessage;
        }

        private static string GetGreeting()
        {
            var now = DateTime.UtcNow;
            var greeting = "Good Morning";
            if (now > new DateTime(now.Year, now.Month, now.Day, 12, 00, 00))
                greeting = "Good Afternoon";

            if (now > new DateTime(now.Year, now.Month, now.Day, 16, 00, 00))
                greeting = "Good evening";

            if (now > new DateTime(now.Year, now.Month, now.Day, 21, 00, 00))
                greeting = "Hello";

            return greeting;
        }
    }
}
