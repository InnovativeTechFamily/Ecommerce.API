using Ecommerce.API.Data;
using Ecommerce.API.DTOs.User;
using Ecommerce.API.Entities.Users;
using Ecommerce.API.Exceptions;
using Ecommerce.API.Middleware;
using Ecommerce.API.Services;
using Ecommerce.API.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<UsersController> _logger;
        //asdf
        public UsersController(
            ApplicationDbContext context,
            IConfiguration configuration,
            IEmailService emailService,
            ICloudinaryService cloudinaryService,
            ILogger<UsersController> logger)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }

        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == createUserDto.Email);

                if (existingUser != null)
                {
                    throw new ErrorHandler("User already exists", 400);
                }

                // Handle avatar upload if provided
                CloudinaryUploadResponse? cloudinaryAvatar = null;
                if (!string.IsNullOrEmpty(createUserDto.Avatar))
                {
                    try
                    {
                        var uploadResult = await _cloudinaryService.UploadImageAsync(
                            createUserDto.Avatar, "avatars");

                        cloudinaryAvatar = new CloudinaryUploadResponse
                        {
                            PublicId = uploadResult.PublicId,
                            Url = uploadResult.Url.ToString(),
                            SecureUrl = uploadResult.SecureUrl.ToString()
                        };
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to upload avatar to Cloudinary");
                        throw new ErrorHandler("Failed to upload avatar", 500);
                    }
                }

                // Create activation token
                //var userForActivation = new
                //{
                //    createUserDto.Name,
                //    createUserDto.Email,
                //    createUserDto.Password,
                //    Avatar = cloudinaryAvatar
                //};

                var activationToken = GenerateActivationToken(
                                        createUserDto.Name,
                                        createUserDto.Email,
                                        createUserDto.Password,
                                        cloudinaryAvatar
                                    );
                var activationUrl = $"{_configuration["Frontend:BaseUrl"]}/activation/{activationToken}";

                // Send activation email
                try
                {
                    await _emailService.SendEmailAsync(
                        createUserDto.Email,
                        "Activate your account",
                        $"Hello {createUserDto.Name}, please click on the link to activate your account: {activationUrl}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send activation email");
                    throw new ErrorHandler("Failed to send activation email", 500);
                }

                return Ok(new
                {
                    success = true,
                    message = $"Please check your email: {createUserDto.Email} to activate your account!",
                    token = activationToken
                });
            }
            catch (ErrorHandler)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                throw new ErrorHandler(ex.Message, 400);
            }
        }

        [HttpPost("activation")]
        public async Task<IActionResult> ActivateUser([FromBody] ActivationDto activationDto)
        {
            try
            {
                var userData = ValidateActivationToken(activationDto.ActivationToken);
                if (userData == null)
                {
                    throw new ErrorHandler("Invalid token", 400);
                }

                // Check if user already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == userData.Email);

                if (existingUser != null)
                {
                    throw new ErrorHandler("User already exists", 400);
                }

                // Create user
                var user = new User
                {
                    Name = userData.Name,
                    Email = userData.Email,
                    Password = PasswordHelper.HashPassword(userData.Password),
                    CreatedAt = DateTime.UtcNow
                };

                // Handle avatar if provided in activation data
                if (userData.Avatar != null)
                {
                    user.Avatar = new Avatar
                    {
                        PublicId = userData.Avatar.PublicId,
                        Url = userData.Avatar.SecureUrl ?? userData.Avatar.Url
                    };
                }

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                // Generate JWT token
                var token = JwtTokenHelper.GenerateJwtToken(user, _configuration);

                return Ok(new
                {
                    success = true,
                    token = token
                });
            }
            catch (ErrorHandler)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user");
                throw new ErrorHandler(ex.Message, 500);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                {
                    throw new ErrorHandler("Please provide all fields!", 400);
                }

                var user = await _context.Users
                    .Include(u => u.Avatar)
                    .Include(u => u.Addresses)
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

                if (user == null)
                {
                    throw new ErrorHandler("User doesn't exist!", 400);
                }

                if (!PasswordHelper.VerifyPassword(loginDto.Password, user.Password))
                {
                    throw new ErrorHandler("Please provide the correct information", 400);
                }

                var token = JwtTokenHelper.GenerateJwtToken(user, _configuration);
                var userResponse = MapUserToDto(user);

                // Set cookie
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(90),
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Secure = true
                };

                Response.Cookies.Append("token", token, cookieOptions);

                return Ok(new AuthResponseDto
                {
                    Success = true,
                    User = userResponse,
                    Token = token
                });
            }
            catch (ErrorHandler)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                throw new ErrorHandler(ex.Message, 500);
            }
        }
        [HttpGet("logout")]
        [IsAuthenticated] // same as your isAuthenticated middleware
        public IActionResult Logout()
        {
            try
            {
                // Option 1: overwrite cookie with expired one (closest to your Node code)
                Response.Cookies.Append("token", string.Empty, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow,        // expire immediately
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,           // "none"
                    Secure = true                           // secure: true
                });

                // Option 2 (alternative): delete cookie by name
                // Response.Cookies.Delete("token");

                return StatusCode(StatusCodes.Status201Created, new
                {
                    success = true,
                    message = "Log out successful!"
                });
            }
            catch (Exception ex)
            {
                // This will be caught by GlobalExceptionMiddleware
                throw new ErrorHandler(ex.Message, 500);
            }
        }
		[HttpGet("getUser")]
		[IsAuthenticated]
		public async Task<IActionResult> GetUser()
		{
			var tokenUser = HttpContext.Items["User"] as User;

			if (tokenUser == null)
				throw new ErrorHandler("User doesn't exist", 400);

			var user = await _context.Users
				.Include(u => u.Addresses)
				.Include(u => u.Avatar)
				.FirstOrDefaultAsync(u => u.Id == tokenUser.Id);

			if (user == null)
				throw new ErrorHandler("User doesn't exist", 400);

			// ✅ USE EXISTING MAPPER
			var response = MapUserToDto(user);

			return Ok(new
			{
				success = true,
				user = response
			});
		}


		[HttpPut("update-user-info")]
        [IsAuthenticated]
        public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserInfoDto dto)
        {
            try
            {
                var email = dto.Email;
                var password = dto.Password;
                var phoneNumber = dto.PhoneNumber;
                var name = dto.Name;

                // Find user by email (like User.findOne({ email }).select("+password"))
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    throw new ErrorHandler("User not found", 400);
                }

                // Verify password (like user.comparePassword(password))
                var isPasswordValid = user.ComparePassword(password);

                if (!isPasswordValid)
                {
                    throw new ErrorHandler(
                        "Please provide the correct information",
                        400
                    );
                }

                // Update user fields (like user.name = name, etc.)
                user.Name = name;
                //user.Email = email;
                user.PhoneNumber = phoneNumber;

                // Save changes (like user.save())
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status201Created, new
                {
                    success = true,
                    user
                });
            }
            catch (Exception ex)
            {
                throw new ErrorHandler(ex.Message, 500);
            }
        }
        //  [Authorize]
        [HttpPost("upload-avatar")]
        [IsAuthenticated]
        public async Task<IActionResult> UploadAvatar([FromBody] AvatarUploadDto avatarDto)
        {
            try
            {
                //var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                //var user = await _context.Users
                //    .Include(u => u.Avatar)
                //    .FirstOrDefaultAsync(u => u.Id == userId);

                //if (user == null)
                //{
                //    throw new ErrorHandler("User not found", 404);
                //}
                // user was loaded in IsAuthenticatedAttribute
                var userObj = HttpContext.Items["User"];
                var user = MapObjectToUserEntity(userObj!);
                if (user == null)
                {
                    throw new ErrorHandler("User doesn't exists", 400);
                }

                // Delete old avatar from Cloudinary if exists
                if (user.Avatar != null && !string.IsNullOrEmpty(user.Avatar.PublicId))
                {
                    try
                    {
                        await _cloudinaryService.DeleteImageAsync(user.Avatar.PublicId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to delete old avatar from Cloudinary");
                    }

                    _context.Avatars.Remove(user.Avatar);
                }

                // Upload new avatar to Cloudinary
                var uploadResult = await _cloudinaryService.UploadImageAsync(
                    avatarDto.ImageData, "avatars");

            

                var avatar = new Avatar
                {
                    PublicId = uploadResult.PublicId,
                    Url = uploadResult.SecureUrl.ToString(),
                    SecureUrl = uploadResult.SecureUrl.ToString(),
                    UserId = user.Id
                };

                await _context.Avatars.AddAsync(avatar);


                await _context.SaveChangesAsync();

                return Ok(new CloudinaryUploadResponse
                {
                    PublicId = uploadResult.PublicId,
                    Url = uploadResult.Url.ToString(),
                    SecureUrl = uploadResult.SecureUrl.ToString()
                });
            }
            catch (ErrorHandler)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading avatar");
                throw new ErrorHandler($"Failed to upload avatar: {ex.Message}", 500);
            }
        }


		[HttpPost("addresses")]
		[IsAuthenticated]
		public async Task<IActionResult> CreateUserAddresses([FromBody] CreateUserAddressDto createUserAddressDto)
		{
			try
			{
				// User loaded by IsAuthenticatedAttribute (like req.user)
				var userId = (HttpContext.Items["User"] as User)?.Id;

				if (userId == null)
				{
					throw new ErrorHandler("User not found", 400);
				}


                if(AddressType.Home.ToString() != createUserAddressDto.AddressType && AddressType.Office.ToString() != createUserAddressDto.AddressType)
                {
					throw new ErrorHandler(
						$"{createUserAddressDto.AddressType} Please Select AddressType Either Home Or Office ",
						400
					);
				}

				var isUserExist = await _context.UserAddresses.AnyAsync(u => u.UserId == userId && u.AddressType == createUserAddressDto.AddressType);
                if (isUserExist) 
                {
                    return BadRequest(new ErrorHandler("An address with this AddressType already exists for this user.", 400));
				}

				// Add new address to collection (like user.addresses.push)
				var add =new UserAddress
					{
						Address1 = createUserAddressDto.AddressLine1,
						Address2 = createUserAddressDto.AddressLine2,
						City = createUserAddressDto.City,
						State = createUserAddressDto.State,
                        Country = createUserAddressDto.Country,
						ZipCode = createUserAddressDto.ZipCode,
						AddressType = createUserAddressDto.AddressType,
                        UserId = userId.Value
					};

                await _context.UserAddresses.AddAsync(add);


				// Save changes (like user.save())
				await _context.SaveChangesAsync();

				return Ok(new
				{
					success = true,
					message = "Address Created successfully!"
				});
			}
			catch (Exception ex)
			{
				throw new ErrorHandler(ex.Message, 500);
			}
		}


		[HttpPut("update-user-addresses")]
        [IsAuthenticated]
        public async Task<IActionResult> UpdateUserAddresses([FromBody] UpdateUserAddressDto updateUserAddressDto)
        {
            try
            {
                // User loaded by IsAuthenticatedAttribute (like req.user)
                var userId = (HttpContext.Items["User"] as User)?.Id;

                if (userId == null)
                {
                    throw new ErrorHandler("User not found", 400);
                }

                var user = await _context.Users
                    .Include(u => u.Addresses)  // Include addresses collection
                    .FirstAsync(u => u.Id == userId);

                if (AddressType.Home.ToString() != updateUserAddressDto.AddressType && AddressType.Office.ToString() != updateUserAddressDto.AddressType)
                {
                    throw new ErrorHandler(
                        $"{updateUserAddressDto.AddressType} Please Select AddressType Either Home Or Office ",
                        400
                    );
                }

                // Find existing address by ID
                var existsAddress = user.Addresses
                    .FirstOrDefault(a => a.Id == updateUserAddressDto.Id);


                if (existsAddress != null)
                {
                    // Update existing address (like Object.assign)
                    existsAddress.Address1 = updateUserAddressDto.AddressLine1;
                    existsAddress.Address2 = updateUserAddressDto.AddressLine2;
                    existsAddress.City = updateUserAddressDto.City;
                    existsAddress.State = updateUserAddressDto.State;
                    existsAddress.Country = updateUserAddressDto.Country;
                    existsAddress.ZipCode = updateUserAddressDto.ZipCode;
                    existsAddress.AddressType = updateUserAddressDto.AddressType;
                }
                else
				{ 
                     throw new ErrorHandler(
						"aDDRE NOT FOUND",
						400
					);
				}
            
                
                // Save changes (like user.save())
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
					message = "Address updated successfully!"
				});
            }
            catch (Exception ex)
            {
                throw new   ErrorHandler(ex.Message, 500);
            }
        }
        [HttpDelete("delete-user-address/{id}")]
        [IsAuthenticated]
        public async Task<IActionResult> DeleteUserAddress(int id)
        {
            try
            {
                // User ID from IsAuthenticatedAttribute (like req.user._id)
                var userId = (HttpContext.Items["User"] as User)?.Id;

                if (userId == null)
                {
                    throw new ErrorHandler("User not found", 400);
                }

                // Equivalent to User.updateOne({ _id: userId }, { $pull: { addresses: { _id: addressId } } })
                var address = await _context.UserAddresses
                    .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

                if (address == null)
                {
                    throw new ErrorHandler("Address not found", 404);
                }

                _context.UserAddresses.Remove(address);
                await _context.SaveChangesAsync();

                // Get updated user (like User.findById(userId))
                var user = await _context.Users
                    .Include(u => u.Addresses)
                    .FirstAsync(u => u.Id == userId);

				if (user == null)
					throw new ErrorHandler("User doesn't exist", 400);

				// ✅ USE EXISTING MAPPER
				var response = MapUserToDto(user);

				return Ok(new
                {
                    success = true,
                    user = response
				});
            }
            catch (Exception ex)
            {
                throw new ErrorHandler(ex.Message, 500);
            }
        }
        [HttpPut("update-user-password")]
        [IsAuthenticated]
        public async Task<IActionResult> UpdateUserPassword([FromBody] UpdateUserPasswordDto dto)
        {
            try
            {
                // Get user ID from IsAuthenticatedAttribute (like req.user.id)
                var userId = (HttpContext.Items["User"] as User)?.Id;

                if (userId == null)
                {
                    throw new ErrorHandler("User not found", 400);
                }

                // Find user with password (like User.findById(req.user.id).select("+password"))
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    throw new ErrorHandler("User not found", 400);
                }

                // Verify old password (like user.comparePassword(oldPassword))
                var isPasswordMatched = user.ComparePassword(dto.OldPassword);

                if (!isPasswordMatched)
                {
                    throw new ErrorHandler("Old password is incorrect!", 400);
                }

                // Check if new password matches confirm password
                if (dto.NewPassword != dto.ConfirmPassword)
                {
                    throw new ErrorHandler(
                        "Password doesn't matched with each other!",
                        400
                    );
                }

                // Update password (hash automatically via SetPassword)
                user.SetPassword(dto.NewPassword);

                // Save changes (like user.save())
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Password updated successfully!"
                });
            }
            catch (Exception ex)
            {
                throw new ErrorHandler(ex.Message, 500);
            }
        }
        [HttpGet("user-info/{id}")]
        public async Task<IActionResult> GetUserInfo(int id)
        {
            try
            {
                // Equivalent to User.findById(req.params.id)
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    throw new ErrorHandler("User not found", 404);
                }

                return StatusCode(StatusCodes.Status201Created, new
                {
                    success = true,
                    user
                });
            }
            catch (Exception ex)
            {
                throw new ErrorHandler(ex.Message, 500);
            }
        }
        [HttpGet("admin-all-users")]
        [IsAuthenticated]  // your existing attribute
        [IsAdmin("Admin")]  // existing attribute
        public async Task<IActionResult> AdminAllUsers()
        {
            try
            {
                // Equivalent to User.find().sort({ createdAt: -1 })
                var users = await _context.Users
                    .OrderByDescending(u => u.CreatedAt)
                    .ToListAsync();

                return StatusCode(StatusCodes.Status201Created, new
                {
                    success = true,
                    users
                });
            }
            catch (Exception ex)
            {
                throw new ErrorHandler(ex.Message, 500);
            }
        }

        [HttpDelete("delete-user/{id}")]
        [IsAuthenticated]  // your existing attribute
        [IsAdmin("Admin")]  // existing attribute
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                // Equivalent to User.findById(req.params.id)
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    throw new ErrorHandler(
                        "User is not available with this id",
                        400
                    );
                }

                /*
                // Uncomment if avatar deletion needed (like cloudinary.v2.uploader.destroy)
                if (!string.IsNullOrEmpty(user.AvatarPublicId))
                {
                    await _cloudinary.DeleteImageAsync(user.AvatarPublicId);
                }
                */

                // Equivalent to User.findByIdAndDelete(req.params.id)
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status201Created, new
                {
                    success = true,
                    message = "User deleted successfully!"
                });
            }
            catch (Exception ex)
            {
                throw new ErrorHandler(ex.Message, 500);
            }
        }

        [Authorize]
        [HttpDelete("delete-avatar")]
        public async Task<IActionResult> DeleteAvatar()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var user = await _context.Users
                    .Include(u => u.Avatar)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null || user.Avatar == null)
                {
                    return Ok(new { success = true, message = "No avatar to delete" });
                }

                // Delete from Cloudinary
                if (!string.IsNullOrEmpty(user.Avatar.PublicId))
                {
                    try
                    {
                        await _cloudinaryService.DeleteImageAsync(user.Avatar.PublicId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to delete avatar from Cloudinary");
                    }
                }

                // Remove from database
                _context.Avatars.Remove(user.Avatar);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Avatar deleted successfully" });
            }
            catch (ErrorHandler)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting avatar");
                throw new ErrorHandler($"Failed to delete avatar: {ex.Message}", 500);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost("upload-user-avatar/{userId}")]
        public async Task<IActionResult> UploadUserAvatar(int userId, [FromBody] AvatarUploadDto avatarDto)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Avatar)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    throw new ErrorHandler("User not found", 404);
                }

                // Delete old avatar from Cloudinary if exists
                if (user.Avatar != null && !string.IsNullOrEmpty(user.Avatar.PublicId))
                {
                    try
                    {
                        await _cloudinaryService.DeleteImageAsync(user.Avatar.PublicId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to delete old avatar from Cloudinary");
                    }

                    _context.Avatars.Remove(user.Avatar);
                }

                // Upload new avatar to Cloudinary
                var uploadResult = await _cloudinaryService.UploadImageAsync(
                    avatarDto.ImageData, "avatars");

                // Create new avatar
                user.Avatar = new Avatar
                {
                    PublicId = uploadResult.PublicId,
                    Url = uploadResult.SecureUrl.ToString()
                };

                await _context.SaveChangesAsync();

                return Ok(new CloudinaryUploadResponse
                {
                    PublicId = uploadResult.PublicId,
                    Url = uploadResult.Url.ToString(),
                    SecureUrl = uploadResult.SecureUrl.ToString()
                });
            }
            catch (ErrorHandler)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading user avatar");
                throw new ErrorHandler($"Failed to upload avatar: {ex.Message}", 500);
            }
        }

        // Helper methods remain the same as before...
        private string GenerateActivationToken(string name,string email,string password,CloudinaryUploadResponse? avatar)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Activation:SecretKey"]);

            var claims = new List<Claim>
                            {
                                new Claim("name", name),
                                new Claim("email", email),
                                new Claim("password", password)
                            };

            if (avatar != null)
            {
                claims.Add(new Claim(
                    "avatar",
                    Newtonsoft.Json.JsonConvert.SerializeObject(avatar)
                ));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        private User? ValidateActivationToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Activation:SecretKey"]);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                // Extract claims and reconstruct user data
                var jwtToken = (JwtSecurityToken)validatedToken;

                // Deserialize CloudinaryUploadResponse and map to Avatar
                CloudinaryUploadResponse? avatarResponse = null;
                var avatarClaim = principal.FindFirst("avatar")?.Value;
                Avatar? avatar = null;
                if (!string.IsNullOrEmpty(avatarClaim))
                {
                    avatarResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<CloudinaryUploadResponse>(avatarClaim);
                    if (avatarResponse != null)
                    {
                        avatar = new Avatar
                        {
                            PublicId = avatarResponse.PublicId,
                            Url = avatarResponse.SecureUrl ?? avatarResponse.Url
                        };
                    }
                }

                // Replace this line in ValidateActivationToken:
               var  Email = principal.FindFirst(ClaimTypes.Email);//principal.FindFirst("email")?.Value,

                // With this corrected line:
                //Email = principal.FindFirst("email")?.Value, //or we use in principle   MapInboundClaims = false 
                return new User
                {
                    Name = principal.FindFirst("name")?.Value,
                    Email = Email.Value,//principal.FindFirst("email")?.Value,
                    Password = principal.FindFirst("password")?.Value,
                    Avatar = avatar
                };
            }
            catch
            {
                return null;
            }
        }

        private UserResponseDto MapUserToDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                Avatar = user.Avatar != null ? new AvatarDto
                {
                    PublicId = user.Avatar.PublicId,
                    Url = user.Avatar.Url
                } : null,
                CreatedAt = user.CreatedAt,
                Addresses = user.Addresses?.Select(a => new UserAddressDto
                {
                    Id = a.Id,
                    Country = a.Country,
                    State = a.State,
                    City = a.City,
                    Address1 = a.Address1,
                    Address2 = a.Address2,
                    ZipCode = a.ZipCode,
                    AddressType = a.AddressType
                }).ToList() ?? new List<UserAddressDto>()
            };
        }
        private User MapObjectToUserEntity(object userObject)
        {
            // Cast the object to dynamic to access properties
            dynamic obj = userObject;

            var user = new User
            {
                Id = obj.Id ?? 0, // Assuming 0 for new users
                Name = obj.Name,
                Email = obj.Email,
                Password = obj.Password, // Note: You should hash this password
                PhoneNumber = obj.PhoneNumber,
                Role = obj.Role ?? "user", // Default to "user"
                CreatedAt = obj.CreatedAt ?? DateTime.UtcNow
            };

            // Map Avatar if it exists
            if (obj.Avatar != null)
            {
                user.Avatar = new Avatar
                {
                    PublicId = obj.Avatar.PublicId,
                    Url = obj.Avatar.Url
                };
            }

            // Map Addresses if they exist
            if (obj.Addresses != null)
            {
                user.Addresses = new List<UserAddress>();
                foreach (var address in obj.Addresses)
                {
                    user.Addresses.Add(new UserAddress
                    {
                        Country = address.Country,
                        City = address.City,
                        Address1 = address.Address1,
                        Address2 = address.Address2,
                        ZipCode = address.ZipCode,
                        AddressType = address.AddressType
                    });
                }
            }

            return user;
        }
    }
}
