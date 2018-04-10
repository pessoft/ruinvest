using Ruinvest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ruinvest.Logic
{
    public class DataWrapper
    {
        public static double AvailableMoneyByUserId(int userId)
        {
            double money = 0;
            try
            {
                var cash = new UserCashAccountModel()
                {
                    UserId = userId,
                    Amount = 0
                };

                using (UserCashAccountContext db = new UserCashAccountContext())
                {
                    var userCash = db.UserCashAccounts.FirstOrDefault(p => p.UserId == userId);

                    money = userCash == null ? 0 : userCash.Amount;
                }
            }
            catch (Exception e)
            {
                money = 0;
            }

            return money;
        }

        public static bool TakeMoneyByUserId(int userId, double amount)
        {
            var success = false;
            try
            {
                success = ChangeMoneyByUserId(userId, amount, (a, b) => a - b);
            }
            catch (Exception e)
            {
                success = false;
            }

            return success;
        }

        public static bool AddMoneyByUserId(int userId, double amount)
        {
            var success = false;
            try
            {
                success = ChangeMoneyByUserId(userId, amount, (a, b) => a + b);
            }
            catch (Exception e)
            {
                success = false;
            }

            return success;
        }

        private static bool ChangeMoneyByUserId(int userId, double amount, Func<double, double, double> operation)
        {
            var success = false;
            try
            {
                using (UserCashAccountContext db = new UserCashAccountContext())
                {
                    var userCash = db.UserCashAccounts.FirstOrDefault(p => p.UserId == userId);

                    if (userCash != null)
                    {
                        userCash.Amount = operation(userCash.Amount, amount);
                        success = true;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return success;
        }

        public static bool AddCashUser(int userId)
        {
            var success = false;
            try
            {
                var cash = new UserCashAccountModel()
                {
                    UserId = userId,
                    Amount = 0
                };

                using (UserCashAccountContext db = new UserCashAccountContext())
                {
                    db.UserCashAccounts.Add(cash);
                    db.SaveChanges();
                    success = true;
                }
            }
            catch (Exception e)
            {
                success = false;
            }

            return success;
        }

        public static bool AddNewDeposit(Deposit deposit)
        {
            var success = false;
            try
            {
                using (DepositContext db = new DepositContext())
                {
                    db.Deposits.Add(deposit);
                    db.SaveChanges();
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