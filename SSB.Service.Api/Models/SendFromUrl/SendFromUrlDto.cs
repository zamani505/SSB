using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSB.Service.SSBApi.Models.SendFromUrl
{
    public class SendFromUrlDto
    {
        public string[] Result { get; set; }
        public string Message { get; set; }
        public string Code { get; set; }
    }
}