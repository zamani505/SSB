using SSB.Service.Core;
using SSB.Service.SSBApi.CacheManager.Login;
using SSB.Service.SSBApi.Models.SendSMS;
using SSB.Service.SSBApi.Models.SMS;
using SSB.Service.SSBApi.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace SSB.Service.SSBApi.Controllers
{
    public class BaseController : ApiController
    {
        #region props
        protected readonly CacheLogin _cacheLogin;
        protected SMSService _service;
        protected LineNumerValidation _lineNumberValidation;
        #endregion
        #region ctors
        public BaseController() {
            _cacheLogin = new CacheLogin();
            _service = new SMSService();
            _lineNumberValidation = new LineNumerValidation();
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

                return new SMSDto() { Message = "متاسفانه مشکلی بوجود آمده است" };
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

                return new SMSDto() { Message = "متاسفانه مشکلی بوجود آمده است" };
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

                return new SMSDto() { Message = "متاسفانه مشکلی بوجود آمده است" };
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

                return new SMSDto() { Message = "متاسفانه مشکلی بوجود آمده است" };
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

                return new SendSMSDto() { Message = "متاسفانه مشکلی بوجود آمده است" };
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

                return new SendSMSDto() { Message = "متاسفانه مشکلی بوجود آمده است" };
            }
        }
        #endregion
    }
}