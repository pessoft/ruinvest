using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuinvestLogic.Models
{
    public class NotificationMessageContext : DbContext
    {
        public NotificationMessageContext()
         : base(DBStringConnections.RuinvestConnection)
        { }

        public DbSet<NotificationMessageModel> Notification { get; set; }
    }
}
