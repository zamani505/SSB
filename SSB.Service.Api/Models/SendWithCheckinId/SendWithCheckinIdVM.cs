
namespace SSB.Service.SSBApi.Models
{
    public class SendWithCheckinIdVM
    {
        public string[] Messages { get; set; }
        public string FromNumber { get; set; }
        public string[] ToNumbers { get; set; }
        public long[] CheckingMessageId { get; set; }
    }
}