using Microsoft.EntityFrameworkCore;

namespace TaskManagementSystem.Models
{
    public class NodeContext : DbContext
    {
        public DbSet<Node> Nodes { get; set; }
        public NodeContext(DbContextOptions<NodeContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
