using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseSSBApi.Models
{
    internal class RecieveModel
    {
        public List<RecieveSMSModel> Result { get; set; }
        public string Message { get; set; }
        public string Code { get; set; } = "0";
    }
    public class RecieveSMSModel
    {
        public string RcvSmsText { get; set; }
        public string RcvsmsKeyWord { get; set; }
        public string RcvSmsfrom { get; set; }
        public string RcvSmsTo { get; set; }
        public string RcvSmsInteredDate { get; set; }
        public string RcvSmsUDH { get; set; }
        public string RcvsmsCharSet { get; set; }
        public string rcvSmsSmsC { get; set; }
        public string RcvSmsStatus { get; set; }
        public string RcvSmsMessageID { get; set; }
        public string RcvSmsRuleID { get; set; }
        public string RcvSmsDeliveredTime { get; set; }
        public bool RcvSmsReeded { get; set; }
        public string UsrId { get; set; }
        public int RecieveSMSId { get; set; }
        public int OperatorId { get; set; }
        public DateTime RecieveDate { get; set; }
    }
}
