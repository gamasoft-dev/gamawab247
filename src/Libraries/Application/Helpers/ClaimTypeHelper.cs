using System.Security.Claims;

namespace Application.Helpers
{
    public class ClaimTypeHelper
    {
        public static string UserId { get; set; } = "UserId";
        public static string Email { get; set; } = "Email";
        public static string FullName { get; set; } = "FullName";
        public static string Role { get; set; } = ClaimTypes.Role;
        public static string BusinessId { get; set; } = "BusinessId";
    }
}
