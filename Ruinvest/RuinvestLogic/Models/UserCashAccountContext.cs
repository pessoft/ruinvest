using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RuinvestLogic.Models
{
    public class UserCashAccountContext : DbContext
    {
        public UserCashAccountContext()
            : base(DBStringConnections.RuinvestConnection)
        { }

        public DbSet<UserCashAccountModel> UserCashAccounts { get; set; }
    }
}