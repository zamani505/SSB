using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using SSB.Service.Core;

namespace SSB.Service.Web
{
    /// <summary>
    /// Summary description for AccountingWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class AccountingWebService : System.Web.Services.WebService
    {

        //[WebMethod]
        //public string Registeration(string[] strParamsCustomer, int RoleAccounting, string ManagerID, string LineNumber, string[] strParamsUserCredit)
        //{
        //    SMSService srv = new SMSService();
        //    return srv.Registeration(strParamsCustomer, RoleAccounting, ManagerID, LineNumber, strParamsUserCredit);
        //}
        [WebMethod]
        public string RegisterAcc(string[] strParamsCustomer, int RoleAccounting, string ManagerID, string LineNumber, string[] strParamsUserCredit)
        {
            SMSService srv = new SMSService();
            return srv.Registeration(strParamsCustomer, RoleAccounting, ManagerID, LineNumber, strParamsUserCredit);
        }
    }
}
