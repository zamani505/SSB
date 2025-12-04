using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSB.Service.Web
{
    public class VoiceMessageClass
    {
        public avanak.WebService3 avanak = new Web.avanak.WebService3();
        public long CreateFileByText(string username, string password, string speaker, string text, string title, string callFromMobile)
        {
            long Result = 0;
            try
            {

                Result = avanak.GenerateTTS(username, password, speaker, text, title, callFromMobile);
            }
            catch (Exception ex)
            {

                //throw;
            }
            return Result;
        }
        public long BulkSendVoiceMessage(string username, string password, string title, string numbers, int maxtrycount,
            int minutebetweentries, string startDate, string startTime, string endDate, string endTime, int messageId, bool removeInvalids, bool autoStart, bool vote, int serverId)
        {
            long Result = 0;

            Result=avanak.CreateCampaign(username, password, title, numbers, maxtrycount, minutebetweentries, startDate, startTime, endDate, endTime, messageId, removeInvalids, serverId,
                autoStart, vote);


            return Result;
        }
        public decimal getCredit(string username, string password)
        {
            return avanak.GetCredit(username, password);
        }
        public byte GetSendVoiceStatus(string username, string password,int avanakId)
        {
            byte res = 20;
            var ava = avanak.GetCampaignById(username, password, avanakId);
            if (ava == null)
                return res;
            else
            {
                switch (ava.StatusId)
                {
                    case 8:
                        res = 31;
                        break;
                    case 9:
                        res = 17;
                        break;
                    case 10:
                        res = 32;
                        break;
                    case 11:
                        res = 22;
                        break;
                    case 12:
                        res = 1;
                        break;
                    case 27:
                        res = 20;
                        break;
                }
            }
            return res;
        }
        public long SendVoiceMessageWithText(string username, string password, string speaker, string text, string title, int serverId, string number,
           bool vote, string callFromMobile)
        {
            long Result = 0;
            try
            {
                Result = avanak.QuickSendWithTTS(username, password, text, number, vote, serverId, callFromMobile);
            }
            catch (Exception ex)
            {

                //throw;
            }
            return Result;
        }

        public long SendVoiceMessageWithVoice(string username, string password, string title, byte[] file, string callFromMobile)
        {
            long Result = 0;
            try
            {
                Result = avanak.UploadMessage(username, password, title, file, false, callFromMobile);
            }
            catch (Exception ex)
            {

                //throw;
            }
            return Result;
        }
        public byte[] DownloadMessage(string username, string password, int messageId)
        {
            var res = avanak.DownloadMessage(username, password, messageId);
            return res;
        }
        public long UploadFile(string username, string password, string title, byte[] file, string callFromMobile)
        {
            return avanak.UploadMessage(username, password, title, file, false, callFromMobile);
        }
    }
}