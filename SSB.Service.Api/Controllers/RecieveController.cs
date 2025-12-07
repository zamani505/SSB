using SSB.Service.Core;
using SSB.Service.SSBApi.Constant;
using SSB.Service.SSBApi.Extentions;
using SSB.Service.SSBApi.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;
using static SSB.Service.SSBApi.Constant.SSBConstant;
using HttpPostAttribute = System.Web.Mvc.HttpPostAttribute;

namespace SSB.Service.SSBApi.Controllers
{
    [System.Web.Mvc.Route("api/Recieve")]
    public class RecieveController : BaseController
    {
        #region props
        
        #endregion
        #region public methods
        [HttpPost]
        public IHttpActionResult RecieveSMS([FromBody] RecieveSMSVM recieveSMSVM)
        {
            try
            {
                var validate = _lineNumberValidation.LineValidation(new string[] { recieveSMSVM.PhNo }, _username);
                if (!string.IsNullOrEmpty(validate))
                    return Ok(new RecieveDto() { Code = validate });
                var tblRec = _service.RecieveSMS(recieveSMSVM.StartDate, recieveSMSVM.EndDate, recieveSMSVM.PhNo);
                return Ok(new RecieveDto() { Result = tblRec.ToRecieveSMSModel() });
            }
            catch (Exception ex)
            {
                return Ok(new RecieveDto() { Code = SSBErrorCode.EXCEPTION.ToString(), Message = "متاسفانه مشکلی بوجود آمده است" });
            }
        }
        [HttpPost]
        public IHttpActionResult RecieveSMSById([FromBody] RecieveSMSByIdVM recieveSMSByIdVM)
        {
            try
            {
                var validate = _lineNumberValidation.LineValidation(new string[] { recieveSMSByIdVM.PhNo }, _username);
                if (!string.IsNullOrEmpty(validate))
                    return Ok(new RecieveDto() { Code = validate });
                var tblRec = _service.RecieveSMSById(recieveSMSByIdVM.Id, recieveSMSByIdVM.PhNo);
                return Ok(new RecieveDto() { Result = tblRec.ToRecieveSMSModel() });
            }
            catch (Exception)
            {

                return Ok(new RecieveDto() { Code = SSBErrorCode.EXCEPTION.ToString(), Message = "متاسفانه مشکلی بوجود آمده است" });
            }
           
        }
        #endregion
    }
}