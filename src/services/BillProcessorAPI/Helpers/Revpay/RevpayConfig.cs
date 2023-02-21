using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace BillProcessorAPI.Helpers.Revpay
{
    public class RevpayConfig
    {

        //creates the Datahash
        public static string GenerateSHA512Hash(string key, string webGuid, string state = null)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(key + webGuid + state);
                byte[] hash = sha512.ComputeHash(bytes);
                string hashString = Convert.ToHexString(hash);
                return hashString;
            }

        }
    }
}
