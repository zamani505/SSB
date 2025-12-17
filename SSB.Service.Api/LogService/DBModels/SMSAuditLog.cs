using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace SSB.Service.SSBApi.LogService
{
    public class SMSAuditLog
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string ServiceName { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public DateTime CreateTime { get; set; }
        public int? ExcutionTime { get; set; } 
        public string Ip { get; set; }
        public string Exception { get; set; }

        public static SMSAuditLog New(string username, string serviceName, string request,string ip)
            => new SMSAuditLog() { Username=username,ServiceName=serviceName,Request=request,CreateTime=DateTime.Now,Ip=ip};

        public static SMSAuditLog Update(string response, int excutionTime, string exception,long id)
           => new SMSAuditLog() { Response = response,ExcutionTime = excutionTime, Exception =exception,Id=id };
    }
}