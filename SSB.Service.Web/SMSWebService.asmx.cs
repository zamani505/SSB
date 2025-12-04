using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using SSB.Service.Core;

namespace SSB.Service.Web
{
    /// <summary>
    /// وب سرویس شرکت سهند سامانه برتر جهت ارسال پیامک
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class SMSWebService : System.Web.Services.WebService
    {
        [WebMethod]
        [SoapHeader("Auth", Required = true)]
        public string[] SendQeue(string message, string fromNumber, string[] toNumbers)
        {
            try
            {
                fromNumber = Helpers.Utility.FixPhoneNumber(fromNumber);
                SMSService service = new SMSService();
                if (Auth == null)
                    throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
                List<UserInfo> info = service.Authenticate(Auth.Username, Auth.Password).ToList();
                if (info == null || info.Count == 0)
                    return new string[1] { "18" };
                if (!info[0].IUsrActive.GetValueOrDefault(false))
                    return new string[1] { "16" };
                if (!service.CheckIsLinenumberOwner(new string[] { fromNumber }, Auth.Username))
                    return new string[1] { "20" };
                return service.SendSMSQueue(message, toNumbers, fromNumber, Auth.Username);
            }
            catch (Exception ex)
            {
                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\err.txt", true))
                {
                    outfile.Write("from SmsWebserivce::" + DateTime.Now.ToString() + "::" + ex.Message + "::" + ex.StackTrace + Environment.NewLine);
                }
                return new string[1] { "Failed" };
            }

        }

        [WebMethod]
        [SoapHeader("Auth", Required = true)]
        public string[] ArraySendQeue(string[] Messages, string[] Mobiles, string[] senderNumbers)
        {
            try
            {

                for (int i = 0; i < senderNumbers.Count(); i++)
                {
                    senderNumbers[i] = Helpers.Utility.FixPhoneNumber(senderNumbers[i]);
                }

                SMSService service = new SMSService();
                if (Auth == null)
                    throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
                List<UserInfo> info = service.Authenticate(Auth.Username, Auth.Password).ToList();
                if (info == null || info.Count == 0)
                    return new string[1] { "18" };
                if (!info[0].IUsrActive.GetValueOrDefault(false))
                    return new string[1] { "16" };
                if (!service.CheckIsLinenumberOwner(senderNumbers, Auth.Username))
                    return new string[1] { "20" };

                return service.SendSMSQueue(Messages, Mobiles, senderNumbers, Auth.Username);
            }
            catch (Exception ex)
            {
                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\err.txt", true))
                {
                    outfile.Write("from SmsWebserivce::" + DateTime.Now.ToString() + "::" + ex.Message + "::" + ex.StackTrace + Environment.NewLine);
                }
                throw;
            }
        }


        public Authentication Auth;


        [WebMethod]
        [SoapHeader("Auth", Required = true)]
        public long[] Send(string[] Messages, int[] Encodings, string[] Mobiles, string[] senderNumbers, string[] UDH, int[] MessageClass, int[] Priorities, long[] CheckingIds)
        {
            SMSService service = new SMSService();
            AppSettingsReader rdr = new AppSettingsReader();
            for (int i = 0; i < senderNumbers.Length; i++)
            {
                senderNumbers[i] = Helpers.Utility.FixPhoneNumber(senderNumbers[i]);
            }
            try
            {

            if (Auth == null)
                throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
            List<UserInfo> info = service.Authenticate(Auth.Username, Auth.Password).ToList();
            if (info == null || info.Count == 0)
                return new long[1] { (long)18 };
            if (!info[0].IUsrActive.GetValueOrDefault(false))
                return new long[1] { (long)16 };
            if (!service.CheckIsLinenumberOwner(senderNumbers, Auth.Username))
                return new long[1] { (long)101 };


            return service.SendSMS(Messages, Encodings, Mobiles, senderNumbers, UDH, MessageClass, Priorities, CheckingIds, Auth.Username);
            }
            catch (Exception ex)
            {
                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\err.txt", true))
                {
                    outfile.Write("from SmsWebserivce::" + DateTime.Now.ToString() + "::" + ex.Message + "::" + ex.StackTrace + Environment.NewLine);
                }
                throw;
            }
        }

        [WebMethod]
        [SoapHeader("Auth", Required = true)]
        public long SendOneMessage(string Message, int Encoding, string Mobile, string senderNumber, string UDH, int MessageClass, int Prioritie, long CheckingId)
        {
            senderNumber = Helpers.Utility.FixPhoneNumber(senderNumber);
            long[] res = Send(new string[1] { Message }, new int[] { Encoding }, new string[] { Mobile },
                                  new string[] { senderNumber }, new string[] { UDH }, new int[] { MessageClass },
                                  new int[] { Prioritie }, new long[] { CheckingId });
            if (res == null || res.Length == 0)
                return 0;
            return res[0];
        }


        [WebMethod]
        [SoapHeader("Auth", Required = true)]
        public int GetCredit()
        {
            try
            {
                SMSService srv = new SMSService();
                if (Auth == null)
                    throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
                List<UserInfo> info = srv.Authenticate(Auth.Username, Auth.Password).ToList();
                if (info == null || info.Count == 0)
                    return -18;
                if (!info[0].IUsrActive.GetValueOrDefault(false))
                    return -16;
                return srv.GetCurrentCredit(string.Empty, Auth.Username);

            }
            catch (Exception ex)
            {
                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\err.txt", true))
                {
                    outfile.Write("from SmsWebserivce::" + DateTime.Now.ToString() + "::" + ex.Message + "::" + ex.StackTrace + Environment.NewLine);
                }

                throw;
            }
        }


        [WebMethod]
        [SoapHeader("Auth", Required = true)]
        public DataTable RecieveSMS(string phNo, string startdate, string enddate)
        {
            phNo = Helpers.Utility.FixPhoneNumber(phNo);
            SMSService srv = new SMSService();
            if (Auth == null)
                throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
            List<UserInfo> info = srv.Authenticate(Auth.Username, Auth.Password).ToList();
            if (info == null || info.Count == 0)
                throw new Exception(" نام کاربری و رمز عبور صحیح نمی باشد.");

            if (!info[0].IUsrActive.GetValueOrDefault(false))
                throw new Exception(" کاربر غیر فعال می باشد.");
            List<Tbl_RecieveSms> list = srv.RecieveSMS(startdate, enddate, phNo);
            DataTable dt = GetTblRcvSchema();

            foreach (var rcvs in list)
            {
                DataRow dr = dt.NewRow();
                dr["RcvSmsText"] = rcvs.RcvSmsText;
                dr["RcvsmsKeyWord"] = rcvs.RcvsmsKeyWord;
                dr["RcvSmsfrom"] = rcvs.RcvSmsfrom;
                dr["RcvSmsTo"] = rcvs.RcvSmsTo;
                dr["RcvSmsInteredDate"] = rcvs.RcvSmsInteredDate;
                dr["RcvSmsUDH"] = rcvs.RcvSmsUDH;
                dr["RcvsmsCharSet"] = rcvs.RcvsmsCharSet;
                dr["rcvSmsSmsC"] = rcvs.rcvSmsSmsC;
                dr["RcvSmsStatus"] = rcvs.RcvSmsStatus;
                dr["RcvSmsMessageID"] = rcvs.RcvSmsMessageID;
                dr["RcvSmsRuleID"] = rcvs.RcvSmsRuleID;
                dr["RcvSmsDeliveredTime"] = rcvs.RcvSmsDeliveredTime;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        [WebMethod]
        [SoapHeader("Auth", Required = true)]
        public DataTable RecieveUnreadSMS(string phNo, string startdate, string enddate)
        {
            phNo = Helpers.Utility.FixPhoneNumber(phNo);
            SMSService srv = new SMSService();
            if (Auth == null)
                throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
            List<UserInfo> info = srv.Authenticate(Auth.Username, Auth.Password).ToList();
            if (info == null || info.Count == 0)
                throw new Exception(" نام کاربری و رمز عبور صحیح نمی باشد.");

            if (!info[0].IUsrActive.GetValueOrDefault(false))
                throw new Exception(" کاربر غیر فعال می باشد.");
            List<Tbl_RecieveSms> list = srv.RecieveUnreadSMS(startdate, enddate, phNo);
            DataTable dt = GetTblRcvSchema();

            foreach (var rcvs in list)
            {
                DataRow dr = dt.NewRow();
                dr["RcvSmsText"] = rcvs.RcvSmsText;
                dr["RcvsmsKeyWord"] = rcvs.RcvsmsKeyWord;
                dr["RcvSmsfrom"] = rcvs.RcvSmsfrom;
                dr["RcvSmsTo"] = rcvs.RcvSmsTo;
                dr["RcvSmsInteredDate"] = rcvs.RcvSmsInteredDate;
                dr["RcvSmsUDH"] = rcvs.RcvSmsUDH;
                dr["RcvsmsCharSet"] = rcvs.RcvsmsCharSet;
                dr["rcvSmsSmsC"] = rcvs.rcvSmsSmsC;
                dr["RcvSmsStatus"] = rcvs.RcvSmsStatus;
                dr["RcvSmsMessageID"] = rcvs.RcvSmsMessageID;
                dr["RcvSmsRuleID"] = rcvs.RcvSmsRuleID;
                dr["RcvSmsDeliveredTime"] = rcvs.RcvSmsDeliveredTime;
                dt.Rows.Add(dr);
            }
            return dt;
        }


        private DataTable GetTblRcvSchema()
        {
            DataTable dt = new DataTable("RecieveSMS");
            DataColumn dcRcvSmsText = new DataColumn("RcvSmsText", typeof(string));
            DataColumn dcRcvsmsKeyWord = new DataColumn("RcvsmsKeyWord", typeof(string));
            DataColumn dcRcvSmsfrom = new DataColumn("RcvSmsfrom", typeof(string));
            DataColumn dcRcvSmsTo = new DataColumn("RcvSmsTo", typeof(string));
            DataColumn dcRcvSmsInteredDate = new DataColumn("RcvSmsInteredDate", typeof(string));
            DataColumn dcRcvSmsUDH = new DataColumn("RcvSmsUDH", typeof(string));
            DataColumn dcRcvsmsCharSet = new DataColumn("RcvsmsCharSet", typeof(string));
            DataColumn dcrcvSmsSmsC = new DataColumn("rcvSmsSmsC", typeof(string));
            DataColumn dcRcvSmsStatus = new DataColumn("RcvSmsStatus", typeof(string));
            DataColumn dcRcvSmsMessageID = new DataColumn("RcvSmsMessageID", typeof(string));
            DataColumn dcRcvSmsRuleID = new DataColumn("RcvSmsRuleID", typeof(string));
            DataColumn dcRcvSmsDeliveredTime = new DataColumn("RcvSmsDeliveredTime", typeof(string));

            dt.Columns.AddRange(new DataColumn[] { dcRcvSmsDeliveredTime, dcRcvSmsInteredDate, dcRcvSmsMessageID, dcRcvSmsRuleID, dcRcvSmsText, dcRcvsmsKeyWord, dcRcvSmsfrom
            ,dcRcvSmsTo,dcRcvSmsUDH,dcRcvsmsCharSet,dcrcvSmsSmsC,dcRcvSmsStatus});
            return dt;
        }
        [WebMethod]
        public int[] GetStatuses(long[] messageIds)
        {
            SMSService srv = new SMSService();
            return srv.GetMessageStauses(messageIds);
        }


        [WebMethod]
        [SoapHeader("Auth", Required = true)]
        public int CheckCredit(int SmsFaCount, int SmsEnCount)
        {
            SMSService srv = new SMSService();
            if (Auth == null)
                throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
            List<UserInfo> info = srv.Authenticate(Auth.Username, Auth.Password).ToList();
            if (info == null || info.Count == 0)
                throw new Exception(" نام کاربری و رمز عبور صحیح نمی باشد.");
            return srv.CheckCredit(SmsFaCount, SmsEnCount, Auth.Username);
        }


        [WebMethod]
        [SoapHeader("Auth", Required = true)]
        public string[] CheckVersion()
        {
            SMSService srv = new SMSService();
            if (Auth == null)
                throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
            List<UserInfo> info = srv.Authenticate(Auth.Username, Auth.Password).ToList();
            //if (info == null || info.Count == 0)
            //    throw new Exception(" نام کاربری و رمز عبور صحیح نمی باشد.");

            //if (!info[0].IUsrActive.GetValueOrDefault(false))
            //    throw new Exception(" کاربر غیر فعال می باشد.");
            if (info == null || info.Count == 0)
                return new string[1] { "18" };
            if (!info[0].IUsrActive.GetValueOrDefault(false))
                return new string[1] { "16" };
            var lns = srv.GetLineNumbers(Auth.Username);
            var vr = srv.CheckVersion(Auth.Username);
            string ln = string.Empty;
            if (lns != null)
                ln = lns[0].ILineNumber;

            return new string[]{vr,ln};
        }

    }
}
