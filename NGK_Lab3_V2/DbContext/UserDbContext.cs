using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NGK_Lab3_V2.Models;

namespace NGK_Lab3_V2.DbContext
{
    public class UserDbContext : IdentityDbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) 
            : base(options) 
        { }

        public DbSet<User> User { get; set; }
    }
}
