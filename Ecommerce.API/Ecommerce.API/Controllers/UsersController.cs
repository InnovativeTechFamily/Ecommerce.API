using Ecommerce.API.DTOs.User;
using Ecommerce.API.Exceptions;
using Ecommerce.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;

        public UsersController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/users/create-user
        [HttpPost("create-user")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userDto)
        {
            try
            {
                var user = await _authService.CreateUserAsync(userDto);

                return CreatedAtAction(nameof(CreateUser), new { id = user.Id }, new
                {
                    success = true,
                    message = $"Please check your email: {user.Email} to activate your account!",
                    user
                });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while creating the user",
                    error = ex.Message
                });
            }
        }
    }
}
