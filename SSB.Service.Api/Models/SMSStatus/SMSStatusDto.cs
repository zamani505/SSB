namespace SSB.Service.SSBApi.Models
{
    public class SMSStatusDto
    {
        public int[] Result { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Code { get; set; } = "0";
    }
}