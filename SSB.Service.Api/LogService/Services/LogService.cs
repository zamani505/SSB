
using Newtonsoft.Json;
using SSB.Service.SSBApi.CacheManager.Log;
using SSB.Service.SSBApi.Constant;
using SSB.Service.SSBApi.Extentions;
using SSB.Service.SSBApi.Models;
using System;
using System.Linq;
using System.Net.Http;


namespace SSB.Service.SSBApi.LogService
{
    public class LogService
    {
        #region props
        private readonly DBService _dBService;
        private readonly CacheLog _cacheLog;
        #endregion
        #region ctors
        public LogService()
        {
            _dBService = new DBService();
            _cacheLog = new CacheLog();
        }
        #endregion
        #region public methods
        public HttpRequestMessage AddLog(HttpRequestMessage request)
        {
            var requestBody = request.Body();
            var audit = SMSAuditLog.New(GetUsername(request), request.RequestUri.AbsolutePath.GetVerbName(),
                requestBody, request.GetClientIpAddress());
            var id = _dBService.NewLog(audit);
            var logKey = $"{GetUsername(request)}_{id}";
            request.Headers.Add(SSBConstant.LOG_KEY_HEADER, logKey);
            _cacheLog.Add(logKey, LogModel.New(id, audit.CreateTime, GetUsername(request)));
            return request;

        }
        public void UpdateLog(object body, string cacheKey,Exception ex=null)
        {
            var logModel = _cacheLog.Get(cacheKey);
            var ext = ex == null ? "" : ex.Message;
            var stc = ex == null ? "" : ex.StackTrace;
            if (logModel != null)
            {
                var audit = SMSAuditLog.Update(JsonConvert.SerializeObject(body), logModel.CreateTime.Diff(DateTime.Now),logModel.Id,ext,stc);
                _dBService.UpdateLog(audit);
            }

        }
        public string GetUsername(HttpRequestMessage request)
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