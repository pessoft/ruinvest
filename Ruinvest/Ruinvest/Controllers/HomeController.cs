using RuinvestLogic.Logic;
using RuinvestLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace Ruinvest.Controllers
{
    public class HomeController : Controller
    {
        private readonly double MIN_AMOUNT = 50;
        private readonly double MAX_AMOUNT = 50000;

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public JsonResult MoneyOut(MoneyOutModel moneyOutData)
        {
            var result = new JSONResult();
            var userId = AuthWrapper.GetUserIdByLogin(User.Identity.Name);
            var availableMoney = DataWrapper.AvailableMoneyByUserId(userId);

            if (moneyOutData.IsValidAmount() && moneyOutData.Amount <= availableMoney)
            {
                
                var newOrder = new OrderMoneyOut()
                {
                    OrderId = Guid.NewGuid().ToString(),
                    UserId = userId,
                    Amount = moneyOutData.Amount,
                    OrderDate = DateTime.Now,
                    Status = StatusOrder.InProgress,
                    TypePurce = moneyOutData.TypePurce
                };

                var isAddNewOrder = DataWrapper.AddNewOrderMoneyOut(newOrder);
                var newBalanceUser = 0.0;

                if (isAddNewOrder)
                {
                    DataWrapper.TakeMoneyByUserId(userId, moneyOutData.Amount);
                    newBalanceUser = DataWrapper.AvailableMoneyByUserId(userId);
                }
                
                result.SetIsSuccess(newBalanceUser);
            }
            else
            {
                if (moneyOutData.IsValidAmount())
                {
                    result.SetNotSuccess(ErrorMessages.IncorrectAmount);
                }
                else
                {
                    result.SetNotSuccess(ErrorMessages.NotEnoughMoney);
                }

            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult MoneyIn(MoneyInModel amountData)
        {
            var result = new JSONResult();

            if (amountData.IsValidAmount())
            {
                var newOrder = new OrderTopBalanceModel()
                {
                    OrderId = Guid.NewGuid().ToString(),
                    UserId = AuthWrapper.GetUserIdByLogin(User.Identity.Name),
                    Amount = amountData.Amount,
                    OrderDate = DateTime.Now,
                    Status = StatusOrder.InProgress
                };

                DataWrapper.AddNewOrderTopBalance(newOrder);
                result.SetIsSuccess(FreeKassa.GetUrlCash(newOrder));
            }
            else
            {
                result.SetNotSuccess(ErrorMessages.IncorrectAmount);
            }

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
                var message = $"Пополнение<br>" +
                              $"Дата:{DateTime.Now.ToString()}<br>" +
                              $"Пользователь:{order.UserId}<br>" +
                              $"Сумма:{amount}<br>";
                RuinvestUtils.VK.VKLogic.GetInstance().SendMessage(message);
                DataWrapper.MarkOrderTopBalanceFinished(order.OrderId);
                FreeKassa.SendToCard(order);
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

        [HttpPost]
        [Authorize]
        public JsonResult CreateDeposit(CreateDepositModel model)
        {
            var result = new JSONResult();
            var userId = AuthWrapper.GetUserIdByLogin(User.Identity.Name);
            var availableMoney = DataWrapper.AvailableMoneyByUserId(userId);

            if ((availableMoney < model.DepositAmount) || (model.DepositAmount < MIN_AMOUNT || model.DepositAmount > MAX_AMOUNT))
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
                model.Rate = model.Rate == Rates.Unknown ? Rates.OneDay : model.Rate;

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

            deposits = deposits?.OrderByDescending(p => p.Status)
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
            var userId = AuthWrapper.GetUserIdByLogin(User.Identity.Name);

            ViewBag.AvailableMone = DataWrapper.AvailableMoneyByUserId(userId);
            ViewBag.HasOrder = DataWrapper.HasNonProcessedMoneyOut(userId);
            ViewBag.Orders = DataWrapper.GetMoneyOrdersByUserId(userId);
            ViewBag.PhoneNumber = User.Identity.Name;

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
            if (ModelState.IsValid && model.IsValid())
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
                        accountResult.SetIsSuccess(GetUrlRedirect(model.PhoneNumber));
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
                if (!model.IsValid())
                {
                    accountResult.SetNotSuccess(ErrorMessages.NotFullDataRegistration);
                }
                else
                {
                    accountResult.SetNotSuccess(ErrorMessages.UnknownError);
                }
            }

            return Json(accountResult, JsonRequestBehavior.AllowGet);
        }

        private string GetUrlRedirect(string login)
        {
            var userId = AuthWrapper.GetUserIdByLogin(login);
            var deposits = DataWrapper.GetDepostByUserId(userId);
            var depositCount = deposits.Count;

            return depositCount == 0 ? "/Home/CreateDeposit" : "/Home/Deposits";
        }


        [HttpPost]
        public JsonResult Login(LoginModel model)
        {
            JSONResult accountResult = new JSONResult();

            if (ModelState.IsValid && model.IsValid())
            {
                if (AuthWrapper.LoginUser(model.PhoneNumber, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.PhoneNumber, true);
                   
                    accountResult.SetIsSuccess(GetUrlRedirect(model.PhoneNumber));
                }
                else
                {
                    accountResult.SetNotSuccess(ErrorMessages.NotValidAuthData);
                }
            }
            else
            {
                if (!model.IsValid())
                {
                    accountResult.SetNotSuccess(ErrorMessages.NotFullDataLogin);
                }
                else
                {
                    accountResult.SetNotSuccess(ErrorMessages.UnknownError);
                }
            }

            return Json(accountResult, JsonRequestBehavior.AllowGet);
        }
    }
}