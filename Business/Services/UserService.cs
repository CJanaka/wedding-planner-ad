using Microsoft.AspNetCore.Identity;
using wedding_planer_ad.Business.Interfaces;
using wedding_planer_ad.Models;

namespace wedding_planer_ad.Business.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<ApplicationUser>> GetUsersInRoleAsync(string roleName)
        {
            var users = await _userManager.GetUsersInRoleAsync(roleName);
            return users.ToList();
        }
    }
}
