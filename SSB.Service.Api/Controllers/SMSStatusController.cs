
using SSB.Service.SSBApi.Constant;
using SSB.Service.SSBApi.Extentions;
using SSB.Service.SSBApi.Models;

using System.Linq;
using System.Web;
using System.Web.Http;
using static SSB.Service.SSBApi.Constant.SSBConstant;
using HttpPostAttribute = System.Web.Mvc.HttpPostAttribute;
namespace SSB.Service.SSBApi.Controllers
{
    [System.Web.Mvc.Route("api/SMSStatus")]
    public class SMSStatusController : BaseController
    {
        #region props

        public readonly string _username;
        #endregion
        #region ctors

        public SMSStatusController()
        {
            _username = _cacheLogin.GetUsername(HttpContext.Current.Request.Headers[SSBConstant.TOKEN_NAME]);
        }
        #endregion
        #region public methods
        [HttpPost]
        public IHttpActionResult GetArrayMessageStatus([FromBody] GetArrayMessageStatusVM getArrayMessageStatusVM)
        {
            if (getArrayMessageStatusVM.Ids.Count() > 100)
                return Ok(new SMSStatusDto() { Code = SSBErrorCode.COUNT_OF_GETSTATUS.ToString(), Message = " در هربار فقط 100 پیامک فراخوانی میشود" });
            return Ok(SSB_SMSStatus(getArrayMessageStatusVM.Ids, fromMafa: true));

        }
        [HttpPost]
        public IHttpActionResult GetMessageStatus([FromBody] GetMessageStatusVM getMessageStatusVM) {
            if (getMessageStatusVM.Ids.Count() > 100)
                return Ok(new SMSStatusDto() { Code = SSBErrorCode.COUNT_OF_GETSTATUS.ToString(), Message = " در هربار فقط 100 پیامک فراخوانی میشود" });
            return Ok(SSB_SMSStatus(getMessageStatusVM.Ids));
        }
        [HttpPost]
        public IHttpActionResult GetQueueMessageStatus([FromBody] GetQueueMessageStatusVM getQueueMessageStatusVM) {
            if (getQueueMessageStatusVM.Ids.Count() > 100)
                return Ok(new SMSStatusDto() { Code = SSBErrorCode.COUNT_OF_GETSTATUS.ToString(), Message = " در هربار فقط 100 پیامک فراخوانی میشود" });
            return Ok(SSB_SMSStatus(getQueueMessageStatusVM.Ids));
        }
        #endregion
        #region private methods
        #endregion
    }
}