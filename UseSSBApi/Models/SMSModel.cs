

namespace UseSSBApi.Models
{
    internal class SMSModel
    {
        public string[] Result { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Code { get; set; } = "0";

    }
    internal class SMSModel2
    {
        public long[] Result { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Code { get; set; } = "0";

    }
}
