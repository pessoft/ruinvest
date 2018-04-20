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
        private static readonly string STORE_ID = "73265";
        private static readonly string SICRET1 = "6k5rufnq";
        private static readonly string SICRET2 = "vt736mrm";


        public static string GetStoreId()
        {
            return STORE_ID;
        }

        public static string GetSignatureMoneyIn(this OrderTopBalanceModel order)
        {
            return GetMd5HashHelper(order, SICRET1);
        }

        public static string GetSignatureOrderNotify(this OrderTopBalanceModel order)
        {
            return GetMd5HashHelper(order, SICRET2);
        }

        private static string GetMd5HashHelper(OrderTopBalanceModel order, string secret)
        {
            var str = $"{STORE_ID}:{order.Amount}:{secret}:{order.OrderId}";
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