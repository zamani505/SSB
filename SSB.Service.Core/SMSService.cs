using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Transactions;
using SSB.Helpers;
using SSB.Service.Core.MagfaService;
using SSB.Service.Core.Service;
using System.Text.RegularExpressions;

namespace SSB.Service.Core
{
    public class SMSService
    {
        public int GetCurrentCredit(string today, string userID)
        {

            SMSModelContainer modelContainer = new SMSModelContainer();
            return modelContainer.SpTbl_UsrCredit_SelectTotalCredit(userID, Utility.PersianNowDate).First().GetValueOrDefault(0);
        }

        public double GetTarrifrateRate(string lineNumer)
        {
            double rate = 1;
            using (SMSModelContainer model = new SMSModelContainer())
            {
                lineNumer = Helper.SetPrefixNum(lineNumer);

                if (!Regex.IsMatch(lineNumer, @"^[0-9-]*$"))
                {
                    if (model.OperatorTarrifRates.Any(x => x.LineStartWith == "Brand"))
                        rate = (from p in model.OperatorTarrifRates
                                where p.LineStartWith == "Brand"
                                select new { p.Rate }).First().Rate;
                }
                else
                {
                    if (model.OperatorTarrifRates.Any(x => lineNumer.StartsWith(x.LineStartWith)))
                        rate = (from p in model.OperatorTarrifRates
                                where lineNumer.StartsWith(p.LineStartWith)
                                select new { p.Rate }).First().Rate;
                }
            }

            return rate;

        }
        public int GetOperatorId(string lineNumer)
        {
            int rate = 3;
            using (SMSModelContainer model = new SMSModelContainer())
            {
                lineNumer = Helper.SetPrefixNum(lineNumer);

                if (!Regex.IsMatch(lineNumer, @"^[0-9-]*$"))
                    return 6;
                if (model.OperatorTarrifRates.Any(x => lineNumer.StartsWith(x.LineStartWith)))
                    rate = (from p in model.OperatorTarrifRates
                            where lineNumer.StartsWith(p.LineStartWith)
                            select new { p.OperatorId }).First().OperatorId;
            }

            return rate;

        }

        public bool HasEnoughCredit(int noEnSMS, int noFaSMS, string userId, string LineNumber)
        {

            double rate = GetTarrifrateRate(LineNumber);
            int i = 0;
            int varFaSMS, varEnSMS;
            int FaSmsSum = 0;
            int EnSmsSum = 0;

            using (SMSModelContainer db = new SMSModelContainer())
            {

                try
                {
                    List<UsrCredit> credits = new List<UsrCredit>();

                    credits = (from c in db.UsrCredits
                               where c.IUsrId == userId && c.ICreditAmount > 0
                               select c).ToList();

                    if (credits.Count == 0) return false;

                    while (credits.Count > 0)
                    {
                        //&& i < credits.Count()

                        FaSmsSum = 0;
                        EnSmsSum = 0;
                        for (int j = 0; j < credits.Count; j++)
                        {
                            EnSmsSum += (int)Math.Floor(Convert.ToDouble(credits[j].ICreditAmount.GetValueOrDefault(0) /
                                                                        (credits[j].ISMSEnPrice.GetValueOrDefault(0) *
                                                                        rate)));

                            FaSmsSum += (int)Math.Floor(Convert.ToDouble(credits[j].ICreditAmount.GetValueOrDefault(0) /
                                                                        (credits[j].ISMSFaPrice.GetValueOrDefault(0) *
                                                                         rate)));

                        }

                        varFaSMS = noFaSMS;
                        varEnSMS = noEnSMS;
                        i = 0;


                        if (varEnSMS != 0)
                        {
                            varEnSMS -= EnSmsSum;
                            if (varFaSMS != 0) varFaSMS -= FaSmsSum;
                        }

                        else if (varFaSMS != 0) varFaSMS -= FaSmsSum;
                        string temp_user = userId;

                        #region comments
                        //while ((varFaSMS > 0 || varEnSMS > 0))
                        //{

                        //    if (varEnSMS > 0)
                        //    {
                        //        smsCredit =
                        //            (int)Math.Ceiling(Convert.ToDouble(credits[i].ICreditAmount.GetValueOrDefault(0) /
                        //                                                (credits[i].ISMSEnPrice.GetValueOrDefault(0) *
                        //                                                 rate)));
                        //        varEnSMS -= smsCredit;
                        //    }
                        //    if (varFaSMS > 0)
                        //    {
                        //        smsCredit =
                        //            (int)Math.Ceiling(Convert.ToDouble(credits[i].ICreditAmount.GetValueOrDefault(0) /
                        //                                                (credits[i].ISMSFaPrice.GetValueOrDefault(0) *
                        //                                                 rate)));
                        //        varFaSMS -= smsCredit;
                        //    }
                        //    if (credits.Count > 1)
                        //    {
                        //        i++;
                        //        if (varFaSMS == 0 && varEnSMS == 0 && i <= credits.Count - 1)// darsorati ke tedade row credit baishe 1 bashe 
                        //        {
                        //            varFaSMS = noFaSMS;
                        //            varEnSMS = noEnSMS;
                        //        }
                        //    }
                        //    //i++;
                        //}//while inside
                        #endregion

                        if (varEnSMS > 0 || varFaSMS > 0)
                            return false;
                        //select manager

                        userId = (from p in db.Tbl_UserAccount where p.IUsrId == userId select p.IUsrManagerId).FirstOrDefault();


                        if (userId != null)
                        {
                            userId = (from p in db.Tbl_UserAccount where p.IUsrId == userId select p.IUsrId).FirstOrDefault();
                            if (userId == null)
                                return true;
                        }
                        else
                            return true;




                        credits = (from c in db.UsrCredits
                                   where c.IUsrId == userId && c.ICreditAmount > 0
                                   select c).ToList();



                        if (credits.Count == 0) return false;
                        else return true;


                    }//while outside
                }
                catch (Exception ex)
                {

                    using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\SERVICE_CORE_Exception.txt", true))
                    {

                        outfile.Write(DateTime.Now + "  " + ex.Message + ".:::" + ex.StackTrace +
                                      (!string.IsNullOrEmpty(userId)
                                           ? "UserId for this is :: " + userId
                                           : "Userid is null or empty"));


                    }
                    throw;
                }
            }//using

            //if (noEnSMS > 0 || noFaSMS > 0)
            //    return false;
            return true;
            //        if (noEnSMS > 0)
            //        {

            //            smsCredit = credits[i].ICreditAmount.GetValueOrDefault(0) /
            //                        credits[i].ISMSEnPrice.GetValueOrDefault(0);
            //            if (smsCredit <= noEnSMS)
            //                totalprice += credits[i].ICreditAmount.GetValueOrDefault(0);
            //            else
            //            {
            //                totalprice += credits[i].ISMSEnPrice.GetValueOrDefault(0) * noEnSMS;
            //            }
            //            noEnSMS -= smsCredit;
            //        }
            //         if (noFaSMS > 0)
            //        {
            //            smsCredit = credits[i].ICreditAmount.GetValueOrDefault(0) / credits[i].ISMSFaPrice.GetValueOrDefault(0);
            //            if (smsCredit <= noFaSMS)
            //                totalprice += credits[i].ICreditAmount.GetValueOrDefault(0);
            //            else
            //            {
            //                totalprice += credits[i].ISMSFaPrice.GetValueOrDefault(0) * noFaSMS;
            //            }
            //            noFaSMS -= smsCredit;
            //        }
        }

        public bool HasEnoughCreditForVoiceMessage(int FaSMSCount, int SMSPrice, string userId)
        {

            double rate = 1;
            int i = 0;
            //int varFaSMS, varEnSMS;
            int FaSmsSum = 0;
            //int EnSmsSum = 0;

            using (SMSModelContainer db = new SMSModelContainer())
            {

                try
                {
                    List<UsrCredit> credits = new List<UsrCredit>();

                    credits = (from c in db.UsrCredits
                               where c.IUsrId == userId && c.ICreditAmount > 0
                               select c).ToList();

                    if (credits.Count == 0) return false;

                    // while (credits.Count > 0)
                    //  {
                    //&& i < credits.Count()

                    FaSmsSum = 0;
                    //EnSmsSum = 0;
                    for (int j = 0; j < credits.Count; j++)
                        FaSmsSum += (int)Math.Floor(Convert.ToDouble(credits[j].ICreditAmount.GetValueOrDefault(0) /
                                             (SMSPrice *
                                              rate)));

                    // varFaSMS = noFaSMS;
                    //varEnSMS = noEnSMS;
                    i = 0;
                    if (FaSMSCount != 0) FaSMSCount -= FaSmsSum;
                    string temp_user = userId;
                    if (FaSMSCount > 0)
                        return false;
                    //select manager

                    userId = (from p in db.Tbl_UserAccount where p.IUsrId == userId select p.IUsrManagerId).FirstOrDefault();


                    if (userId != null)
                    {
                        userId = (from p in db.Tbl_UserAccount where p.IUsrId == userId select p.IUsrId).FirstOrDefault();
                        if (userId == null)
                            return true;
                    }
                    else
                        return true;




                    credits = (from c in db.UsrCredits
                               where c.IUsrId == userId && c.ICreditAmount > 0
                               select c).ToList();



                    if (credits.Count == 0) return false;
                    else return true;


                    // }//while outside
                }
                catch (Exception ex)
                {

                    using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\SERVICE_CORE_Exception_HasEnoughCreditForVoiceMessagetxt", true))
                    {

                        outfile.Write(DateTime.Now + "  " + ex.Message + ".:::" + ex.StackTrace +
                                      (!string.IsNullOrEmpty(userId)
                                           ? "UserId for this is :: " + userId
                                           : "Userid is null or empty"));


                    }
                    throw;
                }
            }
            return true;

        }

