using Microsoft.EntityFrameworkCore;

namespace TreeViewCRUDSample.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TreeNodeModel> TreeNodes { get; set; }
        public DbSet<UserPermisson> UserPermissons { get; set; }
    }
}
