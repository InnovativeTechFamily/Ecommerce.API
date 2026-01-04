using Ecommerce.API.Data;
using Ecommerce.API.DTOs;
using Ecommerce.API.DTOs.Shop;
using Ecommerce.API.DTOs.User;
using Ecommerce.API.Entities.Shops;
using Ecommerce.API.Middleware;
using Ecommerce.API.Services;
using Ecommerce.API.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ICloudinaryService _cloudinaryService; // inject Cloudinary service
        private readonly IEmailService _emailService;    // inject email service
        private readonly IConfiguration _config;

        public ShopController(
            ApplicationDbContext db,
            ICloudinaryService cloudinary,
            IEmailService emailService,
            IConfiguration config)
        {
            _db = db;
            _cloudinaryService = cloudinary;
            _emailService = emailService;
            _config = config;
        }

        [HttpPost("create-shop")]
        public async Task<IActionResult> CreateShop([FromBody] CreateSellerDto dto)
        {
            // Check if shop email already exists (like Shop.findOne({ email }))
            var existingShop = await _db.Shops.FirstOrDefaultAsync(s => s.Email == dto.Email);
            if (existingShop != null)
            {
                throw new ErrorHandler("Seller already exists", 400);
            }

            // Handle avatar upload if provided
            CloudinaryUploadResponse? cloudinaryAvatar = null;
            if (dto.Avatar !=null)
            {
                try
                {
                    var uploadResult = await _cloudinaryService.UploadImageAsync(
                        dto.Avatar, "avatars");

                    cloudinaryAvatar = new CloudinaryUploadResponse
                    {
                        PublicId = uploadResult.PublicId,
                        Url = uploadResult.Url.ToString(),
                        SecureUrl = uploadResult.SecureUrl.ToString()
                    };
                }
                catch (Exception ex)
                {
                    //_logger.LogError(ex, "Failed to upload avatar to Cloudinary");
                    throw new ErrorHandler("Failed to upload avatar", 500);
                }
            }
            // Upload avatar to Cloudinary (like cloudinary.v2.uploader.upload)
          //  var myCloud = await _cloudinary.UploadImageAsync(dto.Avatar.Base64Data ?? "", "avatars");

            // Create seller entity (like your seller object)
            var seller = new Shop
            {
                Name = dto.Name,
                Email = dto.Email,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                ZipCode = int.Parse(dto.ZipCode),
               AvatarPublicId = cloudinaryAvatar.PublicId,
                AvatarUrl = cloudinaryAvatar.SecureUrl,
            };

            // Hash password before saving
            seller.SetPassword(dto.Password);

            // Generate activation token (implement createActivationToken equivalent)
            var activationToken = CreateActivationToken(seller);

            // Generate activation URL
            var activationUrl = $"{_config["Frontend:BaseUrl"]}/shop/activation/{activationToken}";

            try
            {
                // Send activation email
                //await _emailService.SendAsync(new EmailRequest
                //{
                //    To = seller.Email,
                //    Subject = "Activate your Shop",
                //    Body = $"Hello {seller.Name}, please click on the link to activate your shop: {activationUrl}"
                //});
                await _emailService.SendEmailAsync(
                        seller.Email,
                        "Activate your Shop",
                        $"Hello {seller.Name}, please click on the link to activate your account: {activationUrl}");

                // Don't save to DB yet - wait for activation
                return StatusCode(StatusCodes.Status201Created, new
                {
                    success = true,
                    message = $"please check your email: {seller.Email} to activate your shop!",
                    token = activationToken
                });
            }
            catch (Exception ex)
            {
                throw new ErrorHandler(ex.Message, 500);
            }
        }
        [HttpPost("activation")]
        public async Task<IActionResult> ActivateShop([FromBody] ActivationRequestDto dto)
        {
            try
            {
                var activationToken = dto.ActivationToken;

                // Verify JWT activation token (like jwt.verify)
                var newSeller = VerifyActivationToken(activationToken);
                if (newSeller == null)
                {
                    throw new ErrorHandler("Invalid token", 400);
                }

                // Extract data from verified token
                var sellerData = newSeller;
                var email = sellerData.Email;

                // Check if shop already exists
                var existingSeller = await _db.Shops.FirstOrDefaultAsync(s => s.Email == email);
                if (existingSeller != null)
                {
                    throw new ErrorHandler("Shop already exists", 400);
                }

                // Create shop from token data
                var seller = new Shop
                {
                    Name = sellerData.Name,
                    Email = email,
                    PasswordHash = sellerData.PasswordHash, // already hashed from create-shop
                    AvatarPublicId = sellerData.AvatarPublicId,
                    AvatarUrl = sellerData.AvatarUrl,
                    Address = sellerData.Address,
                    PhoneNumber = sellerData.PhoneNumber,
                    ZipCode = sellerData.ZipCode,
                    Role = "Seller"
                };

                // Save to database (like Shop.create)
                _db.Shops.Add(seller);
                await _db.SaveChangesAsync();

                // Send JWT token in response cookie (like sendShopToken)
               // SendShopToken(seller, 201);

                return StatusCode(StatusCodes.Status201Created, new
                {
                    success = true,
                    message = "Shop activated successfully!"
                });
            }
            catch (Exception ex)
            {
                throw new ErrorHandler(ex.Message, 500);
            }
        }
        [HttpPost("login-shop")]
        public async Task<IActionResult> LoginShop([FromBody] ShopLoginDto dto)
        {
            try
            {
                var email = dto.Email;
                var password = dto.Password;

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    throw new ErrorHandler("Please provide the all fields!", 400);
                }

                // Equivalent to Shop.findOne({ email }).select("+password")
                var shop = await _db.Shops
                    .FirstOrDefaultAsync(s => s.Email == email);

                if (shop == null)
                {
                    throw new ErrorHandler("Seller doesn't exists!", 400);
                }

                // Compare password (equivalent to shop.comparePassword(password))
                var isPasswordValid = shop.ComparePassword(password); // uses BCrypt.Verify inside

                if (!isPasswordValid)
                {
                    throw new ErrorHandler(
                        "Please provide the correct information",
                        400
                    );
                }

                // sendShopToken(shop, 201, res);
                return SendShopTokenResult(shop);
            }
            catch (ErrorHandler)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ErrorHandler(ex.Message, 500);
            }
        }
        [HttpGet("getSeller")]
        [IsSeller] // like isSeller middleware
        public IActionResult GetSeller()
        {
            // Seller loaded by IsSellerAttribute
            var seller = HttpContext.Items["Seller"] as Shop;

            if (seller == null)
            {
                throw new ErrorHandler("Seller doesn't exists", 400);
            }

            return Ok(new
            {
                success = true,
                seller
            });
        }
        [HttpGet("logout")]
        [IsSeller] // like isSeller middleware
        public IActionResult Logout()
        {
            try
            {
                // Clear seller_token cookie (equivalent to res.cookie("seller_token", null, ...))
                Response.Cookies.Append("seller_token", string.Empty, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow,  // expire immediately (like new Date(Date.now()))
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Secure = true
                });

                return StatusCode(StatusCodes.Status201Created, new
                {
                    success = true,
                    message = "Log out successful!"
                });
            }
            catch (Exception ex)
            {
                throw new ErrorHandler(ex.Message, 500);
            }
        }
        [HttpGet("get-shop-info/{id}")]
        public async Task<IActionResult> GetShopInfo(Guid id)
        {
            try
            {
                // Equivalent to Shop.findById(req.params.id)
                var shop = await _db.Shops.FindAsync(id);

                if (shop == null)
                {
                    throw new ErrorHandler("Shop not found", 404);
                }

                return StatusCode(StatusCodes.Status201Created, new
                {
                    success = true,
                    shop
                });
            }
            catch (Exception ex)
            {
                throw new ErrorHandler(ex.Message, 500);
            }
        }
        [HttpPut("update-shop-avatar")]
        [IsSeller]
        public async Task<IActionResult> UpdateShopAvatar([FromBody] UpdateAvatarDto dto)
        {
            try
            {
                // Seller loaded by IsSellerAttribute (like req.seller)
                var seller = HttpContext.Items["Seller"] as Shop;

                if (seller == null)
                {
                    throw new ErrorHandler("Seller not found", 400);
                }

                // Delete old image from Cloudinary (like cloudinary.v2.uploader.destroy)
                if (!string.IsNullOrEmpty(seller.AvatarPublicId))
                {
                    await _cloudinaryService.DeleteImageAsync(seller.AvatarPublicId);
                }
                CloudinaryUploadResponse? cloudinaryAvatar = null;
                // Upload new image (like cloudinary.v2.uploader.upload)
                var uploadResult = await _cloudinaryService.UploadImageAsync(dto.Avatar.Base64Data, "avatars");
                cloudinaryAvatar = new CloudinaryUploadResponse
                {
                    PublicId = uploadResult.PublicId,
                    Url = uploadResult.Url.ToString(),
                    SecureUrl = uploadResult.SecureUrl.ToString()
                };
                // Update avatar fields
                seller.AvatarPublicId = cloudinaryAvatar.PublicId;
                seller.AvatarUrl = cloudinaryAvatar.SecureUrl;

                // Save changes (like existsSeller.save())
                _db.Shops.Update(seller);
                await _db.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    seller
                });
            }
            catch (Exception ex)
            {
                throw new ErrorHandler(ex.Message, 500);
            }
        }
        [HttpPut("update-seller-info")]
        [IsSeller]
        public async Task<IActionResult> UpdateSellerInfo([FromBody] UpdateSellerInfoDto dto)
        {
            try
            {
                // Seller loaded by IsSellerAttribute (like req.seller)
                var shop = HttpContext.Items["Seller"] as Shop;

                if (shop == null)
                {
                    throw new ErrorHandler("User not found", 400);
                }

                // Update fields (like shop.name = name, etc.)
                shop.Name = dto.Name;
                shop.Description = dto.Description;
                shop.Address = dto.Address;
                shop.PhoneNumber = dto.PhoneNumber;
                shop.ZipCode = int.Parse(dto.ZipCode);

                // Save changes (like shop.save())
                _db.Shops.Update(shop);
                await _db.SaveChangesAsync();

                return StatusCode(StatusCodes.Status201Created, new
                {
                    success = true,
                    shop
                });
            }
            catch (Exception ex)
            {
                throw new ErrorHandler(ex.Message, 500);
            }
        }
        [HttpGet("admin-all-sellers")]
        [IsAuthenticated]  // your existing attribute
        [IsAdmin("Admin")]  // new attribute
        public async Task<IActionResult> AdminAllSellers()
        {
            try
            {
                // Equivalent to Shop.find().sort({ createdAt: -1 })
                var sellers = await _db.Shops
                    .OrderByDescending(s => s.CreatedAt)
                    .ToListAsync();

                return StatusCode(StatusCodes.Status201Created, new
                {
                    success = true,
                    sellers
                });
            }
            catch (Exception ex)
            {
                throw new ErrorHandler(ex.Message, 500);
            }
        }
        [HttpDelete("delete-seller/{id}")]
        [IsAuthenticated]  // your existing attribute
        [IsAdmin("Admin")]  // existing attribute
        public async Task<IActionResult> DeleteSeller(Guid id)
        {
            try
            {
                // Equivalent to Shop.findById(req.params.id)
                var seller = await _db.Shops.FindAsync(id);

                if (seller == null)
                {
                    throw new ErrorHandler(
                        "Seller is not available with this id",
                        400
                    );
                }

                // Equivalent to Shop.findByIdAndDelete(req.params.id)
                _db.Shops.Remove(seller);
                await _db.SaveChangesAsync();

                return StatusCode(StatusCodes.Status201Created, new
                {
                    success = true,
                    message = "Seller deleted successfully!"
                });
            }
            catch (Exception ex)
            {
                throw new ErrorHandler(ex.Message, 500);
            }
        }


        [HttpPut("update-payment-methods")]
        [IsSeller]
        public async Task<IActionResult> UpdatePaymentMethods([FromBody] UpdatePaymentMethodDto dto)
        {
            try
            {
                // Seller loaded by IsSellerAttribute (like req.seller)
                var seller = HttpContext.Items["Seller"] as Shop;

                if (seller == null)
                {
                    throw new ErrorHandler("Seller not found", 400);
                }

                // Update withdrawMethod (like Shop.findByIdAndUpdate)
                seller.WithdrawMethodJson = dto.WithdrawMethodJson;

                // Save changes
                _db.Shops.Update(seller);
                await _db.SaveChangesAsync();

                return StatusCode(StatusCodes.Status201Created, new
                {
                    success = true,
                    seller
                });
            }
            catch (Exception ex)
            {
                throw new ErrorHandler(ex.Message, 500);
            }
        }

            [HttpDelete("delete-withdraw-method")]
            [IsSeller]
            public async Task<IActionResult> DeleteWithdrawMethod()
            {
                try
                {
                    // Seller loaded by IsSellerAttribute (like req.seller)
                    var seller = HttpContext.Items["Seller"] as Shop;

                    if (seller == null)
                    {
                        throw new ErrorHandler("Seller not found with this id", 400);
                    }

                    // Clear withdraw method (like seller.withdrawMethod = null)
                    seller.WithdrawMethodJson = null;

                    // Save changes (like seller.save())
                    _db.Shops.Update(seller);
                    await _db.SaveChangesAsync();

                    return StatusCode(StatusCodes.Status201Created, new
                    {
                        success = true,
                        seller
                    });
                }
                catch (Exception ex)
                {
                    throw new ErrorHandler(ex.Message, 500);
                }
            }
            // Verify activation token (equivalent to jwt.verify)
            private ShopActivationData? VerifyActivationToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_config["Activation:SecretKey"]!);

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                var id = principal.FindFirst("id")?.Value;
          //      var email = principal.FindFirst("email")?.Value;
                var email = principal.FindFirst(ClaimTypes.Email)?.Value;
                var name = principal.FindFirst(ClaimTypes.Name)?.Value;
                var passwordHash = principal.FindFirst("passwordHash")?.Value;
                var avatarPublicId = principal.FindFirst("avatarPublicId")?.Value;
                var avatarUrl = principal.FindFirst("avatarUrl")?.Value;
                var address = principal.FindFirst("address")?.Value;
                var phoneNumber = principal.FindFirst("phoneNumber")?.Value;
                var zipCode = principal.FindFirst("zipCode")?.Value;

                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(email))
                    return null;

                return new ShopActivationData
                {
                    Id = Guid.Parse(id),
                    Name = name!,
                    Email = email,
                    PasswordHash = passwordHash!,
                    AvatarPublicId = avatarPublicId!,
                    AvatarUrl = avatarUrl!,
                    Address = address!,
                    PhoneNumber = phoneNumber!,
                    ZipCode = int.Parse(zipCode!)
                };
            }
            catch
            {
                return null;
            }
        }

        // Send auth token cookie (equivalent to sendShopToken)
        //private void SendShopToken(Shop seller, int statusCode)
        //{
        //    var token = seller.GetJwtToken(_config["JWT_SECRET_KEY"]!, TimeSpan.FromDays(7));

        //    Response.Cookies.Append("token", token, new CookieOptions
        //    {
        //        HttpOnly = true,
        //        Expires = DateTime.UtcNow.AddDays(7),
        //        SameSite = SameSiteMode.None,
        //        Secure = true
        //    });
        //}


        // Helper method (equivalent to createActivationToken)
        private string CreateActivationToken(Shop seller)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Activation:SecretKey"]!);

            var claims = new List<Claim>
    {
        new Claim("id", seller.Id.ToString()),
        new Claim(ClaimTypes.Email, seller.Email), // Use standard claim type for email
        new Claim(ClaimTypes.Name, seller.Name), // Use standard claim type for name
        new Claim("passwordHash", seller.PasswordHash ?? ""),
        new Claim("phoneNumber", seller.PhoneNumber ?? ""),
    };

            // Add optional claims only if they exist
            if (seller.Address != null && !string.IsNullOrEmpty(seller.Address))
                claims.Add(new Claim("address", seller.Address));

            // Fix for CS0472 and CS1503 errors in CreateActivationToken method

            // Replace this block:
            if (seller.ZipCode != null)
              //  claims.Add(new Claim("zipCode", seller.ZipCode));

            // With this:
            claims.Add(new Claim("zipCode", seller.ZipCode.ToString()));

            if (seller.AvatarPublicId != null)
            {
                claims.Add(new Claim("avatarPublicId", seller.AvatarPublicId ?? ""));
                claims.Add(new Claim("avatarUrl", seller.AvatarUrl ?? ""));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7), // 7 days for activation
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                //Issuer = _config["JWT:Issuer"], // Add issuer if you have it
               // Audience = _config["JWT:Audience"] // Add audience if you have it
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // In ShopController.cs
        private void SendShopToken(Shop shop, int statusCode)
        {
            var token = shop.GetJwtToken(_config["Activation:SecretKey"]!, TimeSpan.FromDays(90)); // 90 days

            Response.Cookies.Append("seller_token", token, new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(90),  // 90 * 24 * 60 * 60 * 1000 ms = 90 days
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true
            });

            Response.StatusCode = statusCode;

            // Return JSON response (like your res.json)
            Response.ContentType = "application/json";
            var response = new
            {
                success = true,
                user = shop,
                token
            };

            Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
        }

        // Alternative: Return IActionResult for cleaner controller usage
        private IActionResult SendShopTokenResult(Shop shop)
        {
            var token = shop.GetJwtToken(_config["Activation:SecretKey"]!, TimeSpan.FromDays(90));

            Response.Cookies.Append("seller_token", token, new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(90),
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true
            });

            return StatusCode(StatusCodes.Status201Created, new
            {
                success = true,
                user = shop,
                token
            });
        }


    }
}
