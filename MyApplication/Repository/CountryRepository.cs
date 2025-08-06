using Microsoft.EntityFrameworkCore;
using MyApplication.Contracts;
using MyApplication.Data;

namespace MyApplication.Repository;

public class CountryRepository : GenericRepository<Country>, ICountryRepository
{
    private readonly HotelDBContext _context;

    public CountryRepository(HotelDBContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Country> GetDetails(int id)
    {
        var county = await _context.Countries.Include(h
            => h.Hotels).FirstOrDefaultAsync(h
            => h.Id == id);
        return county;
    }
}