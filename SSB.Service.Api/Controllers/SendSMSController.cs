using SSB.Service.SSBApi.Extentions;
using SSB.Service.SSBApi.Models;
using SSB.Service.SSBApi.Validation;
using System;
using System.Linq;
using System.Web.Http;
using static SSB.Service.SSBApi.Constant.SSBConstant;
using HttpPostAttribute = System.Web.Mvc.HttpPostAttribute;
namespace SSB.Service.SSBApi.Controllers
{
    [System.Web.Mvc.Route("api/SendSMS")]
    public class SendSMSController : BaseController
    {
        #region props
        #endregion
        #region ctors

        public SendSMSController()
        {
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
        public IHttpActionResult Send([FromBody] SendVM sendVM)
        {
            sendVM.FromNumber = Helpers.Utility.FixPhoneNumber(sendVM.FromNumber);

            var validate = _lineNumberValidation.LineValidation(new string[] { sendVM.FromNumber }, _username);
            if (!string.IsNullOrEmpty(validate))
                return Ok(new SendSMSDto() { Code = validate });

            var sms = Enumerable.Repeat(sendVM.Message, sendVM.ToNumber.Length).ToArray();
            return SendTo_SSB_SendSMS(sms, sendVM.FromNumber, sendVM.ToNumber, new long[0], new string[0]);

        }
        [HttpPost]
        public IHttpActionResult SendWithCheckinId([FromBody] SendWithCheckinIdVM sendWithCheckinIdVM)
        {
            sendWithCheckinIdVM.FromNumber = Helpers.Utility.FixPhoneNumber(sendWithCheckinIdVM.FromNumber);
            var validate = _lineNumberValidation.LineValidation(new string[] { sendWithCheckinIdVM.FromNumber }, _username);
            if (!string.IsNullOrEmpty(validate))
                return Ok(new SendSMSDto() { Code = validate });
            if (sendWithCheckinIdVM.ToNumbers.Length != sendWithCheckinIdVM.CheckingMessageId.Length)
                return Ok(new SendSMSDto() { Code = SSBErrorCode.CHECKINGID_EQUALCOUNT_MOBILES.ToString(), Message = " تعداد شناسه ارسالی با تعداد گیرنده برابر نمیباشد." });

            return SendTo_SSB_SendSMS(sendWithCheckinIdVM.Messages, sendWithCheckinIdVM.FromNumber, sendWithCheckinIdVM.ToNumbers, sendWithCheckinIdVM.CheckingMessageId, new string[0]);
        }
        public IHttpActionResult ArraySend([FromBody] ArraySendVM arraySendVM)
        {
            arraySendVM.FromNumber = Helpers.Utility.FixPhoneNumber(arraySendVM.FromNumber);
            var validate = _lineNumberValidation.LineValidation(new string[] { arraySendVM.FromNumber }, _username);
            if (!string.IsNullOrEmpty(validate))
                return Ok(new SendSMSDto() { Code = validate });
            if (arraySendVM.ToNumbers.Length != arraySendVM.Messages.Length)
                return Ok(new SendSMSDto() { Code = SSBErrorCode.SMS_EQUALCOUNT_MOBILE.ToString(), Message = "تعدا پیام با تعداد موبایل برابر نمی باشد" });
            return SendTo_SSB_SendSMS(arraySendVM.Messages, arraySendVM.FromNumber, arraySendVM.ToNumbers, new long[0],new string[0], sendToMagfa: true);

        }
        public IHttpActionResult SendWithUdh([FromBody] SendWithUdhVM sendWithUdhVM) {
            sendWithUdhVM.FromNumber = Helpers.Utility.FixPhoneNumber(sendWithUdhVM.FromNumber);
            var validate = _lineNumberValidation.LineValidation(new string[] { sendWithUdhVM.FromNumber }, _username);
            if (!string.IsNullOrEmpty(validate))
                return Ok(new SendSMSDto() { Code = validate });
            var sms = Enumerable.Repeat(sendWithUdhVM.Message, sendWithUdhVM.ToNumber.Length).ToArray();
            return SendTo_SSB_SendSMS(sms, sendWithUdhVM.FromNumber, sendWithUdhVM.ToNumber, new long[0], sendWithUdhVM.Udh
                );

        }
        #endregion
        #region private methods
        private IHttpActionResult SendTo_SSB_SendSMS(string[] message, string fromNumber, string[] toNumber,
            long[] chkId, string[] udhs, bool sendToMagfa = false)
        {
            int length = toNumber.Length;
            var sms = new string[length];
            var encods = Enumerable.Repeat(1, length).ToArray();
            var mclass = Enumerable.Repeat(1, length).ToArray();
            var priority = Enumerable.Repeat(-1, length).ToArray();
            var origis = Enumerable.Repeat(fromNumber, length).ToArray();
            string[] udh = Enumerable.Repeat("", length).ToArray();
            long[] checkingIds = new long[length];

            Random rnd = new Random();
            for (int i = 0; i < length; i++)
            {
                sms[i] = message[i];
                checkingIds[i] = rnd.Next();
            }

            if (chkId.Count() == toNumber.Count())
                checkingIds = chkId;
            if (udhs.Count() > 0)
                udh = udhs;
            if (sendToMagfa) return Ok(SSB_SendSMSArrayToMagfa(sms, encods, toNumber, origis, udh, mclass, priority, checkingIds, _username));
            return Ok(SSB_SendSMS(sms, encods, toNumber, origis, udh, mclass, priority, checkingIds, _username));

        }
        #endregion
    }
}