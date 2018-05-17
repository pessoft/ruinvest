using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RuinvestLogic.Models
{
    public class UserContext : DbContext
    {
        public UserContext() 
            : base(DBStringConnections.RuinvestConnection)
        { }

        public DbSet<User> Users { get; set; }
    }
}