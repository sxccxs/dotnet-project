using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataClasses
{
    public class ExceptionalResult
    {
        public ExceptionalResult(bool isSuccess = true, string exceptionMessage = null)
        {
            this.IsSuccess = isSuccess;
            this.ExceptionMessage = exceptionMessage;
        }

        public bool IsSuccess { get; }

        public string ExceptionMessage { get; }
    }
}
