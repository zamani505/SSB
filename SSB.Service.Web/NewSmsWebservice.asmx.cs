using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml.Serialization;
using SSB.Service.Core;
using System.Text.RegularExpressions;
using System.Web.Script.Services;
using System.Web.Script.Serialization;

namespace SSB.Service.Web
{
    /// <summary>
    /// Summary description for NewSmsWebservice
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class NewSmsWebservice : System.Web.Services.WebService
    {
        [WebMethod]
        public string[] SendFromUrl(string username, string password, string message, string fromNumber, string toNumber)
        {
            try
            {
                fromNumber = Helpers.Utility.FixPhoneNumber(fromNumber);
                SMSService service = new SMSService();
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
                List<UserInfo> info = service.Authenticate(username, password).ToList();
                if (info == null || info.Count == 0)
                    return new string[1] { "18" };
                if (!info[0].IUsrActive.GetValueOrDefault(false))
                    return new string[1] { "16" };
                if (!service.CheckIsLinenumberOwner(new string[] { fromNumber }, username))
                    return new string[1] { "20" };
                string[] toNumbers = toNumber.Split(',');
                return service.SendSMSQueue(message, toNumbers, fromNumber, username);
            }
            catch (Exception ex)
            {
                //using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\SendFromUrl_Error_Log.txt", true))
                //{
                //    outfile.Write(DateTime.Now+ "  username:"+(string.IsNullOrEmpty(username)?"null":username)+ "  password:" + (string.IsNullOrEmpty(password) ? "null" : password)+
                //        "  message:" + (string.IsNullOrEmpty(message) ? "null" : message)+ "  fromNumber:" + (string.IsNullOrEmpty(fromNumber) ? "null" : fromNumber)+
                //        "  toNumber:" + (string.IsNullOrEmpty(toNumber) ? "null" : toNumber) + Environment.NewLine+ex.Message + "::" + ex.StackTrace + Environment.NewLine);
                //}
                return new string[1] { "Failed" };
            }

        }
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public string testResponse(string test)
        {
            return new JavaScriptSerializer().Serialize(test);
        }

        [WebMethod]
        public string[] SendPostUrl(string username, string password, string from, string to, string message)
        {
            try
            {

                from = Helpers.Utility.FixPhoneNumber(from);
                SMSService service = new SMSService();
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
                List<UserInfo> info = service.Authenticate(username, password).ToList();
                if (info == null || info.Count == 0)
                    return new string[1] { "18" };
                if (!info[0].IUsrActive.GetValueOrDefault(false))
                    return new string[1] { "16" };
                if (!service.CheckIsLinenumberOwner(new string[] { from }, username))
                    return new string[1] { "20" };
                string[] toNumbers = to.Split(',');
                return service.SendSMSQueue(message, toNumbers, from, username);
            }
            catch (Exception ex)
            {
                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\SendPostUrl.txt", true))
                {
                    outfile.Write("from NewSmsWebserivce_SendPostUrl::" + DateTime.Now.ToString() + " :: " + ex.Message + " :: " + ex.StackTrace + Environment.NewLine);
                }
                return new string[1] { "Failed" };
            }

        }
        [WebMethod]
        public string[] ArraySendQeue(string[] Messages, string[] Mobiles, string[] senderNumbers, string Username, string Password)
        {
            try
            {

                for (int i = 0; i < senderNumbers.Count(); i++)
                {
                    senderNumbers[i] = Helpers.Utility.FixPhoneNumber(senderNumbers[i]);
                }

                SMSService service = new SMSService();
                if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                    throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
                List<UserInfo> info = service.Authenticate(Username, Password).ToList();
                if (info == null || info.Count == 0)
                    return new string[1] { "18" };
                if (!info[0].IUsrActive.GetValueOrDefault(false))
                    return new string[1] { "16" };
                if (!service.CheckIsLinenumberOwner(senderNumbers, Username))
                    return new string[1] { "20" };
                //using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\ArraySendQeue_log.txt", true))
                //{
                //    outfile.Write("1" + Environment.NewLine);
                //}
                return service.SendSMSQueue(Messages, Mobiles, senderNumbers, Username);
            }
            catch (Exception ex)
            {
                if (senderNumbers[0] == "+98300083100" || senderNumbers[0] == "98300083100" || senderNumbers[0] == "300083100")
                {
                    string str = "";
                    for (int i = 0; i < Messages.Count(); i++)
                    {
                        str += "message[" + i.ToString() + "]:" + Messages[i] + "  Mobiles[" + i.ToString() + "]:" + Mobiles[i] + "   senderNumbers[" + i.ToString() + "]:" + senderNumbers[i] + Environment.NewLine;
                    }
                    using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\ArraySendQeue.txt", true))
                    {
                        outfile.Write("from NewSmsWebserivce::" + DateTime.Now.ToString() + Environment.NewLine + str + Environment.NewLine + "::" + ex.Message + "::" + ex.StackTrace + Environment.NewLine);
                    }
                }
                throw;
            }
        }

        [WebMethod]
        public string[] ArraySendQeueWithId(Guid[] Ids, string[] Messages, string[] Mobiles, string[] senderNumbers, string Username, string Password)
        {
            try
            {

                for (int i = 0; i < senderNumbers.Count(); i++)
                {
                    senderNumbers[i] = Helpers.Utility.FixPhoneNumber(senderNumbers[i]);
                }

                SMSService service = new SMSService();
                if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                    throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
                List<UserInfo> info = service.Authenticate(Username, Password).ToList();
                if (info == null || info.Count == 0)
                    return new string[1] { "18" };
                if (!info[0].IUsrActive.GetValueOrDefault(false))
                    return new string[1] { "16" };
                if (!service.CheckIsLinenumberOwner(senderNumbers, Username))
                    return new string[1] { "20" };

                return service.SendSMSQueueWithId(Ids, Messages, Mobiles, senderNumbers, Username);
            }
            catch (Exception ex)
            {
                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\ArraySendQeueWithId.txt", true))
                {
                    outfile.Write("from NewSmsWebserivce::" + DateTime.Now.ToString() + "::" + ex.Message + "::" + ex.StackTrace + Environment.NewLine);
                }
                throw;
            }
        }

        [WebMethod]
        public string[] SendQeue(string username, string password, string message, string fromNumber, string[] toNumbers)
        {
            try
            {
                //Utility.CreateLog(HttpContext.Current.Request.UserHostAddress);

                //Utility.CreateLog("USer:"+username+"::"+DateTime.Now);
                //Utility.CreateLog("Numbers:" +  toNumbers.Length);

                fromNumber = Helpers.Utility.FixPhoneNumber(fromNumber);
                SMSService service = new SMSService();
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
                List<UserInfo> info = service.Authenticate(username, password).ToList();
                if (info == null || info.Count == 0)
                    return new string[1] { "18" };
                if (!info[0].IUsrActive.GetValueOrDefault(false))
                    return new string[1] { "16" };
                if (!service.CheckIsLinenumberOwner(new string[] { fromNumber }, username))
                    return new string[1] { "20" };

                return service.SendSMSQueue(message, toNumbers, fromNumber, username);
            }
            catch (Exception ex)
            {
                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\SendQeue.txt", true))
                {
                    outfile.Write("from NewSmsWebserivce::" + DateTime.Now.ToString() + "::" + ex.Message + "::" + ex.StackTrace + Environment.NewLine);
                }
                return new string[1] { "Failed" };
            }

        }

        [WebMethod]
        public long[] Send(string username, string password, string message, string fromNumber, string[] toNumbers)
        {
           
            fromNumber = Helpers.Utility.FixPhoneNumber(fromNumber);
            SMSService service = new SMSService();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
            List<UserInfo> info = service.Authenticate(username, password).ToList();
            if (info == null || info.Count == 0)
                return new long[1] { (long)18 };
            if (!info[0].IUsrActive.GetValueOrDefault(false))
                return new long[1] { (long)16 };
            if (!service.CheckIsLinenumberOwner(new string[] { fromNumber }, username))
                return new long[1] { (long)20 };

            string[] sms = new string[toNumbers.Length];
            int[] encods = new int[toNumbers.Length];
            int[] mclass = new int[toNumbers.Length];
            int[] priority = new int[toNumbers.Length];
            string[] origis = new string[toNumbers.Length];
            string[] udh = new string[toNumbers.Length];
            long[] checkingIds = new long[toNumbers.Length];
            Random rnd = new Random();
            for (int i = 0; i < toNumbers.Length; i++)
            {
                sms[i] = message;
                encods[i] = 1;
                origis[i] = fromNumber;
                udh[i] = "";
                mclass[i] =1 ;
                priority[i] = -1;
                checkingIds[i] = rnd.Next();
            }
            return service.SendSMS(sms, encods, toNumbers, origis, udh, mclass, priority, checkingIds, username);
        }
        [WebMethod]
        public long[] SendWithCheckinId(string username, string password, string[] message, string fromNumber, string[] toNumbers,long[] checkingMessageId)
        {
            #region forssb776
            //if (username.Contains("ssb776"))
            //{
            //    using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\Send_LogForMoalem.txt", true))
            //    {
            //        outfile.Write(" ####################################################################################"
            //            + Environment.NewLine);
            //    }
            //    using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\Send_LogForMoalem.txt", true))
            //    {
            //        outfile.Write("Date :" + DateTime.Now + Environment.NewLine + " username :" + username + " password :" + password + Environment.NewLine +
            //            " message :" + message + " fromNumber :" + fromNumber
            //            + Environment.NewLine);
            //    }
            //    for (int i = 0; i < toNumbers.Count(); i++)
            //    {
            //        using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\Send_LogForMoalem.txt", true))
            //        {
            //            outfile.Write(" toNumbers[" + i.ToString() + "] :" + toNumbers[i]
            //                + Environment.NewLine);
            //        }
            //    }
            //    using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\Send_LogForMoalem.txt", true))
            //    {
            //        outfile.Write(" ####################################################################################"
            //            + Environment.NewLine);
            //    }
            //}
            #endregion
            
            fromNumber = Helpers.Utility.FixPhoneNumber(fromNumber);
            SMSService service = new SMSService();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
            List<UserInfo> info = service.Authenticate(username, password).ToList();
            if (info == null || info.Count == 0)
                return new long[1] { (long)18 };
            if (!info[0].IUsrActive.GetValueOrDefault(false))
                return new long[1] { (long)16 };
            if (!service.CheckIsLinenumberOwner(new string[] { fromNumber }, username))
                return new long[1] { (long)20 };
            if(toNumbers.Length!=checkingMessageId.Length)
                throw new Exception(" تعداد شناسه ارسالی با تعداد گیرنده برابر نمیباشد.");
            string[] sms = new string[toNumbers.Length];
            int[] encods = new int[toNumbers.Length];
            int[] mclass = new int[toNumbers.Length];
            int[] priority = new int[toNumbers.Length];
            string[] origis = new string[toNumbers.Length];
            string[] udh = new string[toNumbers.Length];
            long[] checkingIds = new long[toNumbers.Length];
            Random rnd = new Random();
            for (int i = 0; i < toNumbers.Length; i++)
            {
                sms[i] = message[i];
                encods[i] = 1;
                origis[i] = fromNumber;
                udh[i] = "";
                mclass[i] = 1;
                priority[i] = -1;
                checkingIds[i] = rnd.Next();
            }
            return service.SendSMS(sms, encods, toNumbers, origis, udh, mclass, priority, checkingMessageId, username);
        }
        [WebMethod]
        public int[] getArrayMessageStatus(string username, string password, long[] id)
        {
            SMSService service = new SMSService();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
            if(id.Length>100)
                throw new Exception(" در هربار فقط 100 پیامک فراخوانی میشود.");
            List<UserInfo> info = service.Authenticate(username, password).ToList();
            if (info == null || info.Count == 0)
                return new int[1] { (int)18 }; ;
            return service.getMessageStatusFromMagfa(id);
        }
        [WebMethod]
        public long[] ArraySend(string username, string password, string[] message, string fromNumber, string[] toNumbers)
        {
            fromNumber = Helpers.Utility.FixPhoneNumber(fromNumber);
            SMSService service = new SMSService();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
            List<UserInfo> info = service.Authenticate(username, password).ToList();
            if (info == null || info.Count == 0)
                return new long[1] { (long)18 };
            if (!info[0].IUsrActive.GetValueOrDefault(false))
                return new long[1] { (long)16 };
            if (!service.CheckIsLinenumberOwner(new string[] { fromNumber }, username))
                return new long[1] { (long)20 };
            if (message.Count()!=toNumbers.Count())
                return new long[1] { (long)101 };

            string[] sms = new string[toNumbers.Length];
            int[] encods = new int[toNumbers.Length];
            int[] mclass = new int[toNumbers.Length];
            int[] priority = new int[toNumbers.Length];
            string[] origis = new string[toNumbers.Length];
            string[] udh = new string[toNumbers.Length];
            long[] checkingIds = new long[toNumbers.Length];
            Random rnd = new Random();
            for (int i = 0; i < toNumbers.Length; i++)
            {
                sms[i] = message[i];
                encods[i] = 1;
                origis[i] = fromNumber;
                udh[i] = "";
                mclass[i] = 1;
                priority[i] = -1;
                checkingIds[i] = rnd.Next();
            }
            return service.SendSMSForArraySendMagfa(sms, encods, toNumbers, origis, udh, mclass, priority, checkingIds, username);
        }
        [WebMethod]
        public long[] SendWithUdh(string username, string password, string message, string fromNumber, string[] toNumbers, string[] Udh)
        {
            fromNumber = Helpers.Utility.FixPhoneNumber(fromNumber);
            SMSService service = new SMSService();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
            List<UserInfo> info = service.Authenticate(username, password).ToList();
            if (info == null || info.Count == 0)
                return new long[1] { (long)18 };
            if (!info[0].IUsrActive.GetValueOrDefault(false))
                return new long[1] { (long)16 };
            if (!service.CheckIsLinenumberOwner(new string[] { fromNumber }, username))
                return new long[1] { (long)20 };

            string[] sms = new string[toNumbers.Length];
            int[] encods = new int[toNumbers.Length];
            int[] mclass = new int[toNumbers.Length];
            int[] priority = new int[toNumbers.Length];
            string[] origis = new string[toNumbers.Length];
            string[] udh = new string[toNumbers.Length];
            long[] checkingIds = new long[toNumbers.Length];
            Random rnd = new Random();
            for (int i = 0; i < toNumbers.Length; i++)
            {
                sms[i] = message;
                encods[i] = 1;
                origis[i] = fromNumber;
                udh[i] = "";
                mclass[i] = -1;
                priority[i] = -1;
                checkingIds[i] = rnd.Next();
            }
            return service.SendSMS(sms, encods, toNumbers, origis, Udh, mclass, priority, checkingIds, username);
        }
        [WebMethod]
        public int GetUserCredit(string Username, string password)
        {
            SMSService srv = new SMSService();
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(password))
                throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
            List<UserInfo> info = srv.Authenticate(Username, password).ToList();
            if (info == null || info.Count == 0)
                return -18;
            if (!info[0].IUsrActive.GetValueOrDefault(false))
                return -16;
            return srv.GetCurrentCredit(string.Empty, Username);
        }


