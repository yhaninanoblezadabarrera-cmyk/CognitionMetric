using CognitoMetric.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CognitoMetric.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<UsageRecord> UsageRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UsageRecord>().HasKey(u => u.Id);
            builder.Entity<UsageRecord>()
                   .HasOne(r => r.User)
                   .WithMany()
                   .HasForeignKey(r => r.UserId);
        }
    }
}