using Microsoft.AspNetCore.Identity;
using System;
using wedding_planer_ad.Models;
using wedding_planer_ad.Models.DTO;

namespace wedding_planer_ad.Business.Interfaces
{
    public interface IUserService
    {
        Task<List<ApplicationUser>> GetUsersInRoleAsync(string roleName);
        Task<ApplicationUser> GetUserByIdAsync(string id);
        Task UpdateUserAsync(ApplicationUser user);
        Task DeleteUserAsync(string id);

        Task UpdateUserFromDTOAsync(AdminUserDto admin);

        Task<IdentityResult> CreatePlannerAsync(NewPlannerDto dto);

        Task<IdentityResult> CreateUserWithRoleAsync(AdminUserDto dto, string role);

        Task<IdentityResult> CreateCoupleAsync(NewCoupleDto dto);
        Task<IdentityResult> CreateVendorAsync(NewVendorDto dto);

    }
}