        [WebMethod]
        public int[] GetMessageStatus(string Username, string password, long[] MessageIds)
        {
            SMSService srv = new SMSService();
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(password))
                throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
            List<UserInfo> info = srv.Authenticate(Username, password).ToList();
            if (info == null || info.Count == 0)
                return (new int[] { -18 });
            if (!info[0].IUsrActive.GetValueOrDefault(false))
                return (new int[] { -16 });
            int[] result = new int[MessageIds.Length];

            long msgId = 0;

            for (int j = 0; j < MessageIds.Length; j++)
            {
                using (SMSModelContainer ctx = new SMSModelContainer())
                {

                    msgId = MessageIds[j];
                    result[j] = (from i in ctx.Tbl_SentSMS
                                 where i.ISMSStatusCode.HasValue && i.MagfaSmsId.Equals(msgId)
                                 select i.ISMSStatusCode.Value).FirstOrDefault();


                }

            }
            return result;
        }

        [WebMethod]
        public int[] GetQueueMessageStatus(string Username, string password, string[] MessageIds)
        {
            int[] result = new int[MessageIds.Length];
            Guid[] ids = Array.ConvertAll(MessageIds, x => new Guid(x));
            using (SMSModelContainer ctx = new SMSModelContainer())
            {
                var res = (from i in ctx.View_Tbl_SentSMS
                           where i.ISMSStatusCode.HasValue && ids.Contains(i.Id)
                           select new { i.Id, i.ISMSStatusCode.Value }).ToArray();
                for (int i = 0; i < MessageIds.Length; i++)
                {
                    result[i] = (from r in res where r.Id == Guid.Parse(MessageIds[i]) select r.Value).FirstOrDefault();
                }
            }
            return result;
        }

        [WebMethod]
        public int CheckCredit(string Username, string password, int SmsFaCount, int SmsEnCount)
        {
            SMSService srv = new SMSService();
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(password))
                throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
            List<UserInfo> info = srv.Authenticate(Username, password).ToList();
            if (info == null || info.Count == 0)
                return -18;
            if (!info[0].IUsrActive.GetValueOrDefault(false))
                return -16;
            return srv.CheckCredit(SmsFaCount, SmsEnCount, Username);
        }

        [WebMethod]
        public double GetTarrifOperator(string linenumber)
        {
            SMSService _service = new SMSService();
            return _service.GetTarrifrateRate(linenumber);
        }

        private DataTable GetTblRcvSchema()
        {
            DataTable dt = new DataTable("RecieveSMS");
            DataColumn dcRcvSmsText = new DataColumn("RcvSmsText", typeof(string));
            DataColumn dcRcvSmsfrom = new DataColumn("RcvSmsfrom", typeof(string));
            DataColumn dcRcvSmsTo = new DataColumn("RcvSmsTo", typeof(string));
            DataColumn dcRcvSmsInteredDate = new DataColumn("RcvSmsInteredDate", typeof(string));
            DataColumn dcRcvSmsMessageID = new DataColumn("RcvSmsMessageID", typeof(string));
            DataColumn dcRcvSmsDeliveredTime = new DataColumn("RcvSmsDeliveredTime", typeof(string));
            DataColumn dcRecieveDate = new DataColumn("RecieveDate", typeof(string));

            dt.Columns.AddRange(new DataColumn[] { dcRcvSmsDeliveredTime, dcRcvSmsInteredDate, dcRcvSmsMessageID,  dcRcvSmsText, dcRcvSmsfrom
            ,dcRcvSmsTo,dcRecieveDate});
            return dt;
        }

        [WebMethod]
        public DataTable RecieveSMS(string Username, string password, string phNo, string startdate, string enddate)
        {
            phNo = Helpers.Utility.FixPhoneNumber(phNo);
            SMSService srv = new SMSService();
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(password))
                throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
            List<UserInfo> info = srv.Authenticate(Username, password).ToList();
            if (info == null || info.Count == 0)
                throw new Exception(" نام کاربری و رمز عبور صحیح نمی باشد.");

            if (!info[0].IUsrActive.GetValueOrDefault(false))
                throw new Exception(" کاربر غیر فعال می باشد.");
            if (!srv.CheckIsLinenumberOwner(new string[] { phNo }, Username))
                throw new Exception("خط متعلق به کاربر نمی باشد.");

            List<Tbl_RecieveSms> list = srv.RecieveSMS(startdate, enddate, phNo);
            DataTable dt = GetTblRcvSchema();

            foreach (var rcvs in list)
            {
                DataRow dr = dt.NewRow();
                rcvs.RcvSmsText = Regex.Replace(rcvs.RcvSmsText, "[\x01-\x1F]", "");
                dr["RcvSmsText"] = rcvs.RcvSmsText;
                dr["RcvSmsfrom"] = rcvs.RcvSmsfrom;
                dr["RcvSmsTo"] = rcvs.RcvSmsTo;
                dr["RcvSmsInteredDate"] = rcvs.RcvSmsInteredDate;
                dr["RcvSmsMessageID"] = rcvs.RcvSmsMessageID;
                dr["RcvSmsDeliveredTime"] = rcvs.RcvSmsDeliveredTime;
                dr["RecieveDate"] = (rcvs.RecieveDate.HasValue ? rcvs.RecieveDate.Value.ToString("yyyy-MM-dd-HH-mm-ss") : rcvs.RecieveDate.Value.ToString());
                dt.Rows.Add(dr);
            }
            return dt;
        }

        [WebMethod]
        public List<RecieveSms> RecieveSMSById(string Username, string password, string phNo, int Id)
        {
            phNo = Helpers.Utility.FixPhoneNumber(phNo);
            SMSService srv = new SMSService();
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(password))
                throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
            List<UserInfo> info = srv.Authenticate(Username, password).ToList();
            if (info == null || info.Count == 0)
                throw new Exception(" نام کاربری و رمز عبور صحیح نمی باشد.");

            if (!info[0].IUsrActive.GetValueOrDefault(false))
                throw new Exception(" کاربر غیر فعال می باشد.");
            if (!srv.CheckIsLinenumberOwner(new string[] { phNo }, Username))
                throw new Exception("خط متعلق به کاربر نمی باشد.");

            List<Tbl_RecieveSms> list = srv.RecieveSMSById(Id, phNo);
            List<RecieveSms> rcvsList = new List<RecieveSms>();
            char[] invalid = System.IO.Path.GetInvalidFileNameChars();
            invalid = (from p in invalid
                       where p != '*' && p != '>' && p != '<' && p != '\n' && p != '\r' && p != '\t' && p != '\f' && p != '\a' && p != '\v'
                           && p != ':' && p != '\\' && p != '/' && p != '-' && p != '"' && p != '|' && p != '?'
                       select p).ToArray();
            for (int i = 0; i < list.Count; i++)
            {
                string s = list[i].RcvSmsText;
                list[i].RcvSmsText = new String(s.Where(c => !invalid.Contains(c)).ToArray());
            }
            for (int i = 0; i < list.Count; i++)
            {
                RecieveSms rcvs = new RecieveSms();
                list[i].RcvSmsText = Regex.Replace(list[i].RcvSmsText, "[\x01-\x1F]", "");
                rcvs.RcvSmsText = list[i].RcvSmsText;
                rcvs.RcvsmsKeyWord = list[i].RcvsmsKeyWord;
                rcvs.RcvSmsfrom = list[i].RcvSmsfrom;
                rcvs.RcvSmsTo = list[i].RcvSmsTo;
                rcvs.RcvSmsInteredDate = list[i].RcvSmsInteredDate;
                rcvs.RcvSmsUDH = list[i].RcvSmsUDH;
                rcvs.RcvsmsCharSet = list[i].RcvsmsCharSet;
                rcvs.rcvSmsSmsC = list[i].rcvSmsSmsC;
                rcvs.RcvSmsStatus = list[i].RcvSmsStatus;
                rcvs.RcvSmsMessageID = list[i].RcvSmsMessageID;
                rcvs.RcvSmsRuleID = list[i].RcvSmsRuleID;
                rcvs.RcvSmsDeliveredTime = list[i].RcvSmsDeliveredTime;
                rcvs.RcvSmsReeded = false;
                rcvs.UsrId = list[i].UsrId;
                rcvs.RecieveSMSId = list[i].RecieveSMSId;
                rcvs.OperatorId = list[i].OperatorId.Value;
                rcvs.RecieveDate = list[i].RecieveDate.Value;
                rcvsList.Add(rcvs);
            }
            return rcvsList;

        }
        //[WebMethod]
        //public List<RecieveSmsStruct> NewRecieveSMSById(string Username, string password, string phNo, int Id)
        //{
        //    phNo = Helpers.Utility.FixPhoneNumber(phNo);
        //    SMSService srv = new SMSService();
        //    if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(password))
        //        throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
        //    List<UserInfo> info = srv.Authenticate(Username, password).ToList();
        //    if (info == null || info.Count == 0)
        //        throw new Exception(" نام کاربری و رمز عبور صحیح نمی باشد.");

        //    if (!info[0].IUsrActive.GetValueOrDefault(false))
        //        throw new Exception(" کاربر غیر فعال می باشد.");
        //    if (!srv.CheckIsLinenumberOwner(new string[] { phNo }, Username))
        //        throw new Exception("خط متعلق به کاربر نمی باشد.");

        //    List<Tbl_RecieveSms> list = srv.RecieveSMSById(Id, phNo);
        //    List<RecieveSmsStruct> rcvsList = new List<RecieveSmsStruct>();
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        RecieveSmsStruct rcvs = new RecieveSmsStruct();
        //        list[i].RcvSmsText = Regex.Replace(list[i].RcvSmsText, "[\x01-\x1F]", "");
        //        rcvs.RcvSmsText = list[i].RcvSmsText;
        //        rcvs.RcvsmsKeyWord = list[i].RcvsmsKeyWord;
        //        rcvs.RcvSmsfrom = list[i].RcvSmsfrom;
        //        rcvs.RcvSmsTo = list[i].RcvSmsTo;
        //        rcvs.RcvSmsInteredDate = list[i].RcvSmsInteredDate;
        //        rcvs.RcvSmsUDH = list[i].RcvSmsUDH;
        //        rcvs.RcvsmsCharSet = list[i].RcvsmsCharSet;
        //        rcvs.rcvSmsSmsC = list[i].rcvSmsSmsC;
        //        rcvs.RcvSmsStatus = list[i].RcvSmsStatus;
        //        rcvs.RcvSmsMessageID = list[i].RcvSmsMessageID;
        //        rcvs.RcvSmsRuleID = list[i].RcvSmsRuleID;
        //        rcvs.RcvSmsDeliveredTime = list[i].RcvSmsDeliveredTime;
        //        rcvs.RcvSmsReeded = false;
        //        rcvs.UsrId = list[i].UsrId;
        //        rcvs.RecieveSMSId = list[i].RecieveSMSId;
        //        rcvs.OperatorId = list[i].OperatorId.Value;
        //        rcvs.RecieveDate = list[i].RecieveDate.Value;
        //        rcvsList.Add(rcvs);
        //    }
        //    return rcvsList;

        //}
        [WebMethod]
        public int GetUnreadMessgese(string Username, string password, string toNumber, out string[] messages, out string[] fromNumber, out string[] recivedates)
        {
            toNumber = Helpers.Utility.FixPhoneNumber(toNumber);
            SMSService srv = new SMSService();
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(password))
                throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
            List<UserInfo> info = srv.Authenticate(Username, password).ToList();
            if (info == null || info.Count == 0)
                throw new Exception(" نام کاربری و رمز عبور صحیح نمی باشد.");

            if (!info[0].IUsrActive.GetValueOrDefault(false))
                throw new Exception(" کاربر غیر فعال می باشد.");
            if (!srv.CheckIsLinenumberOwner(new string[] { toNumber }, Username))
                throw new Exception("خط متعلق به کاربر نمی باشد.");
            //if (!srv.CheckIsLinenumberOwner(senderNumbers, Auth.Username))
            //    throw new Exception(" کاربر غیر فعال می باشد.");

            List<Tbl_RecieveSms> list = srv.RecieveUnreadSMS(toNumber);
            messages = new string[list.Count];
            fromNumber = new string[list.Count];
            recivedates = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                list[i].RcvSmsText = Regex.Replace(list[i].RcvSmsText, "[\x01-\x1F]", "");
                messages[i] = list[i].RcvSmsText;
                fromNumber[i] = list[i].RcvSmsfrom;
                recivedates[i] = (list[i].RecieveDate.HasValue ? list[i].RecieveDate.Value.ToString("yyyy-MM-dd-HH-mm-ss") : list[i].RecieveDate.Value.ToString());
            }

            return list.Count;
        }

        [WebMethod]
        public List<RecieveSms> GetUnreadMessgeseWithUsername(string Username, string password)
        {
            SMSService srv = new SMSService();
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(password))
                throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
            List<UserInfo> info = srv.Authenticate(Username, password).ToList();
            if (info == null || info.Count == 0)
                throw new Exception(" نام کاربری و رمز عبور صحیح نمی باشد.");

            if (!info[0].IUsrActive.GetValueOrDefault(false))
                throw new Exception(" کاربر غیر فعال می باشد.");

            List<Tbl_RecieveSms> list = srv.GetUnreadMessgeseWithUsername(Username);
            List<RecieveSms> rcvsList = new List<RecieveSms>();
            for (int i = 0; i < list.Count; i++)
            {
                RecieveSms rcvs = new RecieveSms();
                list[i].RcvSmsText = Regex.Replace(list[i].RcvSmsText, "[\x01-\x1F]", "");
                rcvs.RcvSmsText = list[i].RcvSmsText;
                rcvs.RcvsmsKeyWord = list[i].RcvsmsKeyWord;
                rcvs.RcvSmsfrom = list[i].RcvSmsfrom;
                rcvs.RcvSmsTo = list[i].RcvSmsTo;
                rcvs.RcvSmsInteredDate = list[i].RcvSmsInteredDate;
                rcvs.RcvSmsUDH = list[i].RcvSmsUDH;
                rcvs.RcvsmsCharSet = list[i].RcvsmsCharSet;
                rcvs.rcvSmsSmsC = list[i].rcvSmsSmsC;
                rcvs.RcvSmsStatus = list[i].RcvSmsStatus;
                rcvs.RcvSmsMessageID = list[i].RcvSmsMessageID;
                rcvs.RcvSmsRuleID = list[i].RcvSmsRuleID;
                rcvs.RcvSmsDeliveredTime = list[i].RcvSmsDeliveredTime;
                rcvs.RcvSmsReeded = false;
                rcvs.UsrId = list[i].UsrId;
                rcvs.RecieveSMSId = list[i].RecieveSMSId;
                rcvs.OperatorId = list[i].OperatorId.Value;
                rcvs.RecieveDate = list[i].RecieveDate.Value;
                rcvsList.Add(rcvs);
            }
            return rcvsList;
        }
        // [Serializable, XmlRoot("RecieveSms")]
        public class RecieveSms
        {
            public string RcvSmsText; //{ get; set; }
            public string RcvsmsKeyWord; //{ get; set; }
            public string RcvSmsfrom;// { get; set; }
            public string RcvSmsTo; //{ get; set; }
            public string RcvSmsInteredDate; //{ get; set; }
            public string RcvSmsUDH; //{ get; set; }
            public string RcvsmsCharSet;// { get; set; }
            public string rcvSmsSmsC;// { get; set; }
            public string RcvSmsStatus;// { get; set; }
            public string RcvSmsMessageID;// { get; set; }
            public string RcvSmsRuleID; //{ get; set; }
            public string RcvSmsDeliveredTime;// { get; set; }
            public bool RcvSmsReeded;// { get; set; }
            public string UsrId;//{ get; set; }
            public int RecieveSMSId; //{ get; set; }
            public int OperatorId; //{ get; set; }
            public DateTime RecieveDate; //{ get; set; }
        }
        //public struct RecieveSmsStruct {
        //    public string RcvSmsText; 
        //    public string RcvsmsKeyWord; 
        //    public string RcvSmsfrom;
        //    public string RcvSmsTo; 
        //    public string RcvSmsInteredDate; 
        //    public string RcvSmsUDH; 
        //    public string RcvsmsCharSet;
        //    public string rcvSmsSmsC;
        //    public string RcvSmsStatus;
        //    public string RcvSmsMessageID;
        //    public string RcvSmsRuleID; 
        //    public string RcvSmsDeliveredTime;
        //    public bool RcvSmsReeded;
        //    public string UsrId;
        //    public int RecieveSMSId; 
        //    public int OperatorId; 
        //    public DateTime RecieveDate; 
        //}
        //[Serializable, XmlRoot("RecieveSms")]
        //public class NewRecieveSms
        //{
        //    public string RcvSmsText { get; set; }
        //    public string RcvsmsKeyWord { get; set; }
        //    public string RcvSmsfrom { get; set; }
        //    public string RcvSmsTo { get; set; }
        //    public string RcvSmsInteredDate { get; set; }
        //    public string RcvSmsUDH { get; set; }
        //    public string RcvsmsCharSet { get; set; }
        //    public string rcvSmsSmsC { get; set; }
        //    public string RcvSmsStatus { get; set; }
        //    public string RcvSmsMessageID { get; set; }
        //    public string RcvSmsRuleID { get; set; }
        //    public string RcvSmsDeliveredTime;// { get; set; }
        //    public bool RcvSmsReeded { get; set; }
        //    public string UsrId { get; set; }
        //    public int RecieveSMSId { get; set; }
        //    public int OperatorId { get; set; }
        //    public DateTime RecieveDate { get; set; }
        //}

        [WebMethod]
        public string Test(string[] str)
        {
            Utility.CreateLog("Test: Str length is :" + str.Length);
            return str.Length.ToString();
        }

        [WebMethod]
        public string[] CheckVersion(string username, string password)
        {
            string ln = string.Empty;
            string vr = string.Empty;
            try
            {

                SMSService srv = new SMSService();
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
                List<UserInfo> info = srv.Authenticate(username, password).ToList();
                if (info == null || info.Count == 0)
                    return new string[1] { "18" };
                if (!info[0].IUsrActive.GetValueOrDefault(false))
                    return new string[1] { "16" };
                var lns = srv.GetLineNumbers(username);
                vr = srv.CheckVersion(username);
                if (lns != null && lns.Count > 0)
                    ln = lns[0].ILineNumber;
            }
            catch (Exception ex)
            {
                Utility.CreateLog(ex.Message + "::" + ex.StackTrace);
                throw;
            }

            return new string[] { vr, ln };
        }
        [WebMethod]
        public string[] CheckVersionNew(string username, string password)
        {
            string ln = string.Empty;
            string vr = string.Empty;
            try
            {

                SMSService srv = new SMSService();
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    throw new Exception(" نام کاربری و رمز عبور داده نشده است.");
                List<UserInfo> info = srv.Authenticate(username, password).ToList();
                if (info == null || info.Count == 0)
                    return new string[1] { "18" };
                if (!info[0].IUsrActive.GetValueOrDefault(false))
                    return new string[1] { "16" };
                var lns = srv.GetLineNumbers(username);
                vr = srv.CheckVersion(username);
                if (lns != null && lns.Count > 0)
                {
                    for (int i = 0; i < lns.Count; i++)
                        ln += lns[i].ILineNumber + ",";
                    ln = ln.Substring(0, ln.Length - 1);
                }
            }
            catch (Exception ex)
            {
                Utility.CreateLog(ex.Message + "::" + ex.StackTrace);
                throw;
            }

            return new string[] { vr, ln };
        }
    }
}
