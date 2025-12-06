using SSB.Service.Core;
using SSB.Service.SSBApi.CacheManager.Login;
using SSB.Service.SSBApi.Constant;
using SSB.Service.SSBApi.Extentions;
using SSB.Service.SSBApi.Models.ArraySendQeue;
using SSB.Service.SSBApi.Models.ArraySendQeueWithId;
using SSB.Service.SSBApi.Models.Login;
using SSB.Service.SSBApi.Models.Send;
using SSB.Service.SSBApi.Models.SendFromUrl;
using SSB.Service.SSBApi.Models.SendPostUrl;
using SSB.Service.SSBApi.Models.SendQeue;
using SSB.Service.SSBApi.Models.SendSMS;
using SSB.Service.SSBApi.Models.SMS;
using SSB.Service.SSBApi.Validation;
using SSB.Service.Web.avanak;
using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Razor.Tokenizer;
using System.Web.Services.Description;
using HttpGetAttribute = System.Web.Mvc.HttpGetAttribute;
using HttpPostAttribute = System.Web.Mvc.HttpPostAttribute;
using RoutePrefixAttribute = System.Web.Mvc.RoutePrefixAttribute;

namespace SSB.Service.SSBApi.Controllers
{
    [System.Web.Mvc.Route("api/SSBSMS")]
    public class SSBSMSController : ApiController
    {
        #region props
        public readonly CacheLogin _cacheLogin;
        SMSService _service;
        LineNumerValidation _lineNumberValidation;
        public readonly string _username;
        #endregion
        #region ctors

        public SSBSMSController()
        {
            _cacheLogin = new CacheLogin();
            _service = new SMSService();
            _lineNumberValidation = new LineNumerValidation();
            _username = _cacheLogin.GetUsername(HttpContext.Current.Request.Headers[SSBConstant.TOKEN_NAME]);
        }
        #endregion
        #region public methods

        [HttpPost]
        //[Route("Login")]
        public IHttpActionResult Login([FromBody] LoginVM loginVM)
        {
            var validate = new LoginValidation().Validate(loginVM.Username, loginVM.Password);
            if (!string.IsNullOrEmpty(validate))
                return Ok(new LoginDto() { Message = validate });
            var token = loginVM.Username.CreateToken(loginVM.Password);
            _cacheLogin.AddSession(token, loginVM.Username);
            return Ok(new LoginDto() { SSBToken = token });
        }
        [HttpPost]
        public IHttpActionResult SendFromUrl([FromBody] SendFromUrlVM sendFromUrlVM)
        {
            sendFromUrlVM.FromNumber = Helpers.Utility.FixPhoneNumber(sendFromUrlVM.FromNumber);

            var validate = _lineNumberValidation.LineValidation(new string[] { sendFromUrlVM.FromNumber }, _username);
            if (!string.IsNullOrEmpty(validate))
                return Ok(new SMSDto() { Code = validate });

            return Ok(SSB_SendSMSQueue(sendFromUrlVM.Message, sendFromUrlVM.ToNumber, sendFromUrlVM.FromNumber, _username));
        }
        [HttpPost]
        public IHttpActionResult SendPostUrl([FromBody] SendPostUrlVM sendPostUrlVM)
        {
            sendPostUrlVM.FromNumber = Helpers.Utility.FixPhoneNumber(sendPostUrlVM.FromNumber);

            var validate = _lineNumberValidation.LineValidation(new string[] { sendPostUrlVM.FromNumber }, _username);
            if (!string.IsNullOrEmpty(validate))
                return Ok(new SMSDto() { Code = validate });

            return Ok(SSB_SendSMSQueue(sendPostUrlVM.Message, sendPostUrlVM.ToNumber, sendPostUrlVM.FromNumber, _username));

        }
        [HttpPost]
        public IHttpActionResult ArraySendQeue([FromBody] ArraySendQeueVM arraySendQeueVM)
        {
            arraySendQeueVM.SenderNumbers = arraySendQeueVM.SenderNumbers.FixPhoneNumber();

            var validate = _lineNumberValidation.LineValidation(arraySendQeueVM.SenderNumbers, _username);
            if (!string.IsNullOrEmpty(validate))
                return Ok(new SMSDto() { Code = validate });

            return Ok(SSB_SendSMSQueue(arraySendQeueVM.Messages, arraySendQeueVM.Mobiles, arraySendQeueVM.SenderNumbers, _username));
        }

