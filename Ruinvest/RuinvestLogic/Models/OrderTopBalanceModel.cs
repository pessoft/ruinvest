using RuinvestLogic.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RuinvestLogic.Models
{
    public class OrderTopBalanceModel
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public int UserId { get; set; }
        public double Amount { get; set; }
        public StatusOrder Status { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DatePayment { get; set; }
    }
}