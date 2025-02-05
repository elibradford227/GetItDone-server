using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using GetItDone.models;
using Microsoft.EntityFrameworkCore;

namespace GetItDone.Data
{
    public class GetItDoneDbContext : IdentityDbContext<User>
    {
        public GetItDoneDbContext(DbContextOptions<GetItDoneDbContext> options) : base(options) 
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().Property(u => u.Initials).HasMaxLength(5);

            builder.HasDefaultSchema("identity");
        }
    }
}
