
namespace SSB.Service.SSBApi.Models.SendWithUdh
{
    public class SendWithUdhVM
    {
        public string Message { get; set; }
        public string FromNumber { get; set; }
        public string[] ToNumber { get; set; }
        public string[] Udh { get; set; }
    }
}