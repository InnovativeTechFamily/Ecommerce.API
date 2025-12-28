using Ecommerce.API.Data;
using Ecommerce.API.Utils;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;

namespace Ecommerce.API.Middleware
{
    public class IsAuthenticatedAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            var config = httpContext.RequestServices.GetRequiredService<IConfiguration>();
            var db = httpContext.RequestServices.GetRequiredService<ApplicationDbContext>();

            var token = httpContext.Request.Cookies["token"];

            if (string.IsNullOrEmpty(token))
            {
                throw new ErrorHandler("Please login to continue", 401);
            }

            try
            {
               // var tokenHandler = new JwtSecurityTokenHandler();
                //var key = System.Text.Encoding.UTF8.GetBytes(config["JWT_SECRET_KEY"]!);

                //var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                //{
                //    ValidateIssuer = false,
                //    ValidateAudience = false,
                //    ValidateIssuerSigningKey = true,
                //    IssuerSigningKey = new SymmetricSecurityKey(key),
                //    ValidateLifetime = true
                //}, out _);
                var principal = JwtTokenHelper.ValidateToken(token, config);

                int userId = Convert.ToInt32(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? principal.FindFirst("id")?.Value);

                if (userId == null)
                {
                    throw new ErrorHandler("Please login to continue", 401);
                }

                var user = await db.Users.FindAsync(userId);
                if (user == null)
                {
                    throw new ErrorHandler("User doesn't exists", 400);
                }

                // Attach user to HttpContext like req.user
                httpContext.Items["User"] = user;
            }
            catch (SecurityTokenException ex)
            {
                throw new ErrorHandler(ex.Message, 401);
            }
        }
    }
}
