using Ruinvest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ruinvest.Logic
{
    public class AuthWrapper
    {
        public static int GetUserIdByLogin(string login)
        {
            User user;
            using (UserContext db = new UserContext())
            {
                user = db.Users.FirstOrDefault(p => p.PhoneNumber == login);
            }

            return user.Id;
        }

        public static bool AddNewUser(User user)
        {
            using (UserContext db = new UserContext())
            {
                db.Users.Add(user);
                db.SaveChanges();

                user = db.Users.FirstOrDefault(p => p.PhoneNumber == user.PhoneNumber);
            }

            return user != null;
        }

        public static bool UserExist(string phoneNumber)
        {
            User user;
            using (UserContext db = new UserContext())
            {
                user = db.Users.FirstOrDefault(p => p.PhoneNumber == phoneNumber);
            }

            return user != null;
        }

        public static bool LoginUser(string phoneNumber, string password)
        {
            User user;
            using (UserContext db = new UserContext())
            {
                user = db.Users.FirstOrDefault(p => p.PhoneNumber == phoneNumber && p.Password == password);
            }

            return user != null;
        }
    }
}