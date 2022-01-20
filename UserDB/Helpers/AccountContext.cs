





using Microsoft.EntityFrameworkCore;

namespace UserDB.Models
{
    public class AccountContext : DbContext
    {
        public AccountContext(DbContextOptions<AccountContext> options)
        : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
       
    }
}
