
namespace SSB.Service.SSBApi.Models
{
    public class ArraySendVM
    {
        public string[] Messages { get; set; }
        public string FromNumber { get; set; }
        public string[] ToNumbers { get; set; }
    }
}