using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Protocols;

namespace SSB.Service.Web
{
    public class Authentication :SoapHeader
    {
        public string Username;
        public string Password;

        public object Clone()
        {
            return base.MemberwiseClone();
        }
    }
}