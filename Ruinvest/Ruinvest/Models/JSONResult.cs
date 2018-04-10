using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ruinvest.Models
{
    public class JSONResult
    {
        public bool Success { get; private set; }
        public string ErrMessage { get; private set; }
        public object Data { get; private set; }


        public void SetIsSuccess()
        {
            ErrMessage = "";
            Success = true;
        }

        public void SetIsSuccess(object data)
        {
            SetIsSuccess();
            Data = data;
        }

        public void SetNotSuccess(string errMessage)
        {
            ErrMessage = errMessage;
            Success = false;
        }
    }
}