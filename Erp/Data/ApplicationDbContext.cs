using Erp.Mapps;
using Erp.Migrations;
using Erp.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Security;

namespace Erp.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options) { }
        
        public DbSet<ApplicationUser> Users { get; set; }

        public DbSet<UserImage> UserImages { get; set; }

        public DbSet<UserRolls> UserRolls { get; set; }
        public DbSet<SystemRolls> SystemRolls { get; set; }


        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<UserPermissions> UserPermissions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UserMappps());
            modelBuilder.ApplyConfiguration(new UserPermissionMapps());

            SeedPermissions(modelBuilder);
        }


        private static void SeedPermissions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permissions>().HasData(
                new Permissions { Id = 1, Name = "CanEdit", Description = "Edit permission" },
                new Permissions { Id = 2, Name = "CanDelete", Description = "Delete permission" },
                new Permissions { Id = 3, Name = "CanUpdate", Description = "Update permission" }
            );
        }
    }
}
