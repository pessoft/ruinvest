using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Ruinvest.Models
{
    public class DepositContext :DbContext
    {
        public DepositContext()
         : base(DBStringConnections.RuinvestConnection)
        { }

        public DbSet<Deposit> Deposits { get; set; }
    }
}