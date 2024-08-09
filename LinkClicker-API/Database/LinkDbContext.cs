using LinkClicker_API.Models.LinkClickerDatabase;
using Microsoft.EntityFrameworkCore;

namespace LinkClicker_API.Database
{
    public class LinkDbContext : DbContext
    {
        public LinkDbContext(DbContextOptions<LinkDbContext> options)
           : base(options)
        {
        }

        public DbSet<Link> Links { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the Link entity
            modelBuilder.Entity<Link>()
                .HasKey(l => l.Id); 

            modelBuilder.Entity<Link>()
                .Property(l => l.Url)
                .IsRequired()
                .HasMaxLength(2048); //url length limit

            modelBuilder.Entity<Link>()
                .Property(l => l.Username)
                .IsRequired()
                .HasMaxLength(50); 

            modelBuilder.Entity<Link>()
                .Property(l => l.ExpiryTime)
                .IsRequired(false);

            modelBuilder.Entity<Link>()
                .Property(l => l.MaxClicks)
                .IsRequired(); 

            modelBuilder.Entity<Link>()
                .Property(l => l.ClickCount)
                .IsRequired(); 

            modelBuilder.Entity<Link>()
                .Property(l => l.Status)
                .IsRequired();
        }
    }
}