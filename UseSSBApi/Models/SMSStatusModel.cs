
namespace UseSSBApi.Models
{
    internal class SMSStatusModel
    {
        public int[] Result { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Code { get; set; } = "0";
    }
}
