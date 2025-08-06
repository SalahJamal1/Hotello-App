using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyApplication.Data.Configuration;

public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.HasData
        (
            new Hotel
            {
                Id = 1,
                Name = "John Doe",
                Address = "123 Main St",
                Rating = 5,
                CountryId = 1
            },
            new Hotel
            {
                Id = 2,
                Name = "Amman Doe",
                Address = "17 branch St",
                Rating = 4.3f,
                CountryId = 2
            }, new Hotel
            {
                Id = 3,
                Name = "Jeddah Hotel",
                Address = "17 Makkah St",
                Rating = 4.9f,
                CountryId = 3
            }
        );
    }
}