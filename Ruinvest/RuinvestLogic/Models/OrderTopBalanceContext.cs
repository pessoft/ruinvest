using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RuinvestLogic.Models
{
    public class OrderTopBalanceContext : DbContext
    {
        public OrderTopBalanceContext()
        : base(DBStringConnections.RuinvestConnection)
        { }
        public DbSet<OrderTopBalanceModel> Orders { get; set; }
    }
}