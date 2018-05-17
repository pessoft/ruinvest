using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RuinvestLogic.Models
{
    public class DepositContext :DbContext
    {
        public DepositContext()
         : base(DBStringConnections.RuinvestConnection)
        { }

        public DbSet<Deposit> Deposits { get; set; }
    }
}