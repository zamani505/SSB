using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using SSB.Service.SSBApi.CacheManager.Login;
using SSB.Service.SSBApi.Constant;

namespace SSB.Service.SSBApi.Attribute
{
    public class ApiKeyHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
       HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if(request.RequestUri.AbsolutePath.ToLower().Equals("/api/ssbsms/login"))
                return await base.SendAsync(request, cancellationToken);
            if (!request.Headers.Contains(SSBConstant.TOKEN_NAME))
                return request.CreateResponse(HttpStatusCode.Unauthorized, new { error = "لطفا ابتدا Login کنید!" });
            var key = request.Headers.GetValues(SSBConstant.TOKEN_NAME).FirstOrDefault();
            var exist=new CacheLogin().HaveSession(key);
            if (!exist)
                return request.CreateResponse(HttpStatusCode.Forbidden, new { error = "توکن ارسالی نامعتبر می باشد" });
            
            return await base.SendAsync(request, cancellationToken);
        }
    }
}