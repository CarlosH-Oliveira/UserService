using Microsoft.EntityFrameworkCore;
using UserService.Models.User;

namespace UserService.Data
{
    public class UserContext:DbContext
    {
        public UserContext(DbContextOptions<UserContext> options):base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(user => user.Email)
                .IsUnique();
        }

        public DbSet<User> Users { get; set; }
        
    }
}
