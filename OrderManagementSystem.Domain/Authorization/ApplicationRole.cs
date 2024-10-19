using Microsoft.AspNetCore.Identity;

namespace OrderManagementSystem.Domain.Authorization
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() { }
        public ApplicationRole(string roleName) : base(roleName) { }
    }
}
