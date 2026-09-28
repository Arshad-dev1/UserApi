using Microsoft.EntityFrameworkCore;
using UserApi.DB.Models;

namespace UserApi.DB
{
    public class UserDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                e.Property(u => u.Name).IsRequired().HasMaxLength(100);
                e.Property(u => u.City).IsRequired().HasMaxLength(50);
                e.Property(u => u.State).IsRequired().HasMaxLength(50);
                e.Property(u => u.Pincode).IsRequired().HasMaxLength(8);
            });
        }

    }
}
