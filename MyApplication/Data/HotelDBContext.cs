using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyApplication.Data.Configuration;

namespace MyApplication.Data;

public class HotelDBContext : IdentityDbContext<ApiUser>
{
    public HotelDBContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<ApiUser> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new RolesConfiguration());
        builder.ApplyConfiguration(new UserConfiguration());
        builder.ApplyConfiguration(new HotelConfiguration());
        builder.ApplyConfiguration(new CountryConfiguration());
    }
}