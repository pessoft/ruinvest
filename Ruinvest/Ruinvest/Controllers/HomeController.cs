using Ruinvest.Logic;
using Ruinvest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [Authorize]
        public ActionResult CreateDeposit()
        {
            return View();
        }

        //to do не забыть в дальнейшем проверить наличие средств на счету
        [HttpPost]
        [Authorize]
        public JsonResult CreateDeposit(CreateDepositModel model)
        {
            var result = new JSONResult();

            if (model.DepositAmount < 100 || model.DepositAmount > 50000)
            {
                result.SetNotSuccess(ErrorMessages.IncorrectAmount);
            }
            else
            {
                var currentDate = DateTime.Now;
                var percent = model.Rate == Rates.Month ? ProfitValue.HighPercent : ProfitValue.BasePercent;
                model.Rate = model.Rate == Logic.Rates.Unknown ? Logic.Rates.OneDay : model.Rate;

                var deposit = new Deposit()
                {
                    UserId = AuthWrapper.GetUserIdByLogin(User.Identity.Name),
                    StartDate = currentDate,
                    EndDate = currentDate.AddDays((int)model.Rate),
                    Percent = percent,
                    StartAmount = model.DepositAmount,
                    InterimAmount = model.DepositAmount,
                    EndAmount = model.DepositAmount + (model.DepositAmount * percent / 100.0) * (int)model.Rate,
                    Status = StatusDeposit.Active
                };

                var success = DataWrapper.AddNewDeposit(deposit);

                if (success)
                {
                    result.SetIsSuccess();
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