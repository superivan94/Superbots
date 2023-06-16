using Microsoft.EntityFrameworkCore;

namespace Superbots.App.Common.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Settings> Settings { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }
    }
}
