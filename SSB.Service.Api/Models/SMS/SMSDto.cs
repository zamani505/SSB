using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSB.Service.SSBApi.Models.SMS
{
    public class SMSDto
    {
        public string[] Result { get; set; }
        public string Message { get; set; }
        public string Code { get; set; }
    }
}