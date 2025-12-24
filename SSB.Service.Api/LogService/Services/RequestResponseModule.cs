using Newtonsoft.Json;
using SSB.Service.SSBApi.CacheManager.Login;
using SSB.Service.SSBApi.Constant;
using SSB.Service.SSBApi.Extentions;
using SSB.Service.SSBApi.Models;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SSB.Service.SSBApi.LogService.Services
{
    public class RequestResponseModule : IHttpModule
    {
        private const string KEY = "__response_filter";
        private const string USER_KEY = "__response_username";
        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += (s, e) =>
            {
                var username = ReqLog(HttpContext.Current.Request);
                var response = HttpContext.Current.Response;
                var filter = new ResponseCaptureStream(response.Filter);
                response.Filter = filter;
                HttpContext.Current.Items[KEY] = filter;
                HttpContext.Current.Items[USER_KEY] = username;
            };

            context.EndRequest += (s, e) =>
            {
                var res = HttpContext.Current.Response;
                string username = "";

                if (HttpContext.Current.Request.Headers[SSBConstant.TOKEN_NAME] == null)
                    username = HttpContext.Current.Items[USER_KEY].ToString();
                else
                    username = new CacheLogin().GetUsername(HttpContext.Current.Request.Headers[SSBConstant.TOKEN_NAME]);
                var filter = HttpContext.Current.Items[KEY] as ResponseCaptureStream;
                if (filter != null)
                {
                    string responseBody = filter.GetBody();
                    int statusCode = HttpContext.Current.Response.StatusCode;
                    new LogService().UpdateLog(responseBody, username);


                }
            };
        }
        #region private methods
        private string ReqLog(HttpRequest request)
        {
            request.InputStream.Position = 0;
            string body = "";
            var ms = new MemoryStream();
            request.InputStream.CopyTo(ms);

            byte[] bytes = ms.ToArray();
            body = Encoding.UTF8.GetString(bytes);


            request.InputStream.Position = 0;

            string verbName = request.Url.AbsoluteUri.GetVerbName().ToLower();
            string username = "";
            if (verbName == "login")
            {
                var loginModel = JsonConvert.DeserializeObject<LoginVM>(body);
                username = loginModel.Username;


            }
            else
            {
                try
                {
                    if (request.Headers[SSBConstant.TOKEN_NAME] != null)
                    {
                        username = new CacheLogin().GetUsername(request.Headers[SSBConstant.TOKEN_NAME]);
                        new LogService().AddLog(username, verbName, body, "0.0.0.0");
                    }

                }
                catch (System.Exception)
                {


                }
            }

           
            return username;
        }
        #endregion
    }
}