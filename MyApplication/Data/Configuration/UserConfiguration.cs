using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyApplication.Data.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<ApiUser>
{
    public void Configure(EntityTypeBuilder<ApiUser> builder)
    {
        builder.HasIndex(u => u.Email).IsUnique();
    }
}