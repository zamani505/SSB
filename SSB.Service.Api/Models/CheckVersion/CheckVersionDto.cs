
namespace SSB.Service.SSBApi.Models
{
    public class CheckVersionDto
    {
        public CheckVersionDtoModel Result { get; set; }
        public string Message { get; set; }
        public string Code { get; set; }
    }
    public class CheckVersionDtoModel {
        public string  Version { get; set; }
        public string  LineNumbers { get; set; }
    }
}