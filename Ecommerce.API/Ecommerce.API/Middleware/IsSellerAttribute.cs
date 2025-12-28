using Ecommerce.API.Data;
using Ecommerce.API.Utils;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Ecommerce.API.Middleware
{
    public class IsSellerAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            var config = httpContext.RequestServices.GetRequiredService<IConfiguration>();
            var db = httpContext.RequestServices.GetRequiredService<ApplicationDbContext>();

            // Read seller_token cookie (like req.cookies.seller_token)
            var sellerToken = httpContext.Request.Cookies["seller_token"];

            if (string.IsNullOrEmpty(sellerToken))
            {
                throw new ErrorHandler("Please login to continue", 401);
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = System.Text.Encoding.UTF8.GetBytes(config["Activation:SecretKey"]!);

                var principal = tokenHandler.ValidateToken(sellerToken, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true
                }, out _);

                var sellerId = principal.FindFirst("id")?.Value;

                if (sellerId == null)
                {
                    throw new ErrorHandler("Please login to continue", 401);
                }

                // Load seller from DB (like Shop.findById(decoded.id))
                var seller = await db.Shops.FindAsync(Guid.Parse(sellerId));

                if (seller == null)
                {
                    throw new ErrorHandler("Seller doesn't exists", 400);
                }

                // Attach to HttpContext (like req.seller)
                httpContext.Items["Seller"] = seller;

            //    // Optional: Add claims to User for [Authorize] if needed later
            //    var claims = new List<Claim>
            //{
            //    new Claim(ClaimTypes.NameIdentifier, seller.Id.ToString()),
            //    new Claim("role", seller.Role)
            //};
            //    var identity = new ClaimsIdentity(claims, "Seller");
            //    httpContext.User.AddIdentity(identity);
            }
            catch (SecurityTokenException ex)
            {
                throw new ErrorHandler(ex.Message, 401);
            }
        }
    }
}
