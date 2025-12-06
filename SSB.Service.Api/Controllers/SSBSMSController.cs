using SSB.Service.Core;
using SSB.Service.SSBApi.CacheManager.Login;
using SSB.Service.SSBApi.Constant;
using SSB.Service.SSBApi.Extentions;
using SSB.Service.SSBApi.Models.ArraySendQeue;
using SSB.Service.SSBApi.Models.Login;
using SSB.Service.SSBApi.Models.SendFromUrl;
using SSB.Service.SSBApi.Models.SendPostUrl;
using SSB.Service.SSBApi.Validation;
using SSB.Service.Web.avanak;
using System.Linq;
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
        #endregion
        #region ctors

        public SSBSMSController()
        {
            _cacheLogin = new CacheLogin();
            _service = new SMSService();
            _lineNumberValidation=new LineNumerValidation();

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
            var username = _cacheLogin.GetUsername(HttpContext.Current.Request.Headers[SSBConstant.TOKEN_NAME]);
            var validate = _lineNumberValidation.LineValidation(new string[] { sendFromUrlVM.FromNumber }, username);
            if (!string.IsNullOrEmpty(validate))
                return Ok(new SendFromUrlDto() { Code = validate });

            string[] toNumbers = sendFromUrlVM.ToNumber.Split(',');
            var res= _service.SendSMSQueue(sendFromUrlVM.Message, toNumbers, sendFromUrlVM.FromNumber, username);
            return Ok(new SendFromUrlDto() { Result = res });
        }
        [HttpPost]
        public IHttpActionResult SendPostUrl([FromBody] SendPostUrlVM sendPostUrlVM) {
            sendPostUrlVM.From = Helpers.Utility.FixPhoneNumber(sendPostUrlVM.From);
            var username = _cacheLogin.GetUsername(HttpContext.Current.Request.Headers[SSBConstant.TOKEN_NAME]);
            var validate = _lineNumberValidation.LineValidation(new string[] { sendPostUrlVM.From }, username);
            if (!string.IsNullOrEmpty(validate))
                return Ok(new SendPostUrlDto() { Code = validate });

            string[] toNumbers = sendPostUrlVM.To.Split(',');
            var res = _service.SendSMSQueue(sendPostUrlVM.Message, toNumbers, sendPostUrlVM.From, username);
            return Ok(new SendPostUrlDto() { Result = res });
        }
        [HttpPost]
        public IHttpActionResult ArraySendQeue([FromBody] ArraySendQeueVM arraySendQeueVM) {
            for (int i = 0; i < arraySendQeueVM.SenderNumbers.Count(); i++)
                arraySendQeueVM.SenderNumbers[i] = Helpers.Utility.FixPhoneNumber(arraySendQeueVM.SenderNumbers[i]);
            var username = _cacheLogin.GetUsername(HttpContext.Current.Request.Headers[SSBConstant.TOKEN_NAME]);
            var validate = _lineNumberValidation.LineValidation(arraySendQeueVM.SenderNumbers, username);
            if (!string.IsNullOrEmpty(validate))
                return Ok(new ArraySendQeueDto() { Code = validate });

            var res = _service.SendSMSQueue(arraySendQeueVM.Messages, arraySendQeueVM.Mobiles, arraySendQeueVM.SenderNumbers, username);
            return Ok(new ArraySendQeueDto() { Result = res });
        }
        #endregion
    }
}