using OrderManagementSystem.Domain.Authorization;

namespace OrderManagementSystem.Authentication
{
    public interface IJwtAuthentication
    {
        public string Authenticate(ApplicationUser user);
    }
}
