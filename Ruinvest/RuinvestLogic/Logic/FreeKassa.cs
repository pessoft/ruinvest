using RuinvestLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace RuinvestLogic.Logic
{
    public static class FreeKassa
    {
        public static readonly string STORE_ID = "73265";
        public static readonly string SICRET1 = "6k5rufnq";
        public static readonly string SICRET2 = "vt736mrm";

        private static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// url для редиректа на страницу пополнения free-kassa
        /// </summary>
        public static readonly string URL_CASH = @"http://www.free-kassa.ru/merchant/cash.php?";
        public static readonly string URl_MONEY_OUT = @"http://www.free-kassa.ru/api.php?";

        public static string GetUrlCash(OrderTopBalanceModel order)
        {
            var storeId = string.Format("m={0}&", STORE_ID);
            var amountStr = string.Format("oa={0}&", order.Amount);
            var orderId = string.Format("o={0}&", order.OrderId);
            var sign = string.Format("s={0}", order.GetSignatureMoneyIn());
            var resultUrl =  string.Format("{0}{1}{2}{3}{4}", URL_CASH, storeId, amountStr, orderId, sign);

            return resultUrl;
        }

        public static void SendToCard(OrderTopBalanceModel order)
        {
            var currency = string.Format("currency={0}&", "card");
            var storeId = string.Format("merchant_id={0}&", STORE_ID);
            var sign = string.Format("s={0}&", order.GetSignatureOrderNotify());
            var action = "action=payment&";
            var amountStr = string.Format("amount={0}", order.Amount);
            var resultUrl = string.Format("{0}{1}{2}{3}{4}", URl_MONEY_OUT, currency, storeId, sign, action, amountStr);

            var resultGet = GET(resultUrl);
        }

        private static string GET(string uri)
        {
            try
            {
                var response = client.GetStringAsync(uri).Result;

                return response;
            }
            catch (Exception err)
            {
                return null;
            }
        }

    }
}