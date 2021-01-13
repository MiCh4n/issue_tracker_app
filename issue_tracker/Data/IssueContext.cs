using issue_tracker.Models;
using Microsoft.EntityFrameworkCore;
namespace issue_tracker.Data
{
    public class IssueContext : DbContext
    {
        public IssueContext(DbContextOptions<IssueContext> options) : base(options)
        {
        }

        public DbSet<Issue> Issues { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Issue>().ToTable("Issue");
        }
    }
}