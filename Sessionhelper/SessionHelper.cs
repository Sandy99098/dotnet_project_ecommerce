

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace dotnet_project_ecommerce.sessionhelper
{
    public static class SessionHelper
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static string GetCustomerSession()
        {
            return _httpContextAccessor.HttpContext.Session.GetString("CustomerSession");
        }

        public static void SetCustomerSession(string value)
        {
            _httpContextAccessor.HttpContext.Session.SetString("CustomerSession", value);
        }

        public static void RemoveCustomerSession()
        {
            _httpContextAccessor.HttpContext.Session.Remove("CustomerSession");
        }
    }
}