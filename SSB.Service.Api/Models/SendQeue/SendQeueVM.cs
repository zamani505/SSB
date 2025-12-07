
namespace SSB.Service.SSBApi.Models
{
    public class SendQeueVM
    {
        public string Message { get; set; }
        public string FromNumber { get; set; }
        public string[] ToNumber { get; set; }
    }
}