using Newtonsoft.Json;
using SSB.Service.Core;
using SSB.Service.SSBApi.CacheManager.Login;
using SSB.Service.SSBApi.Constant;
using SSB.Service.SSBApi.Models;
using SSB.Service.SSBApi.Validation;
using System;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using static SSB.Service.SSBApi.Constant.SSBConstant;
namespace SSB.Service.SSBApi.Controllers
{
    public class BaseController : ApiController
    {
        #region props
        protected readonly CacheLogin _cacheLogin;
        protected SMSService _service;
        protected LineNumerValidation _lineNumberValidation;
        public readonly string _username;
        protected LogService.LogService _logService;
        protected HttpRequest _request;
        protected readonly string _cacheKey = "";
        #endregion
        #region ctors
        public BaseController()
        {
            _cacheLogin = new CacheLogin();
            _service = new SMSService();
            _logService=new LogService.LogService();
            _lineNumberValidation = new LineNumerValidation();
            _request = HttpContext.Current.Request;
            _cacheKey= _request.Headers.GetValues(SSBConstant.LOG_KEY_HEADER).FirstOrDefault() ?? "";
            if (_request.Headers[SSBConstant.TOKEN_NAME] != null)
                _username = _cacheLogin.GetUsername(HttpContext.Current.Request.Headers[SSBConstant.TOKEN_NAME]);
        }
        #endregion
        #region protected methods
        protected SMSDto SendSMSQueueWithId(Guid[] ids, string[] messages, string[] mobiles, string[] senderNumbers, string username)
        {
            try
            {
                return new SMSDto() { Result = _service.SendSMSQueueWithId(ids, messages, mobiles, senderNumbers, username) };
            }
            catch (Exception ex)
            {
                var resp= new SMSDto() { Code = SSBErrorCode.EXCEPTION.ToString(), Message = "متاسفانه مشکلی بوجود آمده است" };
                _logService.UpdateLog(JsonConvert.SerializeObject(resp), _cacheKey, ex);
                return resp;
            }
        }
        protected SMSDto SSB_SendSMSQueue(string[] messages, string[] mobiles, string[] origs, string username)
        {
            try
            {
                return new SMSDto() { Result = _service.SendSMSQueue(messages, mobiles, origs, username) };
            }
            catch (System.Exception ex)
            {
                var resp = new SMSDto() { Code = SSBErrorCode.EXCEPTION.ToString(), Message = "متاسفانه مشکلی بوجود آمده است" };
                _logService.UpdateLog(JsonConvert.SerializeObject(resp), _cacheKey, ex);
                return resp;
            }
        }
        protected SMSDto SSB_SendSMSQueue(string messages, string mobiles, string origs, string username)
        {
            try
            {
                string[] toNumbers = mobiles.Split(',');
                return new SMSDto() { Result = _service.SendSMSQueue(messages, toNumbers, origs, username) };
            }
            catch (System.Exception ex)
            {

                var resp = new SMSDto() { Code = SSBErrorCode.EXCEPTION.ToString(), Message = "متاسفانه مشکلی بوجود آمده است" };
                _logService.UpdateLog(JsonConvert.SerializeObject(resp), _cacheKey, ex);
                return resp;
            }
        }
        protected SMSDto SSB_SendSMSQueue(string messages, string[] mobiles, string origs, string username)
        {
            try
            {
                return new SMSDto() { Result = _service.SendSMSQueue(messages, mobiles, origs, username) };
            }
            catch (System.Exception ex)
            {

                var resp = new SMSDto() { Code = SSBErrorCode.EXCEPTION.ToString(), Message = "متاسفانه مشکلی بوجود آمده است" };
                _logService.UpdateLog(JsonConvert.SerializeObject(resp), _cacheKey, ex);
                return resp;
            }
        }
        protected SendSMSDto SSB_SendSMS(string[] messages, int[] encodings, string[] mobiles, string[] origs, string[] udh, int[] messageClass, int[] priorities, long[] checkingIds, string username)
        {
            try
            {
                return new SendSMSDto() { Result = _service.SendSMS(messages, encodings, mobiles, origs, udh, messageClass, priorities, checkingIds, username) };
            }
            catch (Exception ex)
            {
                var resp = new SendSMSDto() { Code = SSBErrorCode.EXCEPTION.ToString(), Message = "متاسفانه مشکلی بوجود آمده است" };
                _logService.UpdateLog(JsonConvert.SerializeObject(resp), _cacheKey, ex);
                return resp;
               
            }
        }
        protected SendSMSDto SSB_SendSMSArrayToMagfa(string[] messages, int[] encodings, string[] mobiles, string[] origs, string[] udh, int[] messageClass, int[] priorities, long[] checkingIds, string username)
        {
            try
            {
                return new SendSMSDto() { Result = _service.SendSMSForArraySendMagfa(messages, encodings, mobiles, origs, udh, messageClass, priorities, checkingIds, username) };
            }
            catch (Exception ex)
            {
                var resp = new SendSMSDto() { Code = SSBErrorCode.EXCEPTION.ToString(), Message = "متاسفانه مشکلی بوجود آمده است" };
                _logService.UpdateLog(JsonConvert.SerializeObject(resp), _cacheKey, ex);
                return resp;
            }
        }
        protected SMSStatusDto SSB_SMSStatus(long[] messageIds, bool fromMafa = false)
        {
            try
            {
                if (fromMafa)
                    return new SMSStatusDto() { Result = _service.getMessageStatusFromMagfa(messageIds) };
                else
                {
                    int[] result = _service.GetStatusFromContainer(messageIds);
                    return new SMSStatusDto() { Result = result };

                }
            }
            catch (Exception ex)
            {
                var resp = new SMSStatusDto() { Code = SSBErrorCode.EXCEPTION.ToString(), Message = "متاسفانه مشکلی بوجود آمده است" };
                _logService.UpdateLog(JsonConvert.SerializeObject(resp), _cacheKey, ex);
                return resp;
                
            }
        }
        protected SMSStatusDto SSB_SMSStatus(string[] messageIds)
        {
            int[] result = _service.GetStatusFromContainer(messageIds);
            return new SMSStatusDto() { Result = result };
        }


        #endregion
        #region private mthods
       
        #endregion
    }
}