        public bool HasEnoughCredit(int noEnSMS, int noFaSMS, string userId, string LineNumber, SMSModelContainer model)
        {

            //using (SMSModelContainer model = new SMSModelContainer())


            int tempEN, tempFa;
            string tempUS;
            tempEN = noEnSMS; tempFa = noFaSMS; tempUS = userId;
            //using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\CheckCreditLog.txt", true))
            //{
            //    outfile.Write("noEnSMS :" + noEnSMS.ToString() + " noFaSMS:" + noFaSMS.ToString() + " userId :" + userId + " LineNumber :" + LineNumber + Environment.NewLine +
            //        "############################" + Environment.NewLine);
            //}
            double rate = GetTarrifrateRate(LineNumber);
            List<UsrCredit> credits = new List<UsrCredit>();
            credits = (from c in model.UsrCredits
                       where c.IUsrId == userId && c.ICreditAmount > 0
                       orderby c.PaymentId
                       select c).ToList();
            int i = 0, smsCredit = 0;
            while ((noFaSMS > 0 || noEnSMS > 0) && i < credits.Count())
            {
                if (noEnSMS > 0)
                {
                    smsCredit = (int)Math.Ceiling(Convert.ToDouble(credits[i].ICreditAmount.GetValueOrDefault(0) /
                                (credits[i].ISMSEnPrice.GetValueOrDefault(0) * rate)));
                    noEnSMS -= smsCredit;
                }
                if (noFaSMS > 0)
                {
                    smsCredit = (int)Math.Ceiling(Convert.ToDouble(credits[i].ICreditAmount.GetValueOrDefault(0) /
                               (credits[i].ISMSFaPrice.GetValueOrDefault(0) * rate)));
                    noFaSMS -= smsCredit;
                }
                i++;
            }

            if (noEnSMS > 0 || noFaSMS > 0)
                return false;
            else
            {
                using (AcountingEntities ae = new AcountingEntities())
                {
                    string str_manager = ae.UserAccounts.Where(p => p.IUsrId.Equals(userId)).FirstOrDefault().IUsrManagerId;
                    //using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\CheckCreditLog.txt", true))
                    //{
                    //    outfile.Write("str_manager :" + str_manager + " userId :" + userId + Environment.NewLine +
                    //        "############################" + Environment.NewLine);
                    //}
                    if (string.IsNullOrEmpty(str_manager)) return true;
                    if (str_manager == userId) return true;
                    var admin = ae.UserAccounts.Where(p => p.IUsrId.Equals(str_manager)).FirstOrDefault();
                    if (admin == null)
                        return true;
                    else
                    {
                        return HasEnoughCredit(tempEN, tempFa, admin.IUsrId, LineNumber, model);
                    }
                }
                // return true;
            }
        }
        public bool CheckAdminCredit(int noEnSMS, int noFaSMS, string userId, string LineNumber, SMSModelContainer model)
        {
            bool res = true;
            using (AcountingEntities ae = new AcountingEntities())
            {


                string str_manager = ae.UserAccounts.Where(p => p.IUsrId.Equals(userId)).FirstOrDefault().IUsrManagerId;
                //using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\CheckCreditLog.txt", true))
                //{
                //    outfile.Write("str_manager :" + str_manager + " userId :" + userId + Environment.NewLine +
                //        "############################" + Environment.NewLine);
                //}
                if (string.IsNullOrEmpty(str_manager)) return true;
                var admin = ae.UserAccounts.Where(p => p.IUsrId.Equals(str_manager)).FirstOrDefault();
                if (admin == null)
                    return true;
                else
                {
                    bool hasCredit = HasEnoughCredit(noEnSMS, noFaSMS, admin.IUsrId, LineNumber, model);
                    //using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\CheckCreditLog.txt", true))
                    //{
                    //    outfile.Write("IUsrId :" + admin.IUsrId + " hasCredit :" + hasCredit.ToString() + Environment.NewLine +
                    //        "############################" + Environment.NewLine);
                    //}
                    if (!hasCredit) return false;
                    else if (admin.IUsrId.ToLower() == "ssb240") return true;
                    else
                        CheckAdminCredit(noEnSMS, noFaSMS, admin.IUsrManagerId, LineNumber, model);
                }

            }
            return res;
        }
        public bool HasEnoughCredit(int price, string userId, string LineNumber)
        {
            using (SMSModelContainer db = new SMSModelContainer())
            {
                int CreditSum;
                CreditSum = (from c in db.UsrCredits.Include("Tbl_Payment")
                             where c.IUsrId == userId && c.ICreditAmount > 0 && c.Tbl_Payment.PayFlag.HasValue && c.Tbl_Payment.PayFlag.Value
                             select c).Sum(p => p.ICreditAmount.Value);
                if (CreditSum < price)
                    return false;
            }
            return true;
        }

        //public int UpdateCredit(List<UsrCredit> credits, int noEnSMS, int noFaSMS, string userId)
        //{
        //    int i = 0, smsCredit = 0;
        //    using (SMSModelContainer ctx = new SMSModelContainer())
        //    {
        //        while ((noFaSMS > 0 || noEnSMS > 0) && i < credits.Count())
        //        {
        //            int smsPrice = 0, noSms = 0;
        //            if (noEnSMS > 0)
        //            {
        //                smsCredit = credits[i].ICreditAmount.GetValueOrDefault(0) /
        //                            credits[i].ISMSEnPrice.GetValueOrDefault(0);
        //                smsPrice = credits[i].ISMSEnPrice.GetValueOrDefault(0);
        //                noSms = noEnSMS;
        //                noEnSMS -= smsCredit;
        //            }
        //            else if (noFaSMS > 0)
        //            {
        //                smsCredit = credits[i].ICreditAmount.GetValueOrDefault(0) /
        //                            credits[i].ISMSFaPrice.GetValueOrDefault(0);
        //                smsPrice = credits[i].ISMSFaPrice.GetValueOrDefault(0);
        //                noSms = noFaSMS;
        //                noFaSMS -= smsCredit;
        //            }

        //            //اگر کردیت بیشتر از نیاز
        //            if (smsCredit > noSms)
        //            {
        //                int cost = smsPrice * noSms;
        //                credits[i].ICreditAmount = credits[i].ICreditAmount - cost;
        //                credits[i].MarkAsModified();
        //            }
        //            else
        //            {
        //                credits[i].ICreditAmount = 0;
        //                credits[i].MarkAsModified();
        //            }
        //            ctx.ApplyChanges("UsrCredits", credits[i]);
        //            ctx.SaveChanges();
        //            i++;
        //        }
        //    }
        //    //شاید منفی باشد؟؟؟
        //    return noEnSMS + noFaSMS;
        //}
        //public int UpdateCredit(List<UsrCredit> credits, int noEnSMS, int noFaSMS, string userId, SMSModelContainer ctx)
        //{
        //    int i = 0, smsCredit = 0;
        //    //using (SMSModelContainer ctx = new SMSModelContainer())
        //    {
        //        while ((noFaSMS > 0 || noEnSMS > 0) && i < credits.Count())
        //        {
        //            int smsPrice = 0, noSms = 0;
        //            if (noEnSMS > 0)
        //            {
        //                smsCredit = credits[i].ICreditAmount.GetValueOrDefault(0) /
        //                            credits[i].ISMSEnPrice.GetValueOrDefault(0);
        //                smsPrice = credits[i].ISMSEnPrice.GetValueOrDefault(0);
        //                noSms = noEnSMS;
        //                noEnSMS -= smsCredit;
        //            }
        //            else if (noFaSMS > 0)
        //            {
        //                smsCredit = credits[i].ICreditAmount.GetValueOrDefault(0) /
        //                            credits[i].ISMSFaPrice.GetValueOrDefault(0);
        //                smsPrice = credits[i].ISMSFaPrice.GetValueOrDefault(0);
        //                noSms = noFaSMS;
        //                noFaSMS -= smsCredit;
        //            }

        //            //اگر کردیت بیشتر از نیاز
        //            if (smsCredit > noSms)
        //            {
        //                int cost = smsPrice * noSms;
        //                credits[i].ICreditAmount = credits[i].ICreditAmount - cost;
        //                credits[i].MarkAsModified();
        //            }
        //            else
        //            {
        //                credits[i].ICreditAmount = 0;
        //                credits[i].MarkAsModified();
        //            }
        //            ctx.ApplyChanges("UsrCredits", credits[i]);
        //            ctx.SaveChanges();
        //            i++;
        //        }
        //    }
        //    //شاید منفی باشد؟؟؟
        //    return noEnSMS + noFaSMS;
        //}

        public int UpdateCredit(List<UsrCredit> credits, int noEnSMS, int noFaSMS, string userId, string lineNumber, byte TransActionType)
        {
            int i = 0, smsCredit = 0;
            double rate = GetTarrifrateRate(lineNumber);
            using (SMSModelContainer ctx = new SMSModelContainer())
            {
                return ctx.SpTbl_Usrcredit_UpdateCredit(userId, noFaSMS, noEnSMS, Helpers.Utility.PersianNowDate, rate, TransActionType).First().GetValueOrDefault(0);
            }
            //شاید منفی باشد؟؟؟
            return noEnSMS + noFaSMS;
        }
        public int UpdateCredit(List<UsrCredit> credits, int noEnSMS, int noFaSMS, string userId, SMSModelContainer ctx, string lineNumber, byte TransActionType)
        {
            double rate = GetTarrifrateRate(lineNumber);
            return ctx.SpTbl_Usrcredit_UpdateCredit(userId, noFaSMS, noEnSMS, Helpers.Utility.PersianNowDate, rate, TransActionType).First().GetValueOrDefault(0);
            //int i = 0, smsCredit = 0;
            ////using (SMSModelContainer ctx = new SMSModelContainer())
            //{
            //    while ((noFaSMS > 0 || noEnSMS > 0) && i < credits.Count())
            //    {
            //        int smsPrice = 0, noSms = 0;
            //        if (noEnSMS > 0)
            //        {
            //            smsCredit = credits[i].ICreditAmount.GetValueOrDefault(0) /
            //                        credits[i].ISMSEnPrice.GetValueOrDefault(0);
            //            smsPrice = credits[i].ISMSEnPrice.GetValueOrDefault(0);
            //            noSms = noEnSMS;
            //            noEnSMS -= smsCredit;
            //        }
            //        else if (noFaSMS > 0)
            //        {
            //            smsCredit = credits[i].ICreditAmount.GetValueOrDefault(0) /
            //                        credits[i].ISMSFaPrice.GetValueOrDefault(0);
            //            smsPrice = credits[i].ISMSFaPrice.GetValueOrDefault(0);
            //            noSms = noFaSMS;
            //            noFaSMS -= smsCredit;
            //        }

            //        //اگر کردیت بیشتر از نیاز
            //        if (smsCredit > noSms)
            //        {
            //            int cost = (int)Math.Ceiling(Convert.ToDouble((smsPrice * noSms)) * rate);
            //            credits[i].ICreditAmount = credits[i].ICreditAmount - cost;
            //            credits[i].MarkAsModified();
            //        }
            //        else
            //        {
            //            credits[i].ICreditAmount = 0;
            //            credits[i].MarkAsModified();
            //        }
            //        ctx.ApplyChanges("UsrCredits", credits[i]);
            //        ctx.SaveChanges();
            //        i++;
            //    }
            //}
            ////شاید منفی باشد؟؟؟
            //return noEnSMS + noFaSMS;
        }

