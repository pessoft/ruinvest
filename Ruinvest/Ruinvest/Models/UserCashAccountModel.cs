using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ruinvest.Models
{
    public class UserCashAccountModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public double Amount { get; set; }
    }
}