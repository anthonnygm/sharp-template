using Microsoft.EntityFrameworkCore;

namespace Appel.SharpTemplate.Models
{
    public class SharpTemplateContext : DbContext
    {
        public SharpTemplateContext(DbContextOptions<SharpTemplateContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
        }
    }
}
