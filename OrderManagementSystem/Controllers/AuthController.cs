using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Application.Users.Dto;
using OrderManagementSystem.Authentication;
using OrderManagementSystem.Domain.Authorization;

namespace OrderManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtAuthentication _jwtAuthentication;

        public AuthController(UserManager<ApplicationUser> userManager, IJwtAuthentication jwtAuthentication)
        {
            _userManager = userManager;
            _jwtAuthentication = jwtAuthentication;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var token = _jwtAuthentication.Authenticate(user);
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }

        [HttpPost("RegisterAdmin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                return Ok(new { message = "Admin user created successfully!" });
            }

            return BadRequest(result.Errors);
        }
    }
}
