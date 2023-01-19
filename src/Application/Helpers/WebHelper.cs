using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;

namespace Application.Helpers
{
    public class WebHelper
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static ClaimsPrincipal CurrentUser => _httpContextAccessor?.HttpContext?.User;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static HttpContext HttpContext
        {
            get { return _httpContextAccessor.HttpContext; }
        }

        public static Guid UserId
        {
            get
            {
                Guid id;

                var userId = _httpContextAccessor.HttpContext.User.Claims.Where(x => x.Type == ClaimTypeHelper.UserId).FirstOrDefault()?.Value ?? "";

                Guid.TryParse(userId, out id);

                return id;
            }
        }
    }
}
