using wedding_planer_ad.Models;

namespace wedding_planer_ad.Business.Interfaces
{
    public interface IUserService
    {
        Task<List<ApplicationUser>> GetUsersInRoleAsync(string roleName);

    }
}
