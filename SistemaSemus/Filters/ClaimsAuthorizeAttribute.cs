using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace SistemaSemus.Filters
{
    public class ClaimsAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string _claimType;
        private readonly string _claimValue;

        public ClaimsAuthorizeAttribute(string claimType, string claimValue)
        {
            _claimType = claimType;
            _claimValue = claimValue;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var identity = (ClaimsIdentity)httpContext.User.Identity;
            var claim = identity.Claims.SingleOrDefault(c => c.Type == _claimType);

            if (claim != null)
            {
                return claim.Value == _claimValue;
            }
            return false;
        }
    }
}