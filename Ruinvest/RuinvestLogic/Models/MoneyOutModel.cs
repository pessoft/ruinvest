﻿using RuinvestLogic.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RuinvestLogic.Models
{
    public class MoneyOutModel
    {
        public string NumberPurce { get; set; }
        public double Amount { get; set; }
        public MoneyOutEnum TypePurce { get; set; }
    }
}