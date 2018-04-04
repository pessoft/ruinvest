using Ruinvest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ruinvest.Logic
{
    public class DataWrapper
    {
        public static bool AddNewDeposit(Deposit deposit)
        {
            var success = false;
            try
            {
                using (DepositContext db = new DepositContext())
                {
                    db.Deposits.Add(deposit);
                    success = true;
                }
            }
            catch (Exception e)
            {
                success = false;
            }
          
            return success;
        }
    }
}