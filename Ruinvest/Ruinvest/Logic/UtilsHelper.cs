using Ruinvest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ruinvest.Logic
{
    public static class UtilsHelper
    {
        private static readonly string STORE_ID = "73265";
        public static string GetMd5Hash(this OrderTopBalanceModel order, string secret)
        {
            var str = $"{STORE_ID}:{order.Amount}:{secret}:{order.OrderId}";
            var md5Hash = Md5Coder.md5.MD5(str);

            return md5Hash;
        }
    }
}