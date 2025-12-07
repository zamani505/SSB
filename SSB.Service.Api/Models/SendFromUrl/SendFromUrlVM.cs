using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSB.Service.SSBApi.Models
{
    public class SendFromUrlVM
    {
        public string Message { get; set; }
        public string FromNumber { get; set; }
        public string ToNumber { get; set; }
    }
}