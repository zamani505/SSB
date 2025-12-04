
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using RoutePrefixAttribute = System.Web.Http.RoutePrefixAttribute;

namespace SSB.Service.SSBApi.Controllers
{
    [RoutePrefix("api/SSBSMS")]
    public class SSBSMSController : ApiController
    {
        [HttpGet, Route("Login")]
        public IHttpActionResult Login()
        {
            return Ok("ok");
        }

        
    }
}