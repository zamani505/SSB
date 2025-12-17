using SSB.Service.Core;
using SSB.Service.SSBApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static SSB.Service.Web.NewSmsWebservice;
using System.Text.RegularExpressions;
using System.Web;
using System.Net;
using System.Net.Http;


namespace SSB.Service.SSBApi.Extentions
{
    public static class SSBExtentions
    {

        public static string CreateToken(this string username, string pass)
        => Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes($"{username}:{pass}:{Guid.NewGuid()}"));

        public static string[] FixPhoneNumber(this string[] numbers)
        {
            for (int i = 0; i < numbers.Count(); i++)
                numbers[i] = Helpers.Utility.FixPhoneNumber(numbers[i]);
            return numbers;
        }
        public static int Diff(this DateTime start,DateTime end)
        {
            var diff = end - start;
            return (int)diff.TotalMilliseconds;
        }
        public static string GetVerbName(this string verbName)
            => verbName.Substring(verbName.LastIndexOf('/') + 1);
        public static bool IsRequestLogin(this HttpRequestMessage request)
            => request.RequestUri.AbsolutePath.GetVerbName().ToLower().Equals("login");
        public static string Body(this HttpRequestMessage request)
           => request.Content != null ? request.Content.ReadAsStringAsync().Result : "";
        public static List<RecieveSMSModel> ToRecieveSMSModel(this List<Tbl_RecieveSms> tbl_RecieveSms) {
            List<RecieveSMSModel> rcvsList = new List<RecieveSMSModel>();
            for (int i = 0; i < tbl_RecieveSms.Count; i++)
            {
                RecieveSMSModel rcvs = new RecieveSMSModel();
                tbl_RecieveSms[i].RcvSmsText = Regex.Replace(tbl_RecieveSms[i].RcvSmsText, "[\x01-\x1F]", "");
                rcvs.RcvSmsText = tbl_RecieveSms[i].RcvSmsText;
                rcvs.RcvsmsKeyWord = tbl_RecieveSms[i].RcvsmsKeyWord;
                rcvs.RcvSmsfrom = tbl_RecieveSms[i].RcvSmsfrom;
                rcvs.RcvSmsTo = tbl_RecieveSms[i].RcvSmsTo;
                rcvs.RcvSmsInteredDate = tbl_RecieveSms[i].RcvSmsInteredDate;
                rcvs.RcvSmsUDH = tbl_RecieveSms[i].RcvSmsUDH;
                rcvs.RcvsmsCharSet = tbl_RecieveSms[i].RcvsmsCharSet;
                rcvs.rcvSmsSmsC = tbl_RecieveSms[i].rcvSmsSmsC;
                rcvs.RcvSmsStatus = tbl_RecieveSms[i].RcvSmsStatus;
                rcvs.RcvSmsMessageID = tbl_RecieveSms[i].RcvSmsMessageID;
                rcvs.RcvSmsRuleID = tbl_RecieveSms[i].RcvSmsRuleID;
                rcvs.RcvSmsDeliveredTime = tbl_RecieveSms[i].RcvSmsDeliveredTime;
                rcvs.RcvSmsReeded = false;
                rcvs.UsrId = tbl_RecieveSms[i].UsrId;
                rcvs.RecieveSMSId = tbl_RecieveSms[i].RecieveSMSId;
                rcvs.OperatorId = tbl_RecieveSms[i].OperatorId.Value;
                rcvs.RecieveDate = tbl_RecieveSms[i].RecieveDate.Value;
                rcvsList.Add(rcvs);
            }
            return rcvsList;
        }
        public static string GetClientIpAddress(this HttpRequestMessage request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            string xff = request.Headers.GetValues("X-Forwarded-For").FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(xff))
            {
                string firstIp = xff.Split(',').FirstOrDefault()?.Trim();
                if (IsValidIpAddress(firstIp))
                    return firstIp;
            }
            string xRealIp = request.Headers.GetValues("X-Real-IP").FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(xRealIp) && IsValidIpAddress(xRealIp))
                return xRealIp;
            string userHost = request.GetClientIpAddress();
            if (!string.IsNullOrWhiteSpace(userHost) && IsValidIpAddress(userHost))
                return userHost;

            return "0.0.0.0"; 
        }
        private static bool IsValidIpAddress(string ip)
        {
            return !string.IsNullOrWhiteSpace(ip) &&
                   (IPAddress.TryParse(ip, out _) || ip == "::1" || ip.StartsWith("fe80:") || ip.StartsWith("::ffff:"));
        }
    }
}