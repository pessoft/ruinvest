using Ruinvest.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ruinvest.Models
{
    public class CreateDepositModel
    {
        public double DepositAmount { get; set; }
        public Rates Rate { get; set; }
    }
}