using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ruinvest.Jobs;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async void VKSender()
        {
            var sender = new VkAmountMoneySender();

             await sender.Execute(null);
        }
    }
}
