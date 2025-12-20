using Newtonsoft.Json;
using SSB.Service.SSBApi.Constant;
using SSB.Service.SSBApi.Models;
using SSB.Service.SSBApi.Models.GetTarrifOperator;
using System.Linq;
using System.Web;
using System.Web.Http;
using static SSB.Service.SSBApi.Constant.SSBConstant;
using HttpPostAttribute = System.Web.Mvc.HttpPostAttribute;
namespace SSB.Service.SSBApi.Controllers
{
    [System.Web.Mvc.Route("v1/api/Credit")]
    public class CreditController : BaseController
    {
        #region props

       
        #endregion
        #region ctors

        public CreditController()
        {
           
        }
        #endregion
        #region public methods
        [HttpPost]
        public IHttpActionResult UserCredit()
        {
            try
            {
                return Ok(new CreditDto() { Credit = _service.GetCurrentCredit(string.Empty, _username) });
            }
            catch (System.Exception ex)
            {
                var resp = Ok(new CreditDto() { Code = SSBErrorCode.EXCEPTION.ToString(), Message = "متاسفانه مشکلی بوجود آمده است" });
                _logService.UpdateLog(JsonConvert.SerializeObject(resp), _cacheKey, ex);
                return resp;
                
            }
        }
        [HttpPost]
        public IHttpActionResult CheckCredit([FromBody] CheckCreditVM checkCreditVM)
        {
            try
            {
                return Ok(new CreditDto() { Credit = _service.CheckCredit(checkCreditVM.SMSFaCount, checkCreditVM.SMSEnCount, _username) });

            }
            catch (System.Exception ex)
            {

                var resp = Ok(new CreditDto() { Code = SSBErrorCode.EXCEPTION.ToString(), Message = "متاسفانه مشکلی بوجود آمده است" });
                _logService.UpdateLog(JsonConvert.SerializeObject(resp), _cacheKey, ex);
                return resp;

            }
        }
        [HttpPost]
        public IHttpActionResult GetTarrifOperator([FromBody] GetTarrifOperatorVM getTarrifOperatorVM)
        {
            try
            {
                return Ok(new GetTarrifOperatorDto() { Tarefe = _service.GetTarrifrateRate(getTarrifOperatorVM.Linenumber) });
            }
            catch (System.Exception ex)
            {
                var resp = Ok(new GetTarrifOperatorDto() { Code = SSBErrorCode.EXCEPTION.ToString(), Message = "متاسفانه مشکلی بوجود آمده است" });
                _logService.UpdateLog(JsonConvert.SerializeObject(resp), _cacheKey, ex);
                return resp;
               

            }
        }
        #endregion
    }
}