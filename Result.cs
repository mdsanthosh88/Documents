using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationDashboardServiceCommon.Result
{
    public class Result<T>
    {
        public bool IsValid
        {
            get;
            private set;
        }
        public T Value
        {
            get;
            private set;
        }
        public string ErrorMessage
        {
            get;
            set;
        }
        public Result(bool isValid)
        {
            this.IsValid = isValid;
        }
        public Result(bool isValid, string errorMessage)
        {
            this.IsValid = isValid;
            this.ErrorMessage = errorMessage;
        }
        public Result(bool isValid, T value)
        {
            this.IsValid = isValid;
            this.Value = value;
        }
        public Result(bool isValid, T value, string errorMessage)
        {
            this.IsValid = isValid;
            this.Value = value;
            this.ErrorMessage = errorMessage;
        }
    }
}
