using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Services
{
    public class ProjectDBContext : DbContext
    {
        public ProjectDBContext(DbContextOptions<ProjectDBContext> option)
            : base(option) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Purchase> PurchaseHistory { get; set; }
        public DbSet<Sale> SalesHistory { get; set; }
    }
}