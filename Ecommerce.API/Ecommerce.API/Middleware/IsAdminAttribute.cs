using Ecommerce.API.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ecommerce.API.Middleware
{
    public class IsAdminAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _allowedRoles;

        public IsAdminAttribute(params string[] roles)
        {
            _allowedRoles = roles; // like isAdmin("Admin")
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Get user from IsAuthenticatedAttribute (req.user equivalent)
            var user = context.HttpContext.Items["User"];

            if (user == null)
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    success = false,
                    message = "Authentication required"
                });
                return;
            }

            // Check if user role is in allowed roles (like roles.includes(req.user.role))
            var userRole = user.GetType().GetProperty("Role")?.GetValue(user)?.ToString();

            if (!_allowedRoles.Contains(userRole))
            {
                throw new ErrorHandler(
                    $"{userRole} can not access this resources!",
                    403
                );
            }
        }
    }
}
