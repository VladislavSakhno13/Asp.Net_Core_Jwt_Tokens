using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Token.Entities;
using Microsoft.EntityFrameworkCore;

namespace Token.Helpers
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DataContext(DbContextOptions<DataContext> options) 
            : base(options) 
        {
            
                Database.EnsureCreated();
            
        }
    }
}
