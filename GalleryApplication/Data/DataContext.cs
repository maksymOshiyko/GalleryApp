using GalleryApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GalleryApplication.Data
{
    public class DataContext : IdentityDbContext<AppUser, IdentityRole<int>, int,
        IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext()
        {
            Database.EnsureCreated();
        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }
        
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Follow> Follows { get; set; }
        public virtual DbSet<Like> Likes { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-1S8GL46; Database=GalleryDb; Trusted_Connection=True;",
                    options =>
                    {
                        
                    });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Comments)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasForeignKey(d => d.PostId);

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.SenderId);
            });
            
            modelBuilder.Entity<Follow>(entity =>
            {
                entity.HasKey(e => new { e.SourceUserId, e.FollowedUserId });

                entity.HasOne(d => d.FollowedUser)
                    .WithMany(p => p.FollowedByUsers)
                    .HasForeignKey(d => d.FollowedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.SourceUser)
                    .WithMany(p => p.FollowedUsers)
                    .HasForeignKey(d => d.SourceUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });
            
            modelBuilder.Entity<Like>(entity =>
            {
                entity.HasOne(d => d.LikeSender)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(d => d.LikeSenderId);

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(d => d.PostId);
            });
            
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.UserId);
            });
            
            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.CountryId);
            });
        }
    }
}