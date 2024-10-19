using OrderManagementSystem.Domain.Common;

using System.ComponentModel.DataAnnotations;

namespace OrderManagementSystem.Application.Users.Dto
{

    public class RegisterModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; } = true;
    }
}
