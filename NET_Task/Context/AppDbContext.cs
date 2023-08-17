using Microsoft.EntityFrameworkCore;
using NET_Task.Models;

namespace NET_Task.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { 
        }
        
        public DbSet<User> Users { get; set; }
    }
}

