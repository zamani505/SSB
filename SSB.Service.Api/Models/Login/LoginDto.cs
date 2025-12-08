
namespace SSB.Service.SSBApi.Models
{
    public class LoginDto
    {
        public string SSBToken { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Code { get; set; } = "0";
    }
}