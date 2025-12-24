
using Newtonsoft.Json;
using SSB.Service.SSBApi.CacheManager.Log;
using SSB.Service.SSBApi.Constant;
using SSB.Service.SSBApi.Extentions;
using SSB.Service.SSBApi.Models;
using System;
using System.Configuration;
using System.IO;
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
        public void LogFile(string text)
        {
            try
            {
                var path =  ConfigurationManager.AppSettings["LogFilePath"].ToString();
                using (var stream = new StreamWriter(path+ "\\LogFile.txt", true))
                {
                    stream.WriteLine($"------------{DateTime.Now}");
                    stream.WriteLine(text);
                    stream.WriteLine($"------------");
                }
            }
            catch (Exception)
            {


            }
        }
        public HttpRequestMessage AddLog(HttpRequestMessage request)
        {
            try
            {
                var requestBody = request.Body();
                var audit = SMSAuditLog.New(GetUsername(request), request.RequestUri.AbsolutePath.GetVerbName(),
                    requestBody, request.GetClientIpAddress());
                LogFile("befor call newlog");
                var id = new DBService().NewLog(audit);
                LogFile("after call newlog id is" + id.ToString());

                var logKey = $"{GetUsername(request)}";
                LogFile("username is" + logKey);
                request.Headers.Add(SSBConstant.LOG_KEY_HEADER, logKey);
                new CacheLog().Add(logKey, LogModel.New(id, audit.CreateTime, GetUsername(request)));
                LogFile("after cache in addlog");
            }
            catch (Exception ex)
            {

                LogFile($"ex in addlog :{ex.Message}");

            }

            return request;

        }
        public void AddLog(string username,string verbName,string  body,string ip)
        {
            try
            {
               
                var audit = SMSAuditLog.New(username,verbName,body,ip);
                LogFile("befor call newlog");
                var id = new DBService().NewLog(audit);
                LogFile("after call newlog id is" + id.ToString());

                var logKey = $"{username}";
                LogFile("username is" + logKey);
               
                new CacheLog().Add(logKey, LogModel.New(id, audit.CreateTime, username));
                LogFile("after cache in addlog");
            }
            catch (Exception ex)
            {

                LogFile($"ex in addlog :{ex.Message}");

            }

           

        }

        public void UpdateLog(object body, string cacheKey, Exception ex = null)
        {
            var logModel = new CacheLog().Get(cacheKey);
            var ext = ex == null ? "" : ex.Message;
            var stc = ex == null ? "" : ex.StackTrace;
            if (logModel != null)
            {
                var audit = SMSAuditLog.Update(JsonConvert.SerializeObject(body), logModel.CreateTime.Diff(DateTime.Now), logModel.Id, ext, stc);
                new DBService().UpdateLog(audit);
            }

        }
        public void UpdateLog(string body, string cacheKey, Exception ex = null)
        {
            var logModel = new CacheLog().Get(cacheKey);
            var ext = ex == null ? "" : ex.Message;
            var stc = ex == null ? "" : ex.StackTrace;
            if (logModel != null)
            {
                var audit = SMSAuditLog.Update(body, logModel.CreateTime.Diff(DateTime.Now), logModel.Id, ext, stc);
                new DBService().UpdateLog(audit);
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