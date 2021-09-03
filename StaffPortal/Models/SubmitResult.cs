using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffPortal.Models
{
    public enum ErrorLevel
    {
        OK,
        Warning,
        Error
    }

    public class SubmitResult
    {
        public string SubmitResultID { get; set; }

        public bool IsSuccessful { get; set; }

        public ErrorLevel ErrorLevel { get; set; }

        public string ErrorCode { get; set; }

        public string ErrorDescription { get; set; }

        public string StackTrace { get; set; }
    }
}
