
namespace SSB.Service.SSBApi.Constant
{
    public class SSBConstant
    {
        public const string TOKEN_NAME = "SSBToken";
        public const string UNAUTHORIZED_MESSAGE = "لطفا ابتدا فرایند lOGIN و دریافت توکن را انجام دهید";
        public const string INVALID_TOKEN_MESSAGE = "توکن ارسالی نا معتبر می باشد";

        //--------------Error Code-----------------
        public enum SSBErrorCode
        {
            SMS_EQUALCOUNT_MOBILE = 101,
            CHECKINGID_EQUALCOUNT_MOBILES = 102,
            COUNT_OF_GETSTATUS = 100,
            EXCEPTION=-1
        }
    }
}