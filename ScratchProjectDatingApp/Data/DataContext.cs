using Microsoft.EntityFrameworkCore;
using ScratchProjectDatingApp.Entity;

namespace ScratchProjectDatingApp.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<AppUser> Users { get; set; }
    }
}
