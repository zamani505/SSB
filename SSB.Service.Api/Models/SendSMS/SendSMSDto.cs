
namespace SSB.Service.SSBApi.Models
{
    public class SendSMSDto
    {
        public long[] Result { get; set; }
        public string Message { get; set; }
        public string Code { get; set; } = "0";
    }
}