using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using SSB.Service.Core;

namespace SSB.Service.Web
{
    /// <summary>
    /// Summary description for UserWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class UserWebService : System.Web.Services.WebService
    {
        public Authentication Auth;

        [WebMethod]
        public string[] QuickInsertCutomer(string linenumber, int credit, string version, string moduleNumber,int FaPrice,int EnPrice)
        {
            SMSService srv = new SMSService();
            return srv.QuickInsertCutomer(linenumber, credit, version, moduleNumber,FaPrice,EnPrice);
        }

    }
}
