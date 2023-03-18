using System.Security.Cryptography;
using System.Text;

namespace BillProcessorAPI.Helpers.Paythru
{
    public class PaythruConfig
    {
        //creates the Datahash for payment creation
        public static string HashForPaythruPaymentCreation(string amount, string secret)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(amount + secret);
                byte[] hash = sha512.ComputeHash(bytes);
                string hashString = Convert.ToHexString(hash);
                return hashString;
            }

        }

        //creates the Datahash for payment notification
        public static string HashForPaythruPaymentNotification(string amount, string commission, string residualAmount, string clientSecret)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(amount + commission + residualAmount + clientSecret);
                byte[] hash = sha512.ComputeHash(bytes);
                string hashString = Convert.ToHexString(hash);
                return hashString;
            }

        }

        //hash ffor login
        public static string HashForPaythruLoginPassword(string secret, string timeStamp)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                //var load = $"{secret}{timeStamp}";
                byte[] bytes = Encoding.UTF8.GetBytes(secret + timeStamp);
                byte[] hash = sha256.ComputeHash(bytes);
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("x2"));
                }
                var result = builder.ToString();
                return result;
            }

        }

        public static string GenerateTransactionReference(string billCode)
        {

            Random random = new Random();
            
            // Generate 16 random digits
            string digits = "";
            for (int i = 0; i < 20; i++)
            {
                digits += random.Next(10).ToString();
            }

            // Combine the alphabet character and the digits
            string randomNumber = $"{billCode}{digits}";

            return randomNumber;
        }

    }
}
