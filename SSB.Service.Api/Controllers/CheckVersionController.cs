using SSB.Service.SSBApi.Extentions;
using SSB.Service.SSBApi.Models;
using System;
using System.Web.Http;
using static SSB.Service.SSBApi.Constant.SSBConstant;
using HttpPostAttribute = System.Web.Mvc.HttpPostAttribute;
namespace SSB.Service.SSBApi.Controllers
{
    [System.Web.Mvc.Route("v1/api/CheckVersion")]
    public class CheckVersionController : BaseController
    {
        #region public methods
        [HttpPost]
        public IHttpActionResult CheckVersion() {
            try
            {
                string ln = string.Empty;
                string vr = string.Empty;
                var lns = _service.GetLineNumbers(_username);
                vr = _service.CheckVersion(_username);
                if (lns != null && lns.Count > 0)
                {
                    for (int i = 0; i < lns.Count; i++)
                        ln += lns[i].ILineNumber + ",";
                    ln = ln.Substring(0, ln.Length - 1);
                }
                return Ok(new CheckVersionDto() { Result=new CheckVersionDtoModel() { Version=vr,LineNumbers=ln} });

            }
            catch (Exception)
            {

                return Ok(new CheckVersionDto() { Code = SSBErrorCode.EXCEPTION.ToString(), Message = "متاسفانه مشکلی بوجود آمده است" });
            }
        }
        #endregion
    }
}