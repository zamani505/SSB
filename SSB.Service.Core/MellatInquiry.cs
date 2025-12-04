using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using SSB.Service.Core.ir.bankmellat.bpm.pgws;

namespace SSB.Service.Core
{
    public class MellatInquiry
    {
        private static PaymentGatewayImplService _MellatService;

        public static PaymentGatewayImplService MellatService
        {
            get
            {
                if (_MellatService == null)
                {
                    _MellatService = new PaymentGatewayImplService();
                }
                return _MellatService;
            }
            set { _MellatService = value; }
        }

        public static bool IsPaid(MellatAcount mellatAcount)
        {
            string Result = string.Empty;
            Result = MellatService.bpInquiryRequest(
                long.Parse(mellatAcount.TerminalID),
                mellatAcount.UserName,
                mellatAcount.Pass,
                long.Parse(mellatAcount.OrderID),
                long.Parse(mellatAcount.OrderID),
                long.Parse(mellatAcount.TransactionNumber)
                );
            return Result == "0" ? true : false;
        }
    }
    public class MellatAcount
    {
        public string TerminalID
        {
            get
            {
                AppSettingsReader apr = new AppSettingsReader();
                return apr.GetValue("MellatTerminalID",typeof(string)).ToString();
            }
        }

        public string UserName
        {
            get
            {
                AppSettingsReader apr = new AppSettingsReader();
                return apr.GetValue("MellatUserName", typeof(string)).ToString();
            }
        }
        public string Pass
        {
            get
            {
                AppSettingsReader apr = new AppSettingsReader();
                return apr.GetValue("MellatPassword", typeof(string)).ToString();
            }
        }
        public string OrderID { get; set; }
        public string TransactionNumber { get; set; }
    }
}
