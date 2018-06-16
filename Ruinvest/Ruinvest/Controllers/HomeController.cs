using NLog;
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
        private Logger logger = LogManager.GetCurrentClassLogger();
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
            try
            {
                var userId = AuthWrapper.GetUserIdByLogin(User.Identity.Name);
                var availableMoney = DataWrapper.AvailableMoneyByUserId(userId);
                var currentDate = DateTime.Now;
                var date1 = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 9, 0, 0);
                var date2 = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 22, 0, 0);
                if (moneyOutData.IsValidAmount() && moneyOutData.Amount <= availableMoney)
                {
                    if (currentDate < date1 || currentDate > date2 
                        || currentDate.DayOfWeek == DayOfWeek.Saturday
                        || currentDate.DayOfWeek == DayOfWeek.Sunday)
                    {
                        result.SetNotSuccess(ErrorMessages.IncorrectDate);
                    }
                    else
                    {

                        var newOrder = new OrderMoneyOut()
                        {
                            OrderId = Guid.NewGuid().ToString(),
                            UserId = userId,
                            Amount = moneyOutData.Amount,
                            OrderDate = DateTime.Now,
                            Status = StatusOrder.InProgress,
                            TypePurce = moneyOutData.TypePurce,
                            NumberPurce = moneyOutData.NumberPurce
                        };

                        var isAddNewOrder = DataWrapper.AddNewOrderMoneyOut(newOrder);
                        var newBalanceUser = 0.0;

                        if (isAddNewOrder)
                        {
                            DataWrapper.TakeMoneyByUserId(userId, moneyOutData.Amount);
                            newBalanceUser = DataWrapper.AvailableMoneyByUserId(userId);


                        }

                        result.SetIsSuccess("/Home/MoneyOut");
                    }
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
            }
            catch (Exception ex)
            {
                result.SetNotSuccess(ErrorMessages.UnknownError);
                logger.Error("Method MoneyOut: ", ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult MoneyIn(MoneyInModel amountData)
        {
            var result = new JSONResult();

            try
            {
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
                    var d = FreeKassa.GetUrlCash(newOrder);
                    result.SetIsSuccess(FreeKassa.GetUrlCash(newOrder));
                }
                else
                {
                    result.SetNotSuccess(ErrorMessages.IncorrectAmount);
                }
            }
            catch (Exception ex)
            {
                result.SetNotSuccess(ErrorMessages.UnknownError);
                logger.Error("Method MoneyIn: ", ex);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string OrderNotify()
        {
            var result = string.Empty;
            try
            {
                string amountStr = "50";//Request.QueryString["AMOUNT"];
                double amount = 0;
                double.TryParse(amountStr, out amount);
                string orderId = "5de66cc3-2fca-47c7-924a-36869411ce40";// Request.QueryString["MERCHANT_ORDER_ID"];
                string sign = "c604b4d409f2fbd9e041178e5f9ac5ea";// Request.QueryString["SIGN"];
                var order = DataWrapper.GetOrderTopBalanceByOrderId(orderId);

                if (order.GetSignatureOrderNotify() == sign && order.Amount == amount)
                {
                    var message = $"Пополнение<br>" +
                                  $"Дата:{DateTime.Now.ToString()}<br>" +
                                  $"Пользователь:{order.UserId}<br>" +
                                  $"Сумма:{amount}<br>";
                    RuinvestUtils.VK.VKLogic.GetInstance().SendMessage(message);
                    DataWrapper.MarkOrderTopBalanceFinished(order.OrderId);
                    //FreeKassa.SendToCard(order);

                    result = "yes";
                }
            }
            catch (Exception e)
            { }
            return result;
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
        public JsonResult ChangeNotifySetting(NotificationMessageModel model)
        {
            var result = new JSONResult();
            try
            {
                var userId = AuthWrapper.GetUserIdByLogin(User.Identity.Name);
                model.UserId = userId;

                DataWrapper.AddOrUpdateNotification(model);
            }
            catch (Exception ex)
            {
                result.SetNotSuccess(ErrorMessages.UnknownError);
                logger.Error("Method ChangeNotifySetting: ", ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult CreateDeposit(CreateDepositModel model)
        {
            var result = new JSONResult();
            try
            {
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
            }
            catch (Exception ex)
            {
                result.SetNotSuccess(ErrorMessages.UnknownError);
                logger.Error("Method CreateDeposit: ", ex);
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

            ViewBag.AvailableMoney = DataWrapper.AvailableMoneyByUserId(userId);
            ViewBag.HasOrder = DataWrapper.HasNonProcessedMoneyOut(userId);
            ViewBag.Orders = DataWrapper.GetMoneyOrdersByUserId(userId);
            ViewBag.PhoneNumber = User.Identity.Name;
            ViewBag.IsShowInfoMoneyOut = DataWrapper.GetNotificationSettingByUserIdAndType(userId, TypeNotification.InfoMoneyOut).IsShow;
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
            try
            {
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
            }
            catch (Exception ex)
            {
                accountResult.SetNotSuccess(ErrorMessages.UnknownError);
                logger.Error("Method Registration: ", ex);
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
            try
            {
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
            }
            catch (Exception ex)
            {
                accountResult.SetNotSuccess(ErrorMessages.UnknownError);
                logger.Error("Method Login: ", ex);

            }
            return Json(accountResult, JsonRequestBehavior.AllowGet);
        }
    }
}