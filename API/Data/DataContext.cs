using API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }

        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Like>()
                .HasKey(l => new { l.TargetUserId, l.SourceUserId });

            modelBuilder.Entity<Like>()
                .HasOne(l => l.SourceUser)
                .WithMany(u => u.LikedUsers)
                .HasForeignKey(l => l.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Like>()
                .HasOne(l => l.TargetUser)
                .WithMany(u => u.LikedByUsers)
                .HasForeignKey(u => u.TargetUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}