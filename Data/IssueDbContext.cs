using IssueCRUD.Models;
using Microsoft.EntityFrameworkCore;

namespace IssueCRUD.Data
{
    public class IssueDbContext : DbContext
    {
        public IssueDbContext(DbContextOptions<IssueDbContext> options) : base(options)
        {

        }

        public DbSet<Issue> Issues { get; set; } //the table to create based on our model Issue
    }
}
