using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using SSB.Service.SSBApi.CacheManager.Login;
using SSB.Service.SSBApi.Constant;
using SSB.Service.SSBApi.Models;
using SSB.Service.SSBApi.Extentions;

namespace SSB.Service.SSBApi.Attribute
{
    public class ApiKeyHandler : DelegatingHandler
    {
        private static readonly Dictionary<string, Type> VerbTypes = new Dictionary<string, Type>
        {
            { "send", typeof(SendSMSDto) },
            { "sendwithcheckinId", typeof(SendSMSDto) },
            { "arraysend", typeof(SendSMSDto) },
            { "sendwithUdh", typeof(SendSMSDto) },
            { "sendfromUrl", typeof(SMSDto) },
            { "sendpostUrl", typeof(SMSDto) },
            { "arraysendQeue", typeof(SMSDto) },
            { "arraysendQeueWithId", typeof(SMSDto) },
            { "sendqeue", typeof(SMSDto) },
            
        };
        protected override async Task<HttpResponseMessage> SendAsync(
       HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if(request.RequestUri.AbsolutePath.ToLower().Equals("/api/ssbsms/login"))
                return await base.SendAsync(request, cancellationToken);
            if (!request.Headers.Contains(SSBConstant.TOKEN_NAME))
                return request.CreateResponse(HttpStatusCode.Unauthorized, GetBody(HttpStatusCode.Unauthorized, request.RequestUri.AbsolutePath.GetVerbName()));
            var key = request.Headers.GetValues(SSBConstant.TOKEN_NAME).FirstOrDefault();
            var exist=new CacheLogin().HaveSession(key);
            if (!exist)
                return request.CreateResponse(HttpStatusCode.Forbidden, GetBody(HttpStatusCode.Unauthorized, request.RequestUri.AbsolutePath.GetVerbName()));
            
            return await base.SendAsync(request, cancellationToken);
        }
        private object GetBody(HttpStatusCode statusCode,string verbName) 
        {
            VerbTypes.TryGetValue(verbName, out Type targetType);
            object instance = Activator.CreateInstance(targetType);
            if(statusCode== HttpStatusCode.Unauthorized)
            targetType.GetProperty("Message").SetValue(instance, SSBConstant.UNAUTHORIZED_MESSAGE);
            else if (statusCode == HttpStatusCode.Forbidden)
                targetType.GetProperty("Message").SetValue(instance, SSBConstant.INVALID_TOKEN_MESSAGE);
            targetType.GetProperty("Code").SetValue(instance,statusCode.ToString());
            return instance ;
        }
    }
}