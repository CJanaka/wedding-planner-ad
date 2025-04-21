using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using wedding_planer_ad.Business.Interfaces;
using wedding_planer_ad.Models;
using wedding_planer_ad.Models.DTO;

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

        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {

            return await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task UpdateUserAsync(ApplicationUser updatedUser)
        {
            var user = await _userManager.FindByIdAsync(updatedUser.Id);
            if (user != null)
            {
                user.FirstName = updatedUser.FirstName;
                user.LastName = updatedUser.LastName;
                user.PhoneNumber = updatedUser.PhoneNumber;
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        }

        public async Task UpdateUserFromDTOAsync(AdminUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.Id);
            if (user == null) throw new Exception("User not found");

            user.Email = dto.Email;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.PhoneNumber = dto.PhoneNumber;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                throw new Exception("Failed to update user");

            // Update password if provided
            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passResult = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);
                if (!passResult.Succeeded)
                {
                    var errors = string.Join(", ", passResult.Errors.Select(e => e.Description));
                    throw new Exception($"Password reset failed: {errors}");
                }
            }
        }

        // Services/UserService.cs
        public async Task<IdentityResult> CreatePlannerAsync(NewPlannerDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Planner");
            }

            return result;
        }

        public async Task<IdentityResult> CreateCoupleAsync(NewCoupleDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Couple");
            }

            return result;
        }

        public async Task<IdentityResult> CreateVendorAsync(NewVendorDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Vendor");
            }

            return result;
        }


        public async Task<IdentityResult> CreateUserWithRoleAsync(AdminUserDto dto, string role)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, dto.NewPassword);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
            return result;
        }

        public async Task<ApplicationUser> GetByEmail(string mail)
        {
            return await _userManager.Users.
                FirstOrDefaultAsync(u => u.Email == mail);

        }
    }

}
