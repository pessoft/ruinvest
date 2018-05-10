using QiwiApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ruinvest.Logic
{
    public class QiwiWallet
    {
        private static Qiwi qiwi;
        // потом вынести в ресурсы
        private readonly string PHONE = "+79601605946";
        private readonly string TOKEN = "5ba77f9e8666a87c0a1d9f19ec72a9f1";

        public QiwiWallet()
        {
            if(qiwi == null)
            {
                qiwi = new Qiwi(PHONE, TOKEN);
            }
        }

    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="phone">в формате начиная с 7, и без плюса</param>
        /// <param name="amount"></param>
        public void SendMoney(string phone, double amount)
        {
            var success = qiwi.SendMoneyToWallet(phone, (decimal)amount).Result;
        }

        public double GetBalance()
        {
            return (double)qiwi.GetBalancesAsync().Result.Rub;
        }
    }
}