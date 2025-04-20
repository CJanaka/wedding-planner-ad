using System.Numerics;
using Microsoft.EntityFrameworkCore;
using wedding_planer_ad.Business.Interfaces;
using wedding_planer_ad.Data;
using wedding_planer_ad.Exceptions;
using wedding_planer_ad.Models;

namespace wedding_planer_ad.Business.Services
{
    public class GuestService : IGuestService
    {

        private readonly ApplicationDbContext _context;

        public GuestService(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task<Guest> CreateAsync(Guest guest)
        {
            _context.Guest.Add(guest);
            await _context.SaveChangesAsync();
            return guest;
        }

        public async Task<IEnumerable<Guest>>  GetGustsByCoupleId(int id)
        {
            return await _context.Guest
                .Where(g => g.CoupleId == id && !g.IsDeleted)
                .ToListAsync();
        }

        public async Task<Guest> UpdateAsync(Guest guest)
        {
            var existing = await _context.Guest.FindAsync(guest.Id);

            if (existing == null || existing.IsDeleted)
                throw new ResourceNotFoundException($"guest with id {guest.Id} not found.");

            existing.MealPreference = guest.MealPreference;
            existing.IsDeleted = guest.IsDeleted;
            existing.LastName = guest.LastName;
            existing.FirstName = guest.FirstName;
            existing.Email = guest.Email;
            existing.RSVPStatus = guest.RSVPStatus;
            existing.SeatingArrangement = guest.SeatingArrangement;


            await _context.SaveChangesAsync();
            return existing;
        }
    }
}
