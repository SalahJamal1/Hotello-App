using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyApplication.Data.Configuration;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasData
        (
            new Country
            {
                Id = 1,
                Name = "United States",
                ShortName = "USA"
            },
            new Country
            {
                Id = 2,
                Name = "Jordan",
                ShortName = "JO"
            }, new Country
            {
                Id = 3,
                Name = "Saudi Arabia",
                ShortName = "KSA"
            }
        );
    }
}