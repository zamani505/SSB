
namespace UseSSBApi.Models
{
    public class LoginModel
    {
        public string SSBToken { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Code { get; set; } = "0";
    }
}
