﻿using Ruinvest.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Ruinvest.Logic
{
    public class DataWrapper
    {
        public static List<OrderMoneyOut> GetMoneyOrdersByUserId(int userId)
        {
            var resultData = new List<OrderMoneyOut>();
            try
            {
                using (var db = new OrderMoneyOutContext())
                {
                    resultData = db.Orders.Where(p => p.UserId == userId).ToList();
                }
            }
            catch (Exception e)
            { }

            return resultData;
        }

        public static bool HasNonProcessedMoneyOut(int userId)
        {
            bool success = false;
            try
            {
                using (var db = new OrderMoneyOutContext())
                {
                    var countOrder = db.Orders.Where(p => p.UserId == userId && p.Status == StatusOrder.InProgress).Count();
                    success = countOrder > 0;
                }
            }
            catch (Exception e)
            {
                success = false;
            }

            return success;
        }

        public static List<OrderMoneyOut> GetMoneyOrdersByDate(DateTime date)
        {
            var  resultData = new List<OrderMoneyOut>();
            try
            {
                using (var db = new OrderMoneyOutContext())
                {
                    resultData = db.Orders.Where(p => p.OrderDate.Year == date.Year 
                    && p.OrderDate.Month == date.Month
                    && p.OrderDate.Day == date.Day
                    && p.Status == StatusOrder.InProgress).ToList();
                }
            }
            catch (Exception e)
            {}

            return resultData;
        }

        public static List<OrderMoneyOut> GetMoneyOrdersUpToDate(DateTime date)
        {
            var resultData = new List<OrderMoneyOut>();
            try
            {
                using (var db = new OrderMoneyOutContext())
                {
                    resultData = db.Orders.Where(p => p.OrderDate < date
                        && p.Status == StatusOrder.InProgress).ToList();
                }
            }
            catch (Exception e)
            { }

            return resultData;
        }


        public static bool MarkOrderMoneyOutFinished(string orderId)
        {
            bool success = false;
            try
            {
                using (var db = new OrderMoneyOutContext())
                {
                    var order = db.Orders.FirstOrDefault(p => p.OrderId == orderId);

                    if (order != null)
                    {
                        order.Status = StatusOrder.Finished;
                        order.ExecutionDate = DateTime.Now;
                        db.SaveChanges();
                        success = true;
                    }
                }
            }
            catch (Exception e)
            {
                success = false;
            }

            return success;
        }

        public static bool AddNewOrderMoneyOut(OrderMoneyOut order)
        {
            var success = false;
            try
            {
                using (var db = new OrderMoneyOutContext())
                {
                    db.Orders.Add(order);
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

        public static bool AddNewOrderTopBalance(OrderTopBalanceModel order)
        {
            var success = false;
            try
            {
                using (var db = new OrderTopBalanceContext())
                {
                    db.Orders.Add(order);
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

        public static OrderTopBalanceModel GetOrderTopBalanceByOrderId(string orderId)
        {
            OrderTopBalanceModel order;
            try
            {
                using (var db = new OrderTopBalanceContext())
                {
                    order = db.Orders.FirstOrDefault(p => p.OrderId == orderId);
                }
            }
            catch (Exception e)
            {
                order = null;
            }

            return order;
        }

        public static bool MarkOrderTopBalanceFinished(string orderId)
        {
            bool success = false;
            try
            {
                using (var db = new OrderTopBalanceContext())
                {
                    var order = db.Orders.FirstOrDefault(p => p.OrderId == orderId);

                    if (order != null)
                    {
                        order.Status = StatusOrder.Finished;
                        order.DatePayment = DateTime.Now;
                        db.SaveChanges();
                        success = true;
                    }
                }
            }
            catch (Exception e)
            {
                success = false;
            }

            return success;
        }

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