        private int IncreaseCredit(List<UsrCredit> updatedCredits, int enCount, int faCount, List<UsrCredit> list)
        {
            SMSModelContainer ctx = new SMSModelContainer();
            int i = 0; int noSMS = 0, smsCredit = 0;
            while ((enCount > 0 || faCount > 0) && i < updatedCredits.Count)
            {

                UsrCredit ncr = new UsrCredit(); UsrCredit cr = updatedCredits[i];
                int tafazol = cr.ICreditAmount.GetValueOrDefault(0) - list.First(x => x.PaymentId == cr.PaymentId).ICreditAmount.GetValueOrDefault(0);
                if (enCount > 0)
                {
                    smsCredit = tafazol / cr.ISMSEnPrice.GetValueOrDefault(0);
                    if (smsCredit > enCount)
                    {
                        cr.ICreditAmount += enCount * cr.ISMSEnPrice.GetValueOrDefault(0);
                        enCount -= smsCredit;
                    }
                    else
                    {
                        cr.ICreditAmount += smsCredit * cr.ISMSEnPrice.GetValueOrDefault(0);
                        enCount -= smsCredit;
                    }
                    cr.MarkAsModified();
                }
                else if (faCount > 0)
                {
                    smsCredit = tafazol / cr.ISMSFaPrice.GetValueOrDefault(0);
                    if (smsCredit > faCount)
                    {
                        cr.ICreditAmount += faCount * cr.ISMSFaPrice.GetValueOrDefault(0);
                        faCount -= smsCredit;
                    }
                    else
                    {
                        cr.ICreditAmount += smsCredit * cr.ISMSFaPrice.GetValueOrDefault(0);
                        faCount -= smsCredit;
                    }
                    cr.MarkAsModified();
                }

                ctx.ApplyChanges("UsrCredits", cr);
                ctx.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                i++;
            }
            return enCount + faCount;
        }
        public Guid[] InsertToSentSMSForVAS(string[] Mobiles, string message, string origs, string userId, int operatorId, int VasServiceId)
        {
            DataTable Retdt = new DataTable();
            Retdt = Utility.GetBuklDtAccountingForVas();

            string SendingDate = Utility.PersianNowDate;
            string SendingTime = DateTime.Now.ToShortTimeString().ToString();
            Guid[] Ids = new Guid[Mobiles.Length];
            int priority = 0;
            priority = SetPriority(Mobiles.Length, userId);
            for (int i = 0; i < Mobiles.Length; i++)
            {
                Ids[i] = Guid.NewGuid();
                object[] NewRow;
                long test;
                bool canParse = long.TryParse(Utility.SetPrefixNum(Mobiles[i]), out test);
                if (canParse)
                    NewRow = Utility.NewBulkTableRowAccountingForVAS(-1, message, SendingDate,
                                                                      DateTime.Now.ToShortTimeString(), origs, Mobiles[i],
                                                                      SentSmsType.SuccessefulSent, userId, Ids[i], operatorId, priority, VasServiceId);
                else
                    NewRow = Utility.NewBulkTableRowAccountingForVAS(-1, message, SendingDate,
                                                                      DateTime.Now.ToShortTimeString(), origs, "0",
                                                                      SentSmsType.SuccessefulSent, userId, Ids[i], operatorId, priority, VasServiceId);
                Retdt.Rows.Add(NewRow);
            }
            EntityConnection cn = new EntityConnection("name=SMSModelContainer");
            using (SqlConnection sqlConn = new SqlConnection(cn.StoreConnection.ConnectionString))
            {
                new Utility().BulkInsertSentSmsAccountingForVAS(Retdt, sqlConn);

            }
            return Ids;
        }
        public Guid[] InsertToSentSMS(string[] Mobiles, string message, string origs, string userId, int operatorId)
        {
            DataTable Retdt = new DataTable();
            Retdt = Utility.GetBuklDtAccounting();

            string SendingDate = Utility.PersianNowDate;
            string SendingTime = DateTime.Now.ToShortTimeString().ToString();
            Guid[] Ids = new Guid[Mobiles.Length];
            int priority = 0;
            priority = SetPriority(Mobiles.Length, userId);
            for (int i = 0; i < Mobiles.Length; i++)
            {
                Ids[i] = Guid.NewGuid();
                object[] NewRow;
                long test;
                bool canParse = long.TryParse(Utility.SetPrefixNum(Mobiles[i]), out test);
                if (canParse)
                    NewRow = Utility.NewBulkTableRowAccounting(-1, message, SendingDate,
                                                                      DateTime.Now.ToShortTimeString(), origs, Mobiles[i],
                                                                      SentSmsType.SuccessefulSent, userId, Ids[i], operatorId, priority);
                else
                    NewRow = Utility.NewBulkTableRowAccounting(-1, message, SendingDate,
                                                                      DateTime.Now.ToShortTimeString(), origs, "0",
                                                                      SentSmsType.SuccessefulSent, userId, Ids[i], operatorId, priority);
                Retdt.Rows.Add(NewRow);
            }
            EntityConnection cn = new EntityConnection("name=SMSModelContainer");
            using (SqlConnection sqlConn = new SqlConnection(cn.StoreConnection.ConnectionString))
            {
                new Utility().BulkInsertSentSmsAccounting(Retdt, sqlConn);

            }
            return Ids;
        }
        public string[] SendSMSQueue(string message, string[] Mobiles, string origs, string userId)
        {
            //using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\err.txt", true))
            //{
            //    outfile.Write("from NewSmsWebserivce::" + DateTime.Now.ToString() + "::" + ex.Message + "::" + ex.StackTrace + Environment.NewLine);
            //}
          
            int operatorId = GetOperatorId(origs);
            int enSms = 0, faSms = 0;
            // آیا پیامک فارسی یا انگلیسی ،حتما درست شود؟؟؟؟
            string[] TempMobile = SetValidNumber(Mobiles);

            if (Utility.IsFarsiSMS(message))
                faSms = Utility.GetSmsPartCount(message.Length, Utility.FarsiSMSLength, origs) * TempMobile.Length;
            else
                enSms += Utility.GetSmsPartCount(message.Length, Utility.EnglishSMSLength, origs) * TempMobile.Length;
            bool hasCredit = false;
            List<UsrCredit> list = null;
            List<UsrCredit> Originallist = new List<UsrCredit>();
            int remainedSMS = 0;
            using (SMSModelContainer model = new SMSModelContainer())
            {
                hasCredit = HasEnoughCredit(enSms, faSms, userId, origs, model);

                if (!hasCredit)
                    return new string[1] { "14" };
                list = (from c in model.UsrCredits
                        where c.IUsrId == userId && c.ICreditAmount > 0
                        select c).ToList();

                foreach (var lt in list)
                {
                    Originallist.Add((UsrCredit)lt.Clone());
                }
                // کم کردن اعتبار

                remainedSMS = UpdateCredit(list, enSms, faSms, userId, model, origs.ToString(), (byte)Helpers.TransactionType.SendWebService);
            }

            Guid[] Ids = new Guid[Mobiles.Length];
            Ids = InsertToSentSMS(Mobiles, message, origs, userId, operatorId);
            return Array.ConvertAll(Ids, x => x.ToString());
        }

        public string[] SendSMSQueue(string[] Messages, string[] Mobiles, string[] origs, string UserId)
        {
            Guid[] Ids = new Guid[Mobiles.Length];
            try
            {


                int operatorId = GetOperatorId(origs[0]);
                // چک  شود که تعداد encodingبا messages برابر باشد
                if (Messages.Length != Mobiles.Length || origs.Length != Messages.Length)
                    return new string[1] { "103" };

                //foreach (var i in Encodings)
                //{
                //    if (i == 0)
                //        ml.Add( MessageLang.Farsi);
                //    else
                //    {
                //        ml.Add(MessageLang.English);
                //    }
                //}
                int enSms = 0, faSms = 0;
                for (int i = 0; i < Messages.Length; i++)
                {
                    // آیا پیامک فارسی یا انگلیسی ،حتما درست شود؟؟؟؟
                    if (SSB.Helpers.Utility.IsFarsiSMS(Messages[i]))
                        faSms += Utility.GetSmsPartCount(Messages[i].Length, Utility.FarsiSMSLength, origs[0]);
                    else
                        enSms += Utility.GetSmsPartCount(Messages[i].Length, Utility.EnglishSMSLength, origs[0]);
                }
                string[] TempMobile = SetValidNumber(Mobiles);

                if (Messages.Length == 1)
                {
                    enSms = enSms * TempMobile.Length;
                    faSms = faSms * TempMobile.Length;
                }
                //ایا کاربر اعتبار کافی دارد؟
                bool hasCredit = false;
                List<UsrCredit> list = null;
                List<UsrCredit> Originallist = new List<UsrCredit>();
                using (SMSModelContainer model = new SMSModelContainer())
                {
                   
                    hasCredit = HasEnoughCredit(enSms, faSms, UserId, origs[0], model);


                    if (!hasCredit)
                        return new string[1] { "14" };


                    list = (from c in model.UsrCredits
                            where c.IUsrId == UserId && c.ICreditAmount > 0
                            select c).ToList();

                    foreach (var lt in list)
                    {
                        Originallist.Add((UsrCredit)lt.Clone());
                    }
                    // کم کردن اعتبار
                    
                    int remainedSMS = UpdateCredit(list, enSms, faSms, UserId, model, origs[0], (byte)Helpers.TransactionType.SendWebService);
                }
              
                DataTable Retdt = new DataTable();
                Retdt = Utility.GetBuklDtAccounting();
                string SendingDate = Utility.PersianNowDate;
                string SendingTime = DateTime.Now.ToShortTimeString().ToString();

                int priority = 0;
                priority = SetPriority(Mobiles.Length, UserId);
                for (int i = 0; i < Messages.Length; i++)
                {
                    Ids[i] = Guid.NewGuid();
                    object[] NewRow = Utility.NewBulkTableRowAccounting(-1, Messages[i], SendingDate, SendingTime,
                                                      origs[i], Mobiles[i], SentSmsType.SuccessefulSent, UserId, Ids[i], operatorId, priority);
                    Retdt.Rows.Add(NewRow);
                }
              
                EntityConnection cn = new EntityConnection("name=SMSModelContainer");
                using (SqlConnection sqlConn = new SqlConnection(cn.StoreConnection.ConnectionString))
                {
                    new Utility().BulkInsertSentSmsAccounting(Retdt, sqlConn);

                }
               
            }
            catch (Exception ex)
            {

                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\ArraySendSMSQueue_Error.txt", true))
                {
                    outfile.Write(DateTime.Now.ToString() + "   Ex.Message :" + ex.Message + Environment.NewLine + "StackTrace :" + ex.StackTrace + Environment.NewLine + " InnerEx :" +
                        (ex.InnerException == null ? "null" : ex.InnerException.ToString())
                        + Environment.NewLine +
                        "############################" + Environment.NewLine);
                }
            }
            return Array.ConvertAll(Ids, x => x.ToString());
        }

        public string[] SetValidNumber(string[] Mobiles)
        {
            string[] TempMobile = Mobiles;
            try
            {
                for (int i = 0; i < TempMobile.Length; i++)
                {
                    TempMobile[i] = Helpers.Utility.SetPrefixNum(TempMobile[i]);
                    if (TempMobile[i].StartsWith("9")) TempMobile[i] = "0" + TempMobile[i];
                    if (!TempMobile[i].StartsWith("09"))
                        TempMobile[i] = string.Empty;
                    if (TempMobile[i].Length != 11)
                        TempMobile[i] = string.Empty;
                    long test;
                    bool canParse = long.TryParse(TempMobile[i], out test);
                    if (!canParse)
                        TempMobile[i] = string.Empty;
                }
                TempMobile = (from p in TempMobile where !string.IsNullOrEmpty(p) select p).ToArray();
            }
            catch (Exception)
            {

                //throw;
            }
            return TempMobile;
        }

        public string[] SendSMSQueueWithId(Guid[] Ids, string[] Messages, string[] Mobiles, string[] origs, string UserId)
        {
            int operatorId = GetOperatorId(origs[0]);
            // چک  شود که تعداد encodingبا messages برابر باشد
            if (Messages.Length != Mobiles.Length || origs.Length != Messages.Length)
                return new string[1] { "3" };

            //foreach (var i in Encodings)
            //{
            //    if (i == 0)
            //        ml.Add( MessageLang.Farsi);
            //    else
            //    {
            //        ml.Add(MessageLang.English);
            //    }
            //}
            int enSms = 0, faSms = 0;
            for (int i = 0; i < Messages.Length; i++)
            {
                // آیا پیامک فارسی یا انگلیسی ،حتما درست شود؟؟؟؟
                if (SSB.Helpers.Utility.IsFarsiSMS(Messages[i]))
                    faSms += Utility.GetSmsPartCount(Messages[i].Length, Utility.FarsiSMSLength, origs[0]);
                else
                    enSms += Utility.GetSmsPartCount(Messages[i].Length, Utility.EnglishSMSLength, origs[0]);
            }
            string[] TempMobile = SetValidNumber(Mobiles);
            if (Messages.Length == 1)
            {
                enSms = enSms * TempMobile.Length;
                faSms = faSms * TempMobile.Length;
            }
            //ایا کاربر اعتبار کافی دارد؟
            bool hasCredit = false;
            List<UsrCredit> list = null;
            List<UsrCredit> Originallist = new List<UsrCredit>();
            using (SMSModelContainer model = new SMSModelContainer())
            {
                hasCredit = HasEnoughCredit(enSms, faSms, UserId, origs[0], model);

                if (!hasCredit)
                    return new string[1] { "14" };
                list = (from c in model.UsrCredits
                        where c.IUsrId == UserId && c.ICreditAmount > 0
                        select c).ToList();

                foreach (var lt in list)
                {
                    Originallist.Add((UsrCredit)lt.Clone());
                }
                // کم کردن اعتبار

                int remainedSMS = UpdateCredit(list, enSms, faSms, UserId, model, origs[0], (byte)Helpers.TransactionType.SendWebService);
            }
            DataTable Retdt = new DataTable();
            Retdt = Utility.GetBuklDtAccounting();
            string SendingDate = Utility.PersianNowDate;
            string SendingTime = DateTime.Now.ToShortTimeString().ToString();
            //Guid[] Ids = new Guid[Mobiles.Length];
            int priority = 0;
            priority = SetPriority(Mobiles.Length, UserId);
            for (int i = 0; i < Messages.Length; i++)
            {
                //Ids[i] = Guid.NewGuid();
                object[] NewRow = Utility.NewBulkTableRowAccounting(-1, Messages[i], SendingDate, SendingTime,
                                                  origs[i], Mobiles[i], SentSmsType.SuccessefulSent, UserId, Ids[i], operatorId, priority);
                Retdt.Rows.Add(NewRow);
            }

            EntityConnection cn = new EntityConnection("name=SMSModelContainer");
            using (SqlConnection sqlConn = new SqlConnection(cn.StoreConnection.ConnectionString))
            {
                new Utility().BulkInsertSentSmsAccounting(Retdt, sqlConn);

            }
            return Array.ConvertAll(Ids, x => x.ToString());
        }
        //get message status from magfa
        public int[] getMessageStatusFromMagfa(long[] id)
        {
            int[] res ;
            MagfaService.SoapSmsQueuableImplementationService s = new SoapSmsQueuableImplementationService();

            s.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["MagfaUsrName"],
                                                  ConfigurationManager.AppSettings["MagfaPassword"]);
            res = s.getMessageStatuses(id);
            return res;
        }
        //send to magfa
        public long[] SendSMS(string[] Messages, int[] Encodings, string[] Mobiles, string[] origs, string[] UDH, int[] MessageClass, int[] Priorities, long[] CheckingIds, string UserId)
        {
            // چک  شود که تعداد encodingبا messages برابر باشد
            if (Messages.Length != Encodings.Length)
                return new long[1] { (long)3 };

            string[] TempMobile = SetValidNumber(Mobiles);
            List<string> tmpMessages = new List<string>();
            List<int> tmpEncodings = new List<int>();
            List<string> tmpMobiles = new List<string>();
            List<string> tmpOrigs = new List<string>();
            List<string> tmpUDH = new List<string>();
            List<int> tmpMessageClass = new List<int>();
            List<int> tmpPriorities = new List<int>();
            List<long> tmpCheckingIds = new List<long>();
            for (int i = 0; i < Mobiles.Length; i++)
            {
                if (TempMobile.Contains(Mobiles[i]))
                {
                    tmpMessages.Add(Messages[i]);
                    tmpEncodings.Add(Encodings[i]);
                    tmpMobiles.Add(Mobiles[i]);
                    tmpOrigs.Add(origs[i]);
                    tmpUDH.Add(UDH[i]);
                    tmpMessageClass.Add(MessageClass[i]);
                    tmpPriorities.Add(Priorities[i]);
                    tmpCheckingIds.Add(CheckingIds[i]);
                }
            }
            if (TempMobile.Length > 0)
            {
                int enSms = 0, faSms = 0;
                for (int i = 0; i < Messages.Length; i++)
                {
                    // آیا پیامک فارسی یا انگلیسی ،حتما درست شود؟؟؟؟
                    if (SSB.Helpers.Utility.IsFarsiSMS(tmpMessages[i]))
                        faSms += Utility.GetSmsPartCount(tmpMessages[i].Length, Utility.FarsiSMSLength, origs[0]);
                    else
                        enSms += Utility.GetSmsPartCount(tmpMessages[i].Length, Utility.EnglishSMSLength, origs[0]);
                }

                if (Messages.Length == 1)
                {
                    enSms = enSms * TempMobile.Length;
                    faSms = faSms * TempMobile.Length;
                }
                //ایا کاربر اعتبار کافی دارد؟
                bool hasCredit = false;
                List<UsrCredit> list = null;
                List<UsrCredit> Originallist = new List<UsrCredit>();
                using (SMSModelContainer model = new SMSModelContainer())
                {
                    hasCredit = HasEnoughCredit(enSms, faSms, UserId, origs[0], model);

                    if (!hasCredit)
                        return new long[1] { (long)14 };
                    list = (from c in model.UsrCredits
                            where c.IUsrId == UserId && c.ICreditAmount > 0
                            select c).ToList();

                    foreach (var lt in list)
                    {
                        Originallist.Add((UsrCredit)lt.Clone());
                    }
                    // کم کردن اعتبار

                    int remainedSMS = UpdateCredit(list, enSms, faSms, UserId, model, origs[0], (byte)Helpers.TransactionType.SendWebService);
                }

                MagfaService.SoapSmsQueuableImplementationService s = new SoapSmsQueuableImplementationService();

                s.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["MagfaUsrName"],
                                                      ConfigurationManager.AppSettings["MagfaPassword"]);

                long[] results;
                try
                {
                    
                    results = s.enqueue(ConfigurationManager.AppSettings["MagfaDomain"], tmpMessages.ToArray(),
                                        tmpMobiles.ToArray(), tmpOrigs.ToArray(), tmpEncodings.ToArray(), tmpUDH.ToArray(),
                                        tmpMessageClass.ToArray(), tmpPriorities.ToArray(), tmpCheckingIds.ToArray());
                    //using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\newsmswebService.txt", true))
                    //{
                    //    outfile.Write("UserId :"+UserId+Environment.NewLine+"results from magfa :" + ((results!=null)?results[0].ToString():"null") + Environment.NewLine + 
                    //        "==============" + Environment.NewLine
                    //       );
                    //}

                }
                catch (Exception ex)
                {
                    using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\newsmswebService_error.txt", true))
                    {
                        outfile.Write(DateTime.Now + "    UserId :" + UserId + Environment.NewLine + "message :" + ex.Message + Environment.NewLine + " inner ex :" + (ex.InnerException != null ? ex.InnerException.ToString() : "null") +
                            Environment.NewLine + "==============" + Environment.NewLine
                             );
                    }
                    results = null;
                }
                if (results != null)
                {
                    List<long> rs = results.ToList();
                    var fl = (from j in rs
                              where j <= 110
                              select new { member = j, encoding = Encodings[rs.IndexOf(j)] }).ToList();

                    //اگر پیامکی ارسال نشده بود
                    if (fl.Count > 0)
                    {
                        int enCount = fl.Count(x => x.encoding == 1);
                        int faCount = fl.Count(x => x.encoding != 1);
                        List<UsrCredit> newCredits = new List<UsrCredit>();
                        var updatedCredits = Originallist.Where(x => !list.Select(c => c.PaymentId).Contains(x.PaymentId) ||
                                                                     (list.Any(b => b.PaymentId == x.PaymentId) &&
                                                                      list.FirstOrDefault(b => b.PaymentId == x.PaymentId).
                                                                          ICreditAmount !=
                                                                      x.ICreditAmount)).OrderByDescending(x => x.PaymentId).
                            ToList();
                        //Increase Amount of Credit
                        IncreaseCredit(updatedCredits, enCount, faCount, list);
                    }
                }
                else
                {
                    IncreaseCredit(new List<UsrCredit>(), enSms, faSms, list);
                }
                DataTable Retdt = new DataTable();
                DataColumn col_smsText = new DataColumn();
                DataColumn col_SendingDate = new DataColumn();
                DataColumn col_Sendingtime = new DataColumn();
                DataColumn col_SenderNumber = new DataColumn();
                DataColumn col_RecieverNumber = new DataColumn();
                DataColumn col_Sendingstatus = new DataColumn();
                DataColumn col_MagfaId = new DataColumn();
                DataColumn Col_UsrId = new DataColumn();
                DataColumn Col_StatusCode = new DataColumn();
                //DataColumn Col_Id = new DataColumn();
                col_smsText.ColumnName = "ISMSText";
                col_SendingDate.ColumnName = "ISendingDate";
                col_Sendingtime.ColumnName = "ISendingTime";
                col_SenderNumber.ColumnName = "ISenderNumber";
                col_RecieverNumber.ColumnName = "IReciverNumber";
                col_Sendingstatus.ColumnName = "ISMSStatuse";
                col_MagfaId.ColumnName = "MagfaSmsId";
                Col_UsrId.ColumnName = "IUsrId";
                Col_StatusCode.ColumnName = "ISMSStatusCode";
                //Col_Id.ColumnName = "Id";
                //DataColumn[] columns = { col_MagfaId, col_smsText, col_SendingDate, col_Sendingtime, col_SenderNumber, col_RecieverNumber, col_Sendingstatus, Col_UsrId, Col_StatusCode, Col_Id };
                DataColumn[] columns = {
                                       col_MagfaId, col_smsText, col_SendingDate, col_Sendingtime, col_SenderNumber,
                                       col_RecieverNumber, col_Sendingstatus, Col_UsrId, Col_StatusCode
                                   };
                Retdt.Columns.AddRange(columns);
                string SendingDate = Utility.PersianNowDate;
                string SendingTime = DateTime.Now.ToShortTimeString().ToString();
                for (int i = 0; i < tmpMessages.Count; i++)
                {
                    long test;
                    bool canParse = long.TryParse(Utility.SetPrefixNum(tmpMobiles[i]), out test);
                    if (canParse)
                    {
                        object[] NewRow = {
                                              results[i], tmpMessages[i], SendingDate,
                                              DateTime.Now.ToShortTimeString().ToString(),
                                              tmpOrigs[i], tmpMobiles[i], "", UserId, (int) SentSmsType.SuccessefulSent
                                          };
                        Retdt.Rows.Add(NewRow);
                    }
                    else
                    {
                        object[] NewRow = {
                                              results[i], tmpMessages[i], SendingDate,
                                              DateTime.Now.ToShortTimeString().ToString(),
                                              tmpOrigs[i], "0", "", UserId, (int) SentSmsType.SuccessefulSent
                                          };
                        Retdt.Rows.Add(NewRow);
                    }
                }
                BulkInsertSentSms(Retdt);
                return results;
            }
            else
                return new long[1] { (long)111 };
        }
        public long[] SendSMSForArraySendMagfa(string[] Messages, int[] Encodings, string[] Mobiles, string[] origs, string[] UDH, int[] MessageClass, int[] Priorities, long[] CheckingIds, string UserId)
        {
            // چک  شود که تعداد encodingبا messages برابر باشد
            if (Messages.Length != Encodings.Length)
                return new long[1] { (long)3 };

            string[] TempMobile = Mobiles;
            List<string> tmpMessages = new List<string>();
            List<int> tmpEncodings = new List<int>();
            List<string> tmpMobiles = new List<string>();
            List<string> tmpOrigs = new List<string>();
            List<string> tmpUDH = new List<string>();
            List<int> tmpMessageClass = new List<int>();
            List<int> tmpPriorities = new List<int>();
            List<long> tmpCheckingIds = new List<long>();
            for (int i = 0; i < Mobiles.Length; i++)
            {
                if (TempMobile.Contains(Mobiles[i]))
                {
                    tmpMessages.Add(Messages[i]);
                    tmpEncodings.Add(Encodings[i]);
                    tmpMobiles.Add(Mobiles[i]);
                    tmpOrigs.Add(origs[i]);
                    tmpUDH.Add(UDH[i]);
                    tmpMessageClass.Add(MessageClass[i]);
                    tmpPriorities.Add(Priorities[i]);
                    tmpCheckingIds.Add(CheckingIds[i]);
                }
            }
            if (TempMobile.Length > 0)
            {
                int enSms = 0, faSms = 0;
                for (int i = 0; i < Messages.Length; i++)
                {
                    // آیا پیامک فارسی یا انگلیسی ،حتما درست شود؟؟؟؟
                    if (SSB.Helpers.Utility.IsFarsiSMS(tmpMessages[i]))
                        faSms += Utility.GetSmsPartCount(tmpMessages[i].Length, Utility.FarsiSMSLength, origs[0]);
                    else
                        enSms += Utility.GetSmsPartCount(tmpMessages[i].Length, Utility.EnglishSMSLength, origs[0]);
                }

                if (Messages.Length == 1)
                {
                    enSms = enSms * TempMobile.Length;
                    faSms = faSms * TempMobile.Length;
                }
                //ایا کاربر اعتبار کافی دارد؟
                bool hasCredit = false;
                List<UsrCredit> list = null;
                List<UsrCredit> Originallist = new List<UsrCredit>();
                using (SMSModelContainer model = new SMSModelContainer())
                {
                    hasCredit = HasEnoughCredit(enSms, faSms, UserId, origs[0], model);

                    if (!hasCredit)
                        return new long[1] { (long)14 };
                    list = (from c in model.UsrCredits
                            where c.IUsrId == UserId && c.ICreditAmount > 0
                            select c).ToList();

                    foreach (var lt in list)
                    {
                        Originallist.Add((UsrCredit)lt.Clone());
                    }
                    // کم کردن اعتبار

                    int remainedSMS = UpdateCredit(list, enSms, faSms, UserId, model, origs[0], (byte)Helpers.TransactionType.SendWebService);
                }

                MagfaService.SoapSmsQueuableImplementationService s = new SoapSmsQueuableImplementationService();

                s.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["MagfaUsrName"],
                                                      ConfigurationManager.AppSettings["MagfaPassword"]);

                long[] results;
                try
                {
                    results = s.enqueue(ConfigurationManager.AppSettings["MagfaDomain"], tmpMessages.ToArray(),
                                        tmpMobiles.ToArray(), tmpOrigs.ToArray(), tmpEncodings.ToArray(), tmpUDH.ToArray(),
                                        tmpMessageClass.ToArray(), tmpPriorities.ToArray(), tmpCheckingIds.ToArray());
                    //using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\newsmswebService.txt", true))
                    //{
                    //    outfile.Write("UserId :"+UserId+Environment.NewLine+"results from magfa :" + ((results!=null)?results[0].ToString():"null") + Environment.NewLine + 
                    //        "==============" + Environment.NewLine
                    //       );
                    //}

                }
                catch (Exception ex)
                {
                    using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\newsmswebService_error.txt", true))
                    {
                        outfile.Write(DateTime.Now + "    UserId :" + UserId + Environment.NewLine + "message :" + ex.Message + Environment.NewLine + " inner ex :" + (ex.InnerException != null ? ex.InnerException.ToString() : "null") +
                            Environment.NewLine + "==============" + Environment.NewLine
                             );
                    }
                    results = null;
                }
                if (results != null)
                {
                    List<long> rs = results.ToList();
                    var fl = (from j in rs
                              where j <= 110
                              select new { member = j, encoding = Encodings[rs.IndexOf(j)] }).ToList();

                    //اگر پیامکی ارسال نشده بود
                    if (fl.Count > 0)
                    {
                        int enCount = fl.Count(x => x.encoding == 1);
                        int faCount = fl.Count(x => x.encoding != 1);
                        List<UsrCredit> newCredits = new List<UsrCredit>();
                        var updatedCredits = Originallist.Where(x => !list.Select(c => c.PaymentId).Contains(x.PaymentId) ||
                                                                     (list.Any(b => b.PaymentId == x.PaymentId) &&
                                                                      list.FirstOrDefault(b => b.PaymentId == x.PaymentId).
                                                                          ICreditAmount !=
                                                                      x.ICreditAmount)).OrderByDescending(x => x.PaymentId).
                            ToList();
                        //Increase Amount of Credit
                        IncreaseCredit(updatedCredits, enCount, faCount, list);
                    }
                }
                else
                {
                    IncreaseCredit(new List<UsrCredit>(), enSms, faSms, list);
                }
                DataTable Retdt = new DataTable();
                DataColumn col_smsText = new DataColumn();
                DataColumn col_SendingDate = new DataColumn();
                DataColumn col_Sendingtime = new DataColumn();
                DataColumn col_SenderNumber = new DataColumn();
                DataColumn col_RecieverNumber = new DataColumn();
                DataColumn col_Sendingstatus = new DataColumn();
                DataColumn col_MagfaId = new DataColumn();
                DataColumn Col_UsrId = new DataColumn();
                DataColumn Col_StatusCode = new DataColumn();
                //DataColumn Col_Id = new DataColumn();
                col_smsText.ColumnName = "ISMSText";
                col_SendingDate.ColumnName = "ISendingDate";
                col_Sendingtime.ColumnName = "ISendingTime";
                col_SenderNumber.ColumnName = "ISenderNumber";
                col_RecieverNumber.ColumnName = "IReciverNumber";
                col_Sendingstatus.ColumnName = "ISMSStatuse";
                col_MagfaId.ColumnName = "MagfaSmsId";
                Col_UsrId.ColumnName = "IUsrId";
                Col_StatusCode.ColumnName = "ISMSStatusCode";
                //Col_Id.ColumnName = "Id";
                //DataColumn[] columns = { col_MagfaId, col_smsText, col_SendingDate, col_Sendingtime, col_SenderNumber, col_RecieverNumber, col_Sendingstatus, Col_UsrId, Col_StatusCode, Col_Id };
                DataColumn[] columns = {
                                       col_MagfaId, col_smsText, col_SendingDate, col_Sendingtime, col_SenderNumber,
                                       col_RecieverNumber, col_Sendingstatus, Col_UsrId, Col_StatusCode
                                   };
                Retdt.Columns.AddRange(columns);
                string SendingDate = Utility.PersianNowDate;
                string SendingTime = DateTime.Now.ToShortTimeString().ToString();
                for (int i = 0; i < tmpMessages.Count; i++)
                {
                    long test;
                    bool canParse = long.TryParse(Utility.SetPrefixNum(tmpMobiles[i]), out test);
                    if (canParse)
                    {
                        object[] NewRow = {
                                              results[i], tmpMessages[i], SendingDate,
                                              DateTime.Now.ToShortTimeString().ToString(),
                                              tmpOrigs[i], tmpMobiles[i], "", UserId, (int) SentSmsType.SuccessefulSent
                                          };
                        Retdt.Rows.Add(NewRow);
                    }
                    else
                    {
                        object[] NewRow = {
                                              results[i], tmpMessages[i], SendingDate,
                                              DateTime.Now.ToShortTimeString().ToString(),
                                              tmpOrigs[i], "0", "", UserId, (int) SentSmsType.SuccessefulSent
                                          };
                        Retdt.Rows.Add(NewRow);
                    }
                }
                BulkInsertSentSms(Retdt);
                return results;
            }
            else
                return new long[1] { (long)111 };
        }

        public List<UserInfo> Authenticate(string username, string password)
        {
            SMSModelContainer model = new SMSModelContainer();
            var info = (from i in model.UserInfoes
                        where i.IUsrId == username && i.IUsrPaswrd == password
                        select i);
            var inf = info.ToList();
            return inf;
        }

        //public List<Tbl_RecieveSms> RecieveSMSByName(string startdate, string enddate, string Username)
        //{
        //    Utility ut = new Utility();
        //    DateTime sDate = Convert.ToDateTime(startdate);
        //    DateTime eDate = Convert.ToDateTime(enddate);
        //    List<Tbl_RecieveSms> rcvs = new List<Tbl_RecieveSms>();
        //    using(SMSModelContainer model = new SMSModelContainer())
        //    {
        //        var lines = (from i in model.Tbl_LineNumber
        //                 where i.IUsrId.ToLower() == Username.ToLower()
        //                 select i).Select(x => x.ILineNumber).ToList();

        //        foreach (var line in lines)
        //        {
        //            rcvs.AddRange((from i in model.GetRecieveSMS(line, Utility.PersianDateString(sDate), Utility.PersianDateString(eDate), "00:00", "99:99",  string.Empty)
        //                           select i).ToList());
        //        }                
        //    }

        //    return rcvs;
        //}

        public List<Tbl_RecieveSms> RecieveSMS(string startdate, string enddate, string PhNo)
        {
            Utility ut = new Utility();
            DateTime sDate = Convert.ToDateTime(startdate);
            DateTime eDate = Convert.ToDateTime(enddate);
            List<Tbl_RecieveSms> rcvs = new List<Tbl_RecieveSms>();
            using (SMSModelContainer model = new SMSModelContainer())
            {
                rcvs =
                    (from i in
                         model.GetRecieveSMS(PhNo, Utility.PersianDateString(sDate), Utility.PersianDateString(eDate),
                                             "00:00", "99:99", string.Empty)
                     select i).ToList();
            }

            return rcvs;
        }

        public List<Tbl_RecieveSms> RecieveUnreadSMS(string startdate, string enddate, string PhNo)
        {
            Utility ut = new Utility();
            DateTime sDate = Convert.ToDateTime(startdate);
            DateTime eDate = Convert.ToDateTime(enddate);
            List<Tbl_RecieveSms> rcvs = new List<Tbl_RecieveSms>();
            using (SMSModelContainer model = new SMSModelContainer())
            {
                rcvs =
                    (from i in
                         model.SpTbl_RecieveUnreadedSms_Select(PhNo, Utility.PersianDateString(sDate), Utility.PersianDateString(eDate),
                                             "00:00", "99:99", "", string.Empty)
                     select i).ToList();
            }

            return rcvs;
        }
        public List<Tbl_RecieveSms> RecieveUnreadSMS(string PhNo)
        {
            Utility ut = new Utility();
            List<Tbl_RecieveSms> rcvs = new List<Tbl_RecieveSms>();
            using (SMSModelContainer model = new SMSModelContainer())
            {
                rcvs =
                    (from i in
                         model.SpTbl_RecieveUnreadedSms_Select(PhNo, "0000/00/00", "9999/99/99",
                                             "00:00", "99:99", "", string.Empty)
                     select i).ToList();
            }

            return rcvs;
        }
        public List<Tbl_RecieveSms> GetUnreadMessgeseWithUsername(string UserId)
        {
            Utility ut = new Utility();
            List<Tbl_RecieveSms> rcvs = new List<Tbl_RecieveSms>();
            using (SMSModelContainer model = new SMSModelContainer())
            {
                rcvs =
                    (from i in
                         model.SpTbl_RecieveUnreadedSms_Select("", "0000/00/00", "9999/99/99",
                                             "00:00", "99:99", UserId, string.Empty)
                     select i).ToList();
            }

            return rcvs;
        }

        public int[] GetMessageStauses(long[] messageid)
        {
            MagfaService.SoapSmsQueuableImplementationService srv = new SoapSmsQueuableImplementationService();
            srv.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["MagfaUsrName"], ConfigurationManager.AppSettings["MagfaPassword"]);
            return srv.getRealMessageStatuses(messageid);
        }

        public int CheckCredit(int noFaSMS, int noEnSMS, string userId)
        {
            int i = 0, smsCredit = 0, totalprice = 0;
            using (SMSModelContainer db = new SMSModelContainer())
            {
                List<UsrCredit> credits = new List<UsrCredit>();
                credits = (from c in db.UsrCredits.Include("Tbl_Payment")
                           where c.IUsrId == userId && c.ICreditAmount > 0 && c.Tbl_Payment.PayFlag.HasValue && c.Tbl_Payment.PayFlag.Value
                           orderby c.PaymentId
                           select c).ToList();
                while ((noFaSMS > 0 || noEnSMS > 0) && i < credits.Count())
                {
                    if (noEnSMS > 0)
                    {
                        smsCredit = credits[i].ICreditAmount.GetValueOrDefault(0) /
                                    credits[i].ISMSEnPrice.GetValueOrDefault(0);
                        if (smsCredit <= noEnSMS)
                            totalprice += credits[i].ICreditAmount.GetValueOrDefault(0);
                        else
                        {
                            totalprice += credits[i].ISMSEnPrice.GetValueOrDefault(0) * noEnSMS;
                        }
                        noEnSMS -= smsCredit;
                    }
                    if (noFaSMS > 0)
                    {
                        smsCredit = credits[i].ICreditAmount.GetValueOrDefault(0) / credits[i].ISMSFaPrice.GetValueOrDefault(0);
                        if (smsCredit <= noFaSMS)
                            totalprice += credits[i].ICreditAmount.GetValueOrDefault(0);
                        else
                        {
                            totalprice += credits[i].ISMSFaPrice.GetValueOrDefault(0) * noFaSMS;
                        }
                        noFaSMS -= smsCredit;
                    }
                    i++;
                }

            }
            if (noEnSMS > 0 || noFaSMS > 0)
                return -1;
            return totalprice;
        }

        public string CheckVersion(string userID)
        {
            string str = string.Empty;
            using (SMSModelContainer db = new SMSModelContainer())
            {
                UserInfo uInfo = new UserInfo();
                uInfo = db.UserInfoes.Where(x => x.IUsrId == userID).FirstOrDefault();
                str = (from i in db.Tbl_UserAccountRole
                       where i.AcsRoleCode == uInfo.AscRoleCode
                       select i.AcsRoleEnName).FirstOrDefault();
            }
            return str;
        }
        public static string GetRandomNumber(int lenght)
        {
            char[] chars = "0123456789".ToCharArray();
            Random rnd = new Random(DateTime.Now.Millisecond);
            string text = "";
            for (int i = 0; i < lenght; i++)
            {
                text += chars[rnd.Next(chars.Length)].ToString();
            }

            return text;
        }
        public string Registeration(string[] strParamsCustomer, int RoleAccounting, string ManagerID, string LineNumber, string[] strParamsUserCredit)
        {
            string Status = "";
            string UserPass = GetRandomNumber(6);
            try
            {
                #region Customer

                Customers customer = new Customers();

                TblCustomerService customerService = new TblCustomerService();
                bool exist = customerService.CheckCustomer(strParamsCustomer[6]);

                if (exist == true)
                {

                    Status = "این شماره موبایل تکراری میباشد";

                    return Status;
                }
                customer.ICPrsAdrs = strParamsCustomer[0];
                customer.ICZipeCode = strParamsCustomer[1];
                customer.ICPrsNumber = strParamsCustomer[2];
                customer.ICCoActivity = strParamsCustomer[3];
                customer.ICParentID = -1;
                customer.IActTypeCode = 2;
                customer.ICName = strParamsCustomer[4];
                customer.ICLName = strParamsCustomer[5];
                customer.ICCoType = true;
                customer.ICMobile = strParamsCustomer[6];
                customer.ICEmail = strParamsCustomer[7];
                customer.ICPrsNumberPic = strParamsCustomer[8];
                customer.ICMeliCartPic = strParamsCustomer[9];
                customerService.Save(customer);
                customerService.SaveChange();
                #endregion
                #region Accounting
                UserAccount tblUserAccount = new UserAccount();
                UserAccountService accountService = new UserAccountService();
                tblUserAccount.IUserParentID = -1;
                tblUserAccount.IUsrManagerId = (string.IsNullOrEmpty(ManagerID) ? "accspki" : ManagerID);
                tblUserAccount.IUsrCCode = customer.ICCode;
                //Role code from Tbl_UserAccountRole...must go to the web.config
                tblUserAccount.AscRoleCode = RoleAccounting;
                tblUserAccount.IUsrId = strParamsCustomer[6];
                tblUserAccount.IUsrPaswrd = UserPass;
                tblUserAccount.IUsrActive = false;
                tblUserAccount.IUsrSpesial = false;
                tblUserAccount.IUsrISAdmin = false;
                tblUserAccount.IUsrIName = "admin";
                tblUserAccount.IUsrAccCreatDate = Utility.PersianNowDate;
                tblUserAccount.IUsrAccExpireDate = Utility.PersianDateString(DateTime.Now.AddYears(1));
                accountService.Save(tblUserAccount);
                accountService.SaveChange();
                #endregion
                #region LineNumber
                LineNumberService lineNumberService = new LineNumberService();
                linenumber linenum = new linenumber();
                try
                {
                    using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\linenumber.txt"))
                    {

                        outfile.Write(LineNumber);

                    }

                    if (LineNumber.Contains(","))
                    {
                        string[] TempLine = LineNumber.Split(',');
                        for (int i = 0; i < TempLine.Length; i++)
                        {
                            if (TempLine[i] != "" && TempLine[i] != ",")
                            {
                                linenum.IUsrId = strParamsCustomer[6];
                                linenum.ILineNumber = TempLine[i];
                                linenum.ILineModuleNumber = "--";
                                linenum.ILineServiceType = "ارسال";
                                linenum.ILineKeyWord = "--";
                                linenum.ILineStatuse = "فعال";
                                linenum.ILineLastModify = SSB.Helpers.Utility.PersianDateString(DateTime.Now);
                                linenum.ILineDuration = "1";
                                lineNumberService.Save(linenum);
                                lineNumberService.SaveChange();
                            }
                        }
                    }
                    else
                    {
                        linenum.IUsrId = strParamsCustomer[6];
                        linenum.ILineNumber = LineNumber;
                        linenum.ILineModuleNumber = "--";
                        linenum.ILineServiceType = "ارسال";
                        linenum.ILineKeyWord = "--";
                        linenum.ILineStatuse = "فعال";
                        linenum.ILineLastModify = SSB.Helpers.Utility.PersianDateString(DateTime.Now);
                        linenum.ILineDuration = "1";
                        lineNumberService.Save(linenum);
                        lineNumberService.SaveChange();
                    }
                }
                catch (Exception)
                {

                    //throw;
                }
                #endregion
                #region UserCredit
                AcountingEntities acountingEntities = new AcountingEntities();
                UsrCreditService creditService = new UsrCreditService();
                PaymentService paymentService = new PaymentService();

                Tbl_Payment tblPayment = new Tbl_Payment();
                Tbl_UsrCredit tblUsrCredit = new Tbl_UsrCredit();
                tblPayment.IUsrId = strParamsCustomer[6];
                tblPayment.PayTypeCode = 1;
                tblPayment.PayCommentCode = 0;
                tblPayment.PaymentAmount = 0; //int.Parse(strParamsUserCredit[0]);
                tblPayment.PaymentDiscount = 0;
                tblPayment.PayDisComment = "";
                tblPayment.PaySerial = "";
                tblPayment.PaymentDate = SSB.Helpers.Utility.PersianDateString(DateTime.Now);
                tblPayment.PayExpiredDate = "";
                tblPayment.PayFlag = true;
                int id = paymentService.Save(tblPayment);
                //var PayId =
                //  acountingEntities.SpTbl_Payment_Insert(strParamsCustomer[6], 1,0,int.Parse(strParamsUserCredit[0]),0,"","",SSB.Helpers.Utility.PersianDateString(DateTime.Now),"",true);
                //var t = PayId.Select(x => x.Value);

                tblUsrCredit.PaymentId = id;
                tblUsrCredit.IUsrId = strParamsCustomer[6];
                tblUsrCredit.ICreditAmount = 0; //int.Parse(strParamsUserCredit[0]);
                tblUsrCredit.ISMSFaPrice = int.Parse(strParamsUserCredit[1]);
                tblUsrCredit.ISMSEnPrice = int.Parse(strParamsUserCredit[2]);
                tblUsrCredit.IRcvSmsPrice = 0;
                creditService.Save(tblUsrCredit);
                creditService.SaveChange();
                #endregion
                #region Extensible
                tbl_Extensible te = new tbl_Extensible();
                te.IUsrId = strParamsCustomer[6];
                te.ServerURL = ConfigurationManager.AppSettings.Get("NewServerUrl").ToString();
                te.UsageType = "Organizational";
                te.UserQuantity = 1;
                te.TotalSupply = 0;
                te.CurrentSupply = 0;
                te.SupportExpert = "";
                te.SalesExpert = "";
                te.FixFaPrice = 0;
                te.FixEnPrice = 0;
                te.RecieveServerURL = ConfigurationManager.AppSettings.Get("NewRecieveUrl").ToString(); ;
                te.HasBankAccount = false;
                new ExtensibleService().Save(te);
                new ExtensibleService().SaveChange();
                #endregion
                Status = UserPass;
            }
            catch (Exception ex)
            {
                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\RegisterInsert.txt"))
                {
                    if (ex.InnerException != null)
                        outfile.Write(ex.Message + "::" + ex.StackTrace + Environment.NewLine + "innerException :" + ex.InnerException);
                    else
                    {
                        outfile.Write(ex.Message + "::" + ex.StackTrace);
                    }
                }
                Status = "not";
            }
            return Status;
        }

        public string[] QuickInsertCutomer(string linenumber, int credit, string version, string moduleNumber, int FaPrice, int EnPrice
            )
        {
            SMSModelContainer ctx = new SMSModelContainer();
            //string uName = Helper.Generate(5, 5);
            //string pass = Helper.Generate(6, 6);
            string uName = Helper.RandomString(5).ToLower();
            string pass = Helper.RandomNumber(6);
            //using (TransactionScope transaction = new TransactionScope())
            //{
            UserAccount userAccount = new UserAccount();
            UserAccountService accService = new UserAccountService();
            while (accService.CheckHasUser(uName))
            {
                uName = Helper.RandomString(5);

            }
            try
            {
                bool IsUserUnique = true;
                //while (IsUserUnique)
                //{
                //    IsUserUnique = this.CheckIsUserUnique(uName);
                //    if (!IsUserUnique)
                //        uName = Helper.Generate(5, 5).ToLower();
                //}

                //Cutomer
                Tbl_Customers customer = new Tbl_Customers();
                customer.ICParentID = -1;
                customer.IActTypeCode = 2;
                customer.ICName = "";
                customer.ICLName = linenumber;
                customer.ICCoType = true;
                ctx.Tbl_Customers.AddObject(customer);
                ctx.SaveChanges();

                //UserAccount
                userAccount.IUserParentID = -1;
                userAccount.IUsrManagerId = "ssb240";
                userAccount.IUsrCCode = customer.ICCode;
                //Role code from Tbl_UserAccountRole...must go to the web.config
                userAccount.AscRoleCode = 50;
                userAccount.IUsrId = uName;
                userAccount.IUsrPaswrd = pass;
                userAccount.IUsrActive = false;
                userAccount.IUsrSpesial = false;
                userAccount.IUsrISAdmin = false;
                userAccount.IUsrIName = "admin";
                userAccount.IUsrAccCreatDate = Utility.PersianNowDate;
                userAccount.IUsrAccExpireDate = Utility.PersianDateString(DateTime.Now.AddYears(1));


                // hp.InsertUserAcc(-1,"ssb240",customer.ICCode,50,uName,pass,
                //ctx.Tbl_UserAccount.AddObject(userAccount);
                //ctx.SaveChanges();

                accService.Save(userAccount);
                accService.SaveChange();
                //Add Credit
                ctx.SpTbl_OnlineUsrCredit_Insert(uName, (int)SSB.Helpers.PayType.Primary, credit,
                                                 Utility.PersianNowDate, true, FaPrice, EnPrice, 0, DateTime.Now, null, null, string.Empty, "شارژ اولیه کاربر");
                //Insert line number
                linenumber line = new linenumber();
                LineNumberService sLine = new LineNumberService();
                line.IUsrId = uName;
                line.ILineNumber = linenumber;
                line.ILineModuleNumber = moduleNumber;
                line.ILineServiceType = "دریافت";
                line.ILineStatuse = "فعال";
                line.ILineLastModify = Utility.PersianNowDate;
                line.ILineDuration = "1";
                sLine.Save(line);
                sLine.SaveChange();

                //ctx.Tbl_LineNumber.AddObject(line);
                //ctx.SaveChanges();
                //transaction.Complete();

            }
            catch (Exception)
            {
                return new string[] { string.Empty };

            }
            // }
            return new string[] { uName, pass };
        }

        private bool CheckIsUserUnique(string username)
        {
            bool res = false;
            using (SMSModelContainer ctx = new SMSModelContainer())
            {
                res = ctx.Tbl_UserAccount.Any(u => u.IUsrId == username);
            }
            return res;
        }

        public bool CheckIsLinenumberOwner(string[] linenumbers, string username)
        {
            bool res = false;

            using (LineNumberService lService = new LineNumberService())
            {
                res = lService.GetByCond(x => x.IUsrId == username && linenumbers.Contains(x.ILineNumber)).Any();
            }
            //using (SMSModelContainer ctx = new SMSModelContainer())
            //{
            //    res =
            //        linenumbers.ToList().Any(
            //            x =>
            //            ctx.Tbl_LineNumber.Where(m => m.IUsrId == username).ToList().Select(l => l.ILineNumber).ToList()
            //                .Contains(x));
            //}
            return res;
        }

        public void BulkInsertSentSms(DataTable dt)
        {
            //dt = new DataTable();
            EntityConnection cn = new EntityConnection("name=SMSModelContainer");

            using (SqlConnection sqlConn = new SqlConnection(cn.StoreConnection.ConnectionString))
            {
                SqlBulkCopy Bulkinsert = new SqlBulkCopy(sqlConn, SqlBulkCopyOptions.KeepIdentity, null);
                if (sqlConn.State != ConnectionState.Open)
                {
                    sqlConn.Open();
                }
                ;
                Bulkinsert.ColumnMappings.Add("MagfaSmsId", "MagfaSmsId");
                Bulkinsert.ColumnMappings.Add("ISMSText", "ISMSText");
                Bulkinsert.ColumnMappings.Add("ISendingDate", "ISendingDate");
                Bulkinsert.ColumnMappings.Add("ISendingTime", "ISendingTime");
                Bulkinsert.ColumnMappings.Add("ISenderNumber", "ISenderNumber");
                Bulkinsert.ColumnMappings.Add("IReciverNumber", "IReciverNumber");
                Bulkinsert.ColumnMappings.Add("ISMSStatuse", "ISMSStatuse");
                Bulkinsert.ColumnMappings.Add("ISMSStatusCode", "ISMSStatusCode");
                Bulkinsert.ColumnMappings.Add("IUsrId", "IUsrId");
                //Bulkinsert.ColumnMappings.Add("Id", "Id");
                Bulkinsert.DestinationTableName = "dbo.Tbl_SentSMS";
                Bulkinsert.WriteToServer(dt);
                sqlConn.Close();
            }
        }

        public List<Tbl_LineNumber> GetLineNumbers(string Username)
        {
            List<Tbl_LineNumber> list = null;
            using (SMSModelContainer ctx = new SMSModelContainer())
            {
                list = ctx.Tbl_LineNumber.Where(x => x.IUsrId == Username).ToList();
            }
            return list;
        }

        public bool GetUserAccountWithUserId(string UserID)
        {
            try
            {
                using (AcountingEntities dc = new AcountingEntities())
                {
                    var q = dc.UserAccounts.Where(s => s.IUsrId == UserID).Count();
                    if (q > 0)
                        return true;
                    else
                        return false;

                }
            }
            catch (Exception ex)
            {
                return false;
            }
            //throw new NotImplementedException();
        }

        public int SetPriority(int count, string userId)
        {
            int priority = 0;
            if (count > 100)
            {
                int smsCount = GetSmsCountByUserId(userId);
                smsCount += count;
                if (smsCount < 10)
                    priority = 1;
                if (smsCount > 500)
                    priority = 0;
                if (smsCount > 4000)
                    priority = -1;
                if (smsCount > 7000)
                    priority = -2;
                return priority;
            }
            else
            {
                if (count < 10)
                    priority = 1;
                if (count > 500)
                    priority = 0;
                if (count > 4000)
                    priority = -1;
                if (count > 7000)
                    priority = -2;
                return priority;
            }
        }

        private int GetSmsCountByUserId(string userId)
        {
            try
            {
                using (SMSModelContainer dc = new SMSModelContainer())
                {
                    return
                        dc.View_Tbl_SentSMS.Count(
                            x =>
                            x.MagfaSmsId == -1 && x.IUsrId == userId &&
                            x.ISMSStatusCode == (int)Helpers.SentSmsType.InSendQueue);
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        public DataTable GetListFreeNumber()
        {
            List<ReservedLineNumbers> lst = new List<ReservedLineNumbers>();
            ReservLineNumberService RService = new ReservLineNumberService();
            DataTable dtNum = new DataTable("ListNumber");
            try
            {
                lst = RService.GetByCond(s => s.State == (int)EnumLineState.Release).ToList();
                dtNum.Columns.Add("Line");
                dtNum.Columns.Add("Price");
                dtNum.Columns.Add("State");
                dtNum.Columns.Add("Id");
                for (int i = 0; i < lst.Count; i++)
                {
                    DataRow row = dtNum.NewRow();
                    row["Line"] = lst[i].LineNumber.ToString();
                    row["Price"] = lst[i].Price.ToString();
                    row["State"] = lst[i].State.ToString();
                    row["Id"] = lst[i].Id.ToString();
                    dtNum.Rows.Add(row);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return dtNum;
        }
        public List<VASServiceInfo> GetVasServices()
        {
            List<VASServiceInfo> lstServices = new List<VASServiceInfo>();
            try
            {
                using (AcountingEntities dc = new AcountingEntities())
                {
                    var q = dc.VASService.ToList();
                    foreach (var item in q)
                    {
                        VASServiceInfo vsi = new VASServiceInfo();
                        vsi.Id = item.Id;
                        vsi.Price = item.Price;
                        vsi.Published = (item.IsPublished.HasValue ? item.IsPublished.Value : false);
                        vsi.Title = item.Title;
                        lstServices.Add(vsi);
                    }
                }
                return lstServices;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<Tbl_RecieveSms> RecieveSMSById(int Id, string phNo)
        {
            List<Tbl_RecieveSms> rcvs = new List<Tbl_RecieveSms>();
            using (SMSModelContainer model = new SMSModelContainer())
            {
                rcvs =
                    (from i in
                         model.SpTbl_RecieveSms_SelectFromId(phNo, Id)
                     select i).ToList();
            }

            return rcvs;
        }
        #region Ticketing

        public int InsertTicket(string title, string body, int messageto, string messagefrom, byte status, DateTime date, int parent, byte[] attachFile, string fileName, int panelId, string managerId, string messageTo)
        {
            try
            {
                string mngId = "";
                string SSB_ManagerId = ConfigurationManager.AppSettings.Get("SSB_ManagerId").ToString();
                if (managerId != messagefrom)
                    mngId = managerId;
                else
                {
                    using (AcountingEntities dc = new AcountingEntities())
                    {
                        var q = dc.UserAccounts.Where(s => s.IUsrId == managerId).FirstOrDefault();
                        if (q != null)
                            mngId = q.IUsrManagerId;
                        if (q.IUsrId.ToLower() == SSB_ManagerId.ToLower())
                            mngId = q.IUsrId;
                    }
                }

                Ticket tc = new Ticket();
                tc.Body = body;
                tc.CreateDate = date;
                tc.MessageFrom = messagefrom;
                tc.TicketStatus = status;
                tc.ParentId = parent;
                tc.ResponsibleRole = messageto;
                tc.Subject = title;
                tc.AttachFile = attachFile;
                tc.FileName = fileName;
                tc.PanelId = panelId;
                tc.ManagerId = mngId;
                tc.MessageTo = messageTo;
                new TicketService().Save(tc);
                return tc.TicketId;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public List<Ticket> GetTickets(string messageFrom, int panelId, byte TicketType)
        {
            List<Ticket> tc = new List<Ticket>();
            using (AcountingEntities dc = new AcountingEntities())
            {
                switch (TicketType)
                {
                    case 0:
                        tc = dc.Ticket.Where(p => p.MessageTo == messageFrom && p.PanelId == panelId /*&& p.ParentId == 0*/).OrderByDescending(p => p.TicketId).ToList();
                        break;
                    case 1:
                        tc = dc.Ticket.Where(p => p.MessageFrom == messageFrom && p.PanelId == panelId /*&& p.ParentId == 0*/).OrderByDescending(p => p.TicketId).ToList();
                        break;
                    case 2:
                        tc = dc.Ticket.Where(p => (p.MessageTo == messageFrom || p.MessageFrom == messageFrom) && p.PanelId == panelId && p.ParentId == 0).OrderByDescending(p => p.TicketId).ToList();
                        break;
                }
                return tc;
            }
        }

        public List<Ticket> GetTicketsByParentId(int parentId)
        {
            using (AcountingEntities dc = new AcountingEntities())
            {
                return dc.Ticket.Where(p => p.ParentId == parentId && p.ParentId != 0).ToList();
            }
        }

        public Ticket GetTicket(int ticketId)
        {
            using (AcountingEntities dc = new AcountingEntities())
            {
                return dc.Ticket.Where(p => p.TicketId == ticketId).FirstOrDefault();
            }
        }

        public void UpdateTicketStatus(int ticketId, byte status)
        {
            using (AcountingEntities dc = new AcountingEntities())
            {
                Ticket ticket = dc.Ticket.Where(p => p.TicketId == ticketId).FirstOrDefault();
                if (ticket != null)
                {
                    ticket.TicketStatus = status;
                    dc.SaveChanges();
                    //new TicketService().Update(ticket);
                }
            }
        }

        public int GetMainTicketCount(int ticketId)
        {
            using (AcountingEntities dc = new AcountingEntities())
            {
                return dc.Ticket.Where(p => p.ParentId == ticketId).Count() + 1;
            }
        }

        public int GetSumFaSmsCount(string userId)
        {
            int sum = 0;
            using (AcountingEntities dc = new AcountingEntities())
            {
                List<Tbl_UsrCredit> lstUserCredit =
                    (from p in dc.Tbl_UsrCredit where p.IUsrId == userId select p).ToList();
                foreach (var usrCredit in lstUserCredit)
                {
                    if (usrCredit.ICreditAmount.HasValue)
                        sum += (usrCredit.ICreditAmount.Value / usrCredit.ISMSFaPrice.Value);
                }
            }
            return sum;
        }
        public int GetSumEnSmsCount(string userId)
        {
            int sum = 0;
            using (AcountingEntities dc = new AcountingEntities())
            {
                List<Tbl_UsrCredit> lstUserCredit =
                    (from p in dc.Tbl_UsrCredit where p.IUsrId == userId select p).ToList();
                foreach (var usrCredit in lstUserCredit)
                {
                    if (usrCredit.ICreditAmount.HasValue)
                        sum += (usrCredit.ICreditAmount.Value / usrCredit.ISMSEnPrice.Value);
                }
            }
            return sum;
        }

        #endregion
    }
}
