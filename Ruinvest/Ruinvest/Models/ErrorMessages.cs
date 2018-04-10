﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ruinvest.Models
{
    public static class ErrorMessages
    {
        public static readonly string NotValidAuthData = "Не правильный норме телефона или пароль";
        public static readonly string ExistentPhoneNumber = "Пользователь с таким номером телефона уже есть";
        public static readonly string UnknownError = "Что-то пошло не так. Пожалуйста, попробуйте позже";

        public static readonly string IncorrectAmount = "Не корректная сумма депозита. Размер депозита от 100 до 50000 рублей";
        public static readonly string NotEnoughMoney = "На вашем счете не достаточно средств";

    }
}