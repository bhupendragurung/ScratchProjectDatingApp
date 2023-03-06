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
        public DbSet<UserLike> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserLike>()
                .HasKey(k => new { k.SourceUserId, k.TargetUserId });
            builder.Entity<UserLike>()
                .HasOne(s=>s.SourceUser)
                .WithMany(l=>l.LikedUsers)
                .HasForeignKey(l=>l.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserLike>()
               .HasOne(s => s.TargetUser)
               .WithMany(l => l.LikedByUsers)
               .HasForeignKey(l => l.TargetUserId)
               .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
