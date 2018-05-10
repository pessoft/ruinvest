using Ruinvest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace Ruinvest.Logic
{
    public static class UtilsHelper
    {
        public static bool IsValid(this RegistrationModel model)
        {
            return !string.IsNullOrEmpty(model.FirstName)
                && !string.IsNullOrEmpty(model.SecondName)
                && !string.IsNullOrEmpty(model.PhoneNumber)
                && !string.IsNullOrEmpty(model.Password);
        }

        public static bool IsValid(this LoginModel model)
        {
            return !string.IsNullOrEmpty(model.PhoneNumber)
                && !string.IsNullOrEmpty(model.Password);
        }
        
        public static bool EqualsDate(this DateTime date1, DateTime date2)
        {
            return date1.Date.Equals(date2.Date);
        }

        public static bool IsValidAmount(this MoneyInModel amountData)
        {
            return IsValidAmount(amountData.Amount);
        }

        public static bool IsValidAmount(this MoneyOutModel moneyOutData)
        {
            return IsValidAmount(moneyOutData.Amount);
        }

        public static bool IsValidAmount(double money)
        {
            return money >= 100 && money <= 50000;
        }

        public static string GetSignatureMoneyIn(this OrderTopBalanceModel order)
        {
            return GetMd5HashHelper(order, FreeKassa.SICRET1);
        }

        public static string GetSignatureOrderNotify(this OrderTopBalanceModel order)
        {
            return GetMd5HashHelper(order, FreeKassa.SICRET2);
        }

        private static string GetMd5HashHelper(OrderTopBalanceModel order, string secret)
        {
            var str = $"{FreeKassa.STORE_ID}:{order.Amount}:{secret}:{order.OrderId}";
            using (MD5 md5Hash = MD5.Create())
            {
                return GetMd5Hash(md5Hash, str);
            }
        }

        private static string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}