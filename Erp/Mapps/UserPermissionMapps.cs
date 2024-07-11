
using Erp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Erp.Mapps
{
    

    public class UserPermissionMapps : IEntityTypeConfiguration<UserPermissions>
    {
        public void Configure(EntityTypeBuilder<UserPermissions> builder)
        {
            // Configure the UserPermissions table
            builder.HasKey(up => new { up.UserId, up.PermissionId });

            builder.HasOne(up => up.User)
                   .WithMany(u => u.UserPermissions)
                   .HasForeignKey(up => up.UserId);

            builder.HasOne(up => up.Permissions)
                   .WithMany(p => p.UserPermissions)
                   .HasForeignKey(up => up.PermissionId);
        }

        
    }

}
