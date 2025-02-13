using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using GetItDone.models;
using Microsoft.EntityFrameworkCore;
using GetItDone.models;

namespace GetItDone.Data
{
    public class GetItDoneDbContext : IdentityDbContext<User>
    {
        public GetItDoneDbContext(DbContextOptions<GetItDoneDbContext> options) : base(options) 
        { 
        }

        public DbSet<User> Users { get; set; }
        public DbSet<models.Task> Tasks { get; set; }
        public DbSet<UserTask> UserTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().Property(u => u.Initials).HasMaxLength(5);

            builder.HasDefaultSchema("identity");
        }
    }
}
