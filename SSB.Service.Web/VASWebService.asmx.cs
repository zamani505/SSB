using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using SSB.Service.Core;

namespace SSB.Service.Web
{
    /// <summary>
    /// Summary description for VASWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class VASWebService : System.Web.Services.WebService
    {
        [WebMethod]
        public List<VASServiceInfo> GetVASServices()
        {
            List<VASServiceInfo> lstServices = new List<VASServiceInfo>();
            SMSService srv = new SMSService();
            var q = srv.GetVasServices().ToList();
            foreach (var item in q)
            {
                VASServiceInfo vasItem = new VASServiceInfo();
                vasItem.Id = item.Id;
                vasItem.Price = item.Price;
                vasItem.Published = item.Published;
                vasItem.Title = item.Title;
                lstServices.Add(vasItem);
            }
            return lstServices;
        }

        public class VASServiceInfo
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public int Price { get; set; }
            public bool Published { get; set; }
        }
    }
}
