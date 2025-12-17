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
using System.Drawing;
using SSB.Service.SSBApi.LogService;
using Newtonsoft.Json;

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
        #endregion
        #region ctors
        public ApiKeyHandler()
        {
            _cacheLog = new CacheLog();
            _cacheLogin = new CacheLogin();
            _dBService = new DBService();
        }
        #endregion
        protected override async Task<HttpResponseMessage> SendAsync(
       HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AddLog(request);
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
                    UpdateLog(body, request);
                    return httpResponse;
                }
            }
            httpResponse= await base.SendAsync(request, cancellationToken);
            var responseBody = httpResponse.Content != null ? await httpResponse.Content.ReadAsStringAsync(): "";
            UpdateLog(responseBody, request);
            return httpResponse;
        }
        #region private methods
        private void AddLog(HttpRequestMessage request)
        {
            var requestBody = request.Body();
            var audit = SMSAuditLog.New(GetUsername(request), request.RequestUri.AbsolutePath.GetVerbName(),
                requestBody, request.GetClientIpAddress());
            var id = _dBService.NewLog(audit);
            var logKey = $"{GetUsername(request)}_{id}";
            request.Headers.Add(SSBConstant.LOG_KEY_HEADER, logKey);
            _cacheLog.Add(logKey, LogModel.New(id,audit.CreateTime, GetUsername(request)));

        }
        private void UpdateLog(object body, HttpRequestMessage request)
        {
            var key = request.Headers.GetValues(SSBConstant.LOG_KEY_HEADER).FirstOrDefault() ?? "";
            var logModel=_cacheLog.Get(key);
            if (logModel != null) {
                var audit = SMSAuditLog.Update(JsonConvert.SerializeObject(body),logModel.CreateTime.Diff(DateTime.Now),"",logModel.Id);
                _dBService.UpdateLog(audit);
            }

        }
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

        private string GetUsername(HttpRequestMessage request)
        {

            if (request.IsRequestLogin())
            {
                var body = request.Body();
                if (!string.IsNullOrEmpty(body))
                {
                    var loginModel = JsonConvert.DeserializeObject<LoginVM>(body);
                    return loginModel != null ? loginModel.Username : string.Empty;
                }
            }
            return request.Headers.GetValues(SSBConstant.TOKEN_NAME).FirstOrDefault() ?? "";

        }


        #endregion
    }
}