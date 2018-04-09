﻿using Ruinvest.Models;
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

        public static List<Deposit> GetDepostByUserId(int userId)
        {
            List<Deposit> deposits = new List<Deposit>();
            try
            {
                using (DepositContext db = new DepositContext())
                {
                    deposits = db.Deposits?.Where(p => p.UserId == userId).ToList();
                }
            }
            catch (Exception e)
            {
                deposits = new List<Deposit>();
            }

            return deposits;
        }
    }
}