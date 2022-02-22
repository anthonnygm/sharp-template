using Appel.SharpTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Appel.SharpTemplate.Infrastructure.Data
{
    public class SharpTemplateContext : DbContext
    {
        public SharpTemplateContext(DbContextOptions<SharpTemplateContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SharpTemplateContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
