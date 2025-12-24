using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using SSB.Service.SSBApi.CacheManager.Login;
using SSB.Service.SSBApi.Constant;
using SSB.Service.SSBApi.Models;
using SSB.Service.SSBApi.Extentions;
using SSB.Service.SSBApi.CacheManager.Log;
using SSB.Service.SSBApi.LogService;

using Newtonsoft.Json;
using System.Web;
using SSB.Service.SSBApi.Controllers;

namespace SSB.Service.SSBApi.Attribute
{
    public class ApiKeyHandler : DelegatingHandler
    {
        #region props
        private static readonly Dictionary<string, Type> VerbTypes = new Dictionary<string, Type>
        {
            { "send", typeof(SendSMSDto) },
            { "sendwithcheckinId", typeof(SendSMSDto) },
            { "arraysend", typeof(SendSMSDto) },
            { "sendwithUdh", typeof(SendSMSDto) },
            { "sendfromUrl", typeof(SMSDto) },
            { "sendpostUrl", typeof(SMSDto) },
            { "arraysendQeue", typeof(SMSDto) },
            { "arraysendQeueWithId", typeof(SMSDto) },
            { "sendqeue", typeof(SMSDto) },

        };
        private readonly CacheLog _cacheLog;
        private readonly CacheLogin _cacheLogin;
        private readonly DBService _dBService;
        private readonly LogService.LogService _logService;

        #endregion
        #region ctors
        public ApiKeyHandler()
        {
            _cacheLog = new CacheLog();
            _cacheLogin = new CacheLogin();
            _dBService = new DBService();
            _logService=new LogService.LogService();
            //var id = _logService.AddLog("", "", "", "");
           
        }
        #endregion
        protected override async Task<HttpResponseMessage> SendAsync(
       HttpRequestMessage request, CancellationToken cancellationToken)
        {

           
            HttpResponseMessage httpResponse = null;
            if (!request.IsRequestLogin())
            {
               
                #region check token
                var body = GetBody(HttpStatusCode.Unauthorized, request.RequestUri.AbsolutePath.GetVerbName());
                if (!request.Headers.Contains(SSBConstant.TOKEN_NAME))
                    httpResponse = request.CreateResponse(HttpStatusCode.Unauthorized, body);
                var key = request.Headers.GetValues(SSBConstant.TOKEN_NAME).FirstOrDefault();
                var exist = new CacheLogin().HaveSession(key);
                if (!exist)
                    httpResponse = request.CreateResponse(HttpStatusCode.Forbidden, body);
                #endregion
                if (httpResponse != null)
                {
                    _logService.UpdateLog(body, request.Headers.GetValues(SSBConstant.LOG_KEY_HEADER).FirstOrDefault() ?? "");
                    return httpResponse;
                }
            }
            httpResponse = await base.SendAsync(request, cancellationToken);
            var responseBody = httpResponse.Content != null ? await httpResponse.Content.ReadAsStringAsync(): "";
            return httpResponse;
        }
        #region private methods
        
        
        private object GetBody(HttpStatusCode statusCode, string verbName)
        {
            VerbTypes.TryGetValue(verbName, out Type targetType);
            object instance = Activator.CreateInstance(targetType);
            if (statusCode == HttpStatusCode.Unauthorized)
                targetType.GetProperty("Message").SetValue(instance, SSBConstant.UNAUTHORIZED_MESSAGE);
            else if (statusCode == HttpStatusCode.Forbidden)
                targetType.GetProperty("Message").SetValue(instance, SSBConstant.INVALID_TOKEN_MESSAGE);
            targetType.GetProperty("Code").SetValue(instance, statusCode.ToString());
            return instance;
        }

        


        #endregion
    }
}