        [HttpPost]
        public IHttpActionResult ArraySendQeueWithId([FromBody] ArraySendQeueWithIdVM arraySendQeueWithIdVM)
        {
            arraySendQeueWithIdVM.SenderNumbers = arraySendQeueWithIdVM.SenderNumbers.FixPhoneNumber();

            var validate = _lineNumberValidation.LineValidation(arraySendQeueWithIdVM.SenderNumbers, _username);
            if (!string.IsNullOrEmpty(validate))
                return Ok(new SMSDto() { Code = validate });

            return Ok(SendSMSQueueWithId(arraySendQeueWithIdVM.Ids, arraySendQeueWithIdVM.Messages, arraySendQeueWithIdVM.Mobiles, arraySendQeueWithIdVM.SenderNumbers, _username));

        }
        [HttpPost]
        public IHttpActionResult SendQeue([FromBody] SendQeueVM sendQeueVM)
        {
            sendQeueVM.FromNumber = Helpers.Utility.FixPhoneNumber(sendQeueVM.FromNumber);
            var validate = _lineNumberValidation.LineValidation(new string[] { sendQeueVM.FromNumber }, _username);
            if (!string.IsNullOrEmpty(validate))
                return Ok(new SMSDto() { Code = validate });

            return Ok(SSB_SendSMSQueue(sendQeueVM.Message, sendQeueVM.ToNumber, sendQeueVM.FromNumber, _username));
        }
        [HttpPost]
        public IHttpActionResult Send([FromBody] SendVM sendVM) {
            sendVM.FromNumber = Helpers.Utility.FixPhoneNumber(sendVM.FromNumber);
            int lenght=sendVM.ToNumber.Length;
            string[] sms = new string[lenght];
            int[] encods = new int[lenght];
            int[] mclass = new int[lenght];
            int[] priority = new int[lenght];
            string[] origis = new string[lenght];
            string[] udh = new string[lenght];
            long[] checkingIds = new long[lenght];
            Random rnd = new Random();
            for (int i = 0; i < lenght; i++)
            {
                sms[i] = sendVM.Message;
                encods[i] = 1;
                origis[i] = sendVM.FromNumber;
                udh[i] = "";
                mclass[i] = 1;
                priority[i] = -1;
                checkingIds[i] = rnd.Next();
            }
            return Ok(SSB_SendSMS(sms, encods, sendVM.ToNumber, origis, udh, mclass, priority, checkingIds, _username));
        }
        #endregion
        #region private methods
        private SMSDto SendSMSQueueWithId(Guid[] ids, string[] messages, string[] mobiles, string[] senderNumbers, string username)
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
        private SMSDto SSB_SendSMSQueue(string[] messages, string[] mobiles, string[] origs, string username)
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
        private SMSDto SSB_SendSMSQueue(string messages, string mobiles, string origs, string username)
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
        private SMSDto SSB_SendSMSQueue(string messages, string[] mobiles, string origs, string username)
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
        private SendSMSDto SSB_SendSMS(string[] messages, int[] encodings, string[] mobiles, string[] origs, string[] udh, int[] messageClass, int[] priorities, long[] checkingIds, string username) {
            try
            {
                return new SendSMSDto(){ Result = _service.SendSMS(messages,encodings,mobiles,origs,udh,messageClass,priorities,checkingIds,username)};
            }
            catch (Exception ex)
            {

                return new SendSMSDto() { Message = "متاسفانه مشکلی بوجود آمده است" };
            }
        }
        #endregion
    }
}