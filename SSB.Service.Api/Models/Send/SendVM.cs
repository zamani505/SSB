
namespace SSB.Service.SSBApi.Models.Send
{
    public class SendVM
    {
        public string Message { get; set; }
        public string FromNumber { get; set; }
        public string[] ToNumber { get; set; }
    }
}