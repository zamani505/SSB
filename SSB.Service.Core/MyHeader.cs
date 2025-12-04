using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Protocols;

namespace SSB.Service.Core
{
    public class Header : SoapHeader
    {
        public string SecurKey;
    }
}