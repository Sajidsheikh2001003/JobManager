using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Jobs> Jobs {get;set;}
    }

    public class Jobs
    {
        public int Id { get; set; }
        public string  FilePath { get; set; }
        public string JobName { get; set; }
        public byte IsCompleted { get; set; }
        public DateTime CompletedOn { get; set; }
        public string? CompletedBy { get; set; }

        public string? UserId { get; set; }
    }





}