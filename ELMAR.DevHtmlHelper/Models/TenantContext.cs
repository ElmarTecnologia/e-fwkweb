using Microsoft.AspNetCore.Http;

namespace ELMAR.DevHtmlHelper.Models
{
    public class TenantContext : ITenantContext
    {
        public readonly IHttpContextAccessor _contextAccessor;

        public TenantContext(IHttpContextAccessor contextAccessor) {
            _contextAccessor = contextAccessor;
        }

        public string GetIdTenant()
        {
            return _contextAccessor.HttpContext.Request.Headers.ContainsKey("X-Tenant-Id")
                ? _contextAccessor.HttpContext.Request.Headers["X-Tenant-Id"].ToString() : "999025";
        }
    }
}
