using Erp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Erp.Mapps
{
    public class UserMappps : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasOne(e => e.UserImage)
               .WithOne(e => e.ApplicationUser)
               .HasForeignKey<UserImage>(e => e.UserId);

        }
    }
}
