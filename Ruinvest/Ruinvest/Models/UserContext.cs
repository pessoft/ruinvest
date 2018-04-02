using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Ruinvest.Models
{
    public class UserContext : DbContext
    {
        public UserContext() 
            : base("RuinvestConnection")
        { }

        public DbSet<User> Users { get; set; }
    }
}