using Ruinvest.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ruinvest.Models
{
    public class Deposit
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Percent { get; set; }
        public double StartAmount { get; set; }
        public double InterimAmount { get; set; }
        public double EndAmount { get; set; }
        public StatusDeposit Status { get; set; }
    }
}