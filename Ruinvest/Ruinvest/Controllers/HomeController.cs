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

        [HttpPost]
        public JsonResult Registration(RegistrationModel model)
        {
            AccountResult accountResult = new AccountResult();
            if (ModelState.IsValid)
            {
                User user;
                using (UserContext db = new UserContext())
                {
                    user = db.Users.FirstOrDefault(p => p.PhoneNumber == model.PhoneNumber);
                }

                if (user == null)
                {
                    using (UserContext db = new UserContext())
                    {
                        db.Users.Add(new User
                        {
                            FirstName = model.FirstName,
                            SecondName = model.SecondName,
                            PhoneNumber = model.PhoneNumber,
                            Password = model.Password,
                            RegistrationDate = DateTime.Now
                        });
                        db.SaveChanges();

                        user = db.Users.FirstOrDefault(p => p.PhoneNumber == model.PhoneNumber);
                    }

                    if (user != null)
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
            AccountResult accountResult = new AccountResult();

            if (ModelState.IsValid)
            {
                User user;
                using (UserContext db = new UserContext())
                {
                    user = db.Users.FirstOrDefault(p => p.PhoneNumber == model.PhoneNumber && p.Password == model.Password);
                }

                if (user == null)
                {
                    accountResult.SetNotSuccess(ErrorMessages.NotValidAuthData);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(model.PhoneNumber, true);
                    accountResult.SetIsSuccess();
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