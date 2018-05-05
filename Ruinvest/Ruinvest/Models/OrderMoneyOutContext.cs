using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Ruinvest.Models
{
    public class OrderMoneyOutContext: DbContext
    {
        public OrderMoneyOutContext()
        : base(DBStringConnections.RuinvestConnection)
        { }
        public DbSet<OrderMoneyOut> Orders { get; set; }
    }
}