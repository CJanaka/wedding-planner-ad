using Microsoft.EntityFrameworkCore;
using wedding_planer_ad.Business.Interfaces;
using wedding_planer_ad.Data;
using wedding_planer_ad.Models;

namespace wedding_planer_ad.Business.Services
{
    public class VendorServices : IVendorServices
    {
        private readonly ApplicationDbContext _context;

        public VendorServices(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task<Vendor> GetById(int id)
        {
            return await _context.Vendor.FirstOrDefaultAsync(t => t.Id == id);

        }
    }
}
