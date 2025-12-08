
namespace SSB.Service.SSBApi.Models
{
    public class ArraySendQeueDto
    {
        public string[] Result { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Code { get; set; } = "0";
    }
}