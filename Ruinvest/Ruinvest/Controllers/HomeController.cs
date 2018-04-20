using Ruinvest.Logic;
using Ruinvest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Ruinvest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public JsonResult MoneyIn(MoneyInModel amountData)
        {
            var result = new JSONResult();

            if (amountData.Amount < 100 || amountData.Amount > 50000)
            {
                result.SetNotSuccess(ErrorMessages.IncorrectAmount);
            }

            var newOrder = new OrderTopBalanceModel()
            {
                OrderId = Guid.NewGuid().ToString(),
                UserId = AuthWrapper.GetUserIdByLogin(User.Identity.Name),
                Amount = amountData.Amount,
                OrderDate = DateTime.Now,
                Status = StatusOrder.InProgress
            };

            //DataWrapper.AddNewOrderTopBalance(newOrder);
            var signature = newOrder.GetSignatureMoneyIn();
            var uri = @"http://www.free-kassa.ru/merchant/cash.php?";
            var storeId = string.Format("m={0}&", UtilsHelper.GetStoreId());
            var amountStr = string.Format("oa={0}&", newOrder.Amount);
            var orderId = string.Format("o={0}&", newOrder.OrderId);
            var sign = string.Format("s={0}", newOrder.GetSignatureMoneyIn());

            var urlMoneyIn = string.Format("{0}{1}{2}{3}{4}", uri, storeId, amountStr, orderId, sign);

            result.SetIsSuccess(urlMoneyIn);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void OrderNotify()
        {
            string amountStr = Request.QueryString["AMOUNT"];
            double amount = 0;
            double.TryParse(amountStr, out amount);
            string orderId = Request.QueryString["MERCHANT_ORDER_ID"];
            string sign = Request.QueryString["SIGN"];
            var order = DataWrapper.GetOrderTopBalanceByOrderId(orderId);

            if (order.GetSignatureOrderNotify() == sign && order.Amount == amount)
            {
                DataWrapper.MarkOrderTopBalanceFinished(order.OrderId);
            }
        }

        [Authorize]
        public ActionResult CreateDeposit()
        {
            var userId = AuthWrapper.GetUserIdByLogin(User.Identity.Name);
            var availableMoney = DataWrapper.AvailableMoneyByUserId(userId);
            ViewBag.AvailableMoney = availableMoney;

            return View();
        }

        //to do не забыть в дальнейшем проверить наличие средств на счету
        [HttpPost]
        [Authorize]
        public JsonResult CreateDeposit(CreateDepositModel model)
        {
            var result = new JSONResult();
            var userId = AuthWrapper.GetUserIdByLogin(User.Identity.Name);
            var availableMoney = DataWrapper.AvailableMoneyByUserId(userId);

            if ((availableMoney < model.DepositAmount) || (model.DepositAmount < 100 || model.DepositAmount > 50000))
            {
                if (availableMoney < model.DepositAmount)
                {
                    result.SetNotSuccess(ErrorMessages.NotEnoughMoney);
                }
                else
                {
                    result.SetNotSuccess(ErrorMessages.IncorrectAmount);
                }
            }
            else
            {
                var currentDate = DateTime.Now;
                var percent = model.Rate == Rates.Month ? ProfitValue.HighPercent : ProfitValue.BasePercent;
                model.Rate = model.Rate == Logic.Rates.Unknown ? Logic.Rates.OneDay : model.Rate;

                var deposit = new Deposit()
                {
                    UserId = userId,
                    StartDate = currentDate,
                    EndDate = currentDate.AddDays((int)model.Rate),
                    Percent = percent,
                    StartAmount = model.DepositAmount,
                    InterimAmount = model.DepositAmount,
                    EndAmount = model.DepositAmount + (model.DepositAmount * percent / 100.0) * (int)model.Rate,
                    Status = StatusDeposit.Active
                };

                var successAddNewDeposit = DataWrapper.AddNewDeposit(deposit);
                var successTakeAmount = DataWrapper.TakeMoneyByUserId(userId, model.DepositAmount);
                var newBalanceUser = DataWrapper.AvailableMoneyByUserId(userId);

                if (successAddNewDeposit && successTakeAmount)
                {
                    result.SetIsSuccess(newBalanceUser);
                }
                else
                {
                    result.SetNotSuccess(ErrorMessages.UnknownError);
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult Deposits()
        {
            var userId = AuthWrapper.GetUserIdByLogin(User.Identity.Name);
            var deposits = DataWrapper.GetDepostByUserId(userId);

            deposits = deposits
                .OrderByDescending(p => p.Status)
                .ThenBy(p => p.StartDate)
                .ToList();
            ViewBag.Deposits = deposits;

            return View();
        }

        [Authorize]
        public ActionResult MoneyIn()
        {
            return View();
        }

        [Authorize]
        public ActionResult MoneyOut()
        {
            return View();
        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public JsonResult Registration(RegistrationModel model)
        {
            JSONResult accountResult = new JSONResult();
            if (ModelState.IsValid)
            {
                if (!AuthWrapper.UserExist(model.PhoneNumber))
                {
                    var newUser = new User
                    {
                        FirstName = model.FirstName,
                        SecondName = model.SecondName,
                        PhoneNumber = model.PhoneNumber,
                        Password = model.Password,
                        RegistrationDate = DateTime.Now
                    };

                    var isSaveUser = AuthWrapper.AddNewUser(newUser);

                    if (isSaveUser)
                    {
                        var userId = AuthWrapper.GetUserIdByLogin(model.PhoneNumber);

                        DataWrapper.AddCashUser(userId);
                        FormsAuthentication.SetAuthCookie(model.PhoneNumber, true);
                        accountResult.SetIsSuccess();
                    }
                    else
                    {
                        accountResult.SetNotSuccess(ErrorMessages.UnknownError);
                    }
                }
                else
                {
                    accountResult.SetNotSuccess(ErrorMessages.ExistentPhoneNumber);
                }
            }
            else
            {
                accountResult.SetNotSuccess(ErrorMessages.UnknownError);
            }

            return Json(accountResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Login(LoginModel model)
        {
            JSONResult accountResult = new JSONResult();

            if (ModelState.IsValid)
            {
                if (AuthWrapper.LoginUser(model.PhoneNumber, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.PhoneNumber, true);
                    accountResult.SetIsSuccess();
                }
                else
                {
                    accountResult.SetNotSuccess(ErrorMessages.NotValidAuthData);
                }
            }
            else
            {
                accountResult.SetNotSuccess(ErrorMessages.UnknownError);
            }

            return Json(accountResult, JsonRequestBehavior.AllowGet);
        }
    }
}