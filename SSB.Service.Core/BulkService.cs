using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Infrastructure;
using SSB.Service.Core.Model;
using SSB.Service.Core.Service;
using System.IO;

namespace SSB.Service.Core
{
    public class BulkService
    {

        public int SaveBulk(SerializeBulk bulk, SerializeBulk_Request[] BulkRequests)
        {
            try
            {
                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\webservice.txt", true))
                {
                    outfile.Write("Enter SaveBulk step1 ...bulk.LineNumber:" + bulk.LineNumber + Environment.NewLine +
                        "############################" + Environment.NewLine);
                }
                bulk.LineNumberCode = this.GetLineNumberCodeByLineNumber(bulk.LineNumber);
                Bulk bu = new Bulk() //Master
                {
                    //AdminNote = bulk.AdminNote,
                    BulkText = bulk.BulkText,
                    IsPersian = bulk.Ispersian,//me
                    //ModifiedDate=bulk.ModifiedDate,
                    //ModifyBy =bulk.ModifyBy,

                    RequestDate = DateTime.Parse(bulk.RequestDate), // me,
                    SendDate = DateTime.Parse(bulk.SendDate), //me

                    SendingType = bulk.SendingType, //me
                    Status = bulk.Status,
                    TotalPrice = bulk.TotalPrice,
                    UserId = bulk.UserId,
                    Notation = bulk.Notation,
                    LineNumberCode = bulk.LineNumberCode,
                    ReturnCredit = false
                };
                List<Bulk_Request> New_lstBulk_Request = new List<Bulk_Request>();
                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\webservice.txt", true))
                {
                    outfile.Write("Enter SaveBulk step1 ...bu.BulkText:" + bu.BulkText + Environment.NewLine +
                        "############################" + Environment.NewLine);
                }
                //Details
                for (int i = 0; i < BulkRequests.Length; i++)
                {
                    Bulk_Request br = new Bulk_Request()
                    {
                        //BulkRequestId =s.BulkrequestId,
                        BulkValue = BulkRequests[i].BulkValue,
                        CategoryId = BulkRequests[i].CategoryId,
                        PhoneType = BulkRequests[i].PhoneType,
                        RequestCount = BulkRequests[i].RequestCount,
                        StartIndex = BulkRequests[i].StartIndex,
                        ValueOperation = BulkRequests[i].ValueOperation,
                        ValueType = BulkRequests[i].ValueType
                    };
                    New_lstBulk_Request.Add(br);
                }

                int BulkId = 0;
                using (SMSModelContainer dc = new SMSModelContainer())
                {
                    New_lstBulk_Request.ForEach(s =>
                    {
                        bu.Bulk_Request.Add(s);
                    });
                    dc.Bulks.AddObject(bu);

                    dc.SaveChanges();

                }
                using (SMSModelContainer dc = new SMSModelContainer())
                {
                    BulkId = (from s in dc.Bulks orderby s.BulkId descending select s).FirstOrDefault().BulkId;
                }

                return BulkId;
            }
            catch (Exception ex)
            {
                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\bulkErr.txt", true))
                {
                    outfile.Write("from BulkService::" + DateTime.Now.ToString() + "::" + ex.Message + "::" + ex.StackTrace + Environment.NewLine);
                }

                return -1;
            }
        }

        public int GetLineNumberCodeByLineNumber(string LineNumber)
        {

            int LineNumberCode = 0;

            using (SMSModelContainer dc = new SMSModelContainer())
            {
                LineNumberCode = dc.Tbl_LineNumber
                    .Where(k => k.ILineNumber.Trim() == LineNumber.Trim())
                    .Select(s => s.ILineNumberCode).FirstOrDefault();
            }

            return LineNumberCode;
        }

        public SSB.Service.Core.Model.SerializeBulkState[] GetBulkState(int[] lstBulkId)
        {

            using (SMSModelContainer dc = new SMSModelContainer())
            {
                SerializeBulkState[] lstBulkStatus = dc.Bulks.Where(s => lstBulkId.Contains(s.BulkId))
                    .Select(m =>
                        new SerializeBulkState()
                        {
                            BulkStatus = m.Status,
                            AdminNote = m.AdminNote,
                            BulkServerNumber = m.BulkId
                        }
                            ).ToList().ToArray();
                return lstBulkStatus;
            }
        }
        //Exception darsorate nabodan bulk samte Accounting ya Null bodan
        public byte GetBulkStatus(int bulkId)
        {
            byte BulkStatus = 0;
            try
            {
                Bulk blk = SelectBulk(bulkId);
                // if (blk != null)
                BulkStatus = (byte)blk.Status;
            }
            catch (Exception ex)
            {
                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\bulkErr.txt", true))
                {
                    outfile.Write("from BulkService::" + DateTime.Now.ToString() + "::" + ex.Message + "::" + ex.StackTrace + Environment.NewLine);
                }

                throw;
            }
            return BulkStatus;
        }
        public bool DeleteBulk(int bulkId)
        {
            try
            {
                Bulk blk = SelectBulk(bulkId);
                string textBulk = blk.BulkText.ToString();
                string useridBulk = blk.UserId.ToString();
                //if (blk.Status == (int)SSB.Helpers.BulkRequestState.Sent)
                //    return false;
                int LineCode = int.Parse(blk.LineNumberCode.ToString());
                string LineNumber = null;
                int BUlkID = blk.BulkId;
                byte BulkStatus = (byte)blk.Status;
                List<Bulk_Request> list = new List<Bulk_Request>();
                list = SelectBulkRequest(BUlkID);

                bool checkNotSend = false;
                string Notation = "";
                using (SMSModelContainer dc = new SMSModelContainer())
                {
                    var bulkRequestList = dc.Bulk_Request.Where(i => i.BulkId.Value == bulkId);
                    var bulk = dc.Bulks.Where(i => i.BulkId == bulkId && (i.Status == 4 || i.Status == 1));
                    LineNumber = dc.Tbl_LineNumber.Where(p => p.ILineNumberCode == LineCode).Select(p => p.ILineNumber).FirstOrDefault();

                    if (bulk != null)
                        if (bulk.Count() > 0)
                        {

                            foreach (var bulkItem in bulk)
                            {
                                checkNotSend = true;
                                dc.Bulks.DeleteObject(bulkItem);
                                Notation = string.IsNullOrEmpty(bulkItem.Notation) ? "" : bulkItem.Notation;
                            }
                            if (checkNotSend == false) return false;
                            foreach (var item in bulkRequestList)
                            {
                                dc.Bulk_Request.DeleteObject(item);
                            }
                            dc.SaveChanges();
                        }
                        else return false;

                }
                SMSService ss = new SMSService();



                double rate = ss.GetTarrifrateRate(LineNumber);

                int count = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    count += int.Parse(list[i].RequestCount.ToString());
                }
                if (Notation != "") count++;
                int faSmsCount = 0;
                int enSmsCount = 0;

                if (SSB.Helpers.Utility.IsFarsiSMS(textBulk))
                {
                    faSmsCount = count *
                         SSB.Helpers.Utility.GetSmsPartCount(textBulk.Length - textBulk.Count(x => x == '\n'),
                                                             SSB.Helpers.Utility.
                                                                 FarsiSMSLength, LineNumber);
                }
                else
                {
                    enSmsCount = count *
                            SSB.Helpers.Utility.GetSmsPartCount(textBulk.Length - textBulk.Count(x => x == '\n'),
                                                                SSB.Helpers.Utility.
                                                                    EnglishSMSLength, LineNumber);
                }
                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\DeleteBulkInCore.txt",true))
                {
                    outfile.Write( "############################" + Environment.NewLine+"faSmsCount :" + faSmsCount.ToString() + " enSmsCount:" + enSmsCount.ToString()+" bulkid:"+bulkId.ToString() + Environment.NewLine 
                       );
                }
                //=============================Add credit Recurcive
                UsrCreditService UsrService = new UsrCreditService();
                UserAccountService AccService = new UserAccountService();
                PaymentService Payservice = new PaymentService();
                CreditLogService LogService = new CreditLogService();
                //------------------------
                Tbl_UsrCredit usrCredit = UsrService.GetCreditForDeleteBulk(useridBulk);
                bool checkHasAdmin = true;
                if (usrCredit == null) checkHasAdmin = false;

                while (checkHasAdmin)
                {


                    int CreditIncrese = ss.CheckCredit(faSmsCount, enSmsCount, usrCredit.IUsrId.ToString());
                    using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\DeleteBulkInCore.txt", true))
                    {
                        outfile.Write("usrCredit.IUsrId :" + usrCredit.IUsrId.ToString() + " bulkid:" + bulkId.ToString() + " CreditIncrese:"+
                            CreditIncrese.ToString() + Environment.NewLine
                           );
                    }
                    usrCredit.ICreditAmount += int.Parse((double.Parse(CreditIncrese.ToString()) * rate).ToString());
                    UsrService.Update(usrCredit);
                    UsrService.SaveChange();

                    AddCrditLog(int.Parse((double.Parse(CreditIncrese.ToString()) * rate).ToString()), usrCredit.ISMSFaPrice.GetValueOrDefault(0), usrCredit.ISMSEnPrice.GetValueOrDefault(0),
                         usrCredit.PaymentId, enSmsCount, faSmsCount, usrCredit.IUsrId.ToString());// Add CrditLog

                    UserAccount tblAcc = AccService.GetByCond(p => p.IUsrId.Equals(usrCredit.IUsrId)).FirstOrDefault();
                    tblAcc = AccService.GetByCond(p => p.IUsrId.Equals(tblAcc.IUsrManagerId)).FirstOrDefault();//
                    string tempUsrId = usrCredit.IUsrId.ToString();//for select payment from subUser
                    if (tblAcc != null)
                    {
                        usrCredit = UsrService.GetCredit(tblAcc.IUsrId);

                        if (usrCredit == null)
                        {

                            int? PayID = Payservice.GetPaymentID(tempUsrId);
                            Tbl_UsrCredit TblUsrCredit = new Tbl_UsrCredit();
                            CreditLog TblCrditLog = LogService.GetByCond(p => p.UserName == tblAcc.IUsrId).OrderByDescending(p => p.Id).Take(1).FirstOrDefault();
                            TblUsrCredit.PaymentId = (int)PayID;
                            TblUsrCredit.IUsrId = tblAcc.IUsrId;
                            TblUsrCredit.ICreditAmount = CreditIncrese;
                            TblUsrCredit.ISMSEnPrice = TblCrditLog.FaPrice;
                            TblUsrCredit.ISMSFaPrice = TblCrditLog.FaPrice;
                            TblUsrCredit.IRcvSmsPrice = 0;
                            UsrService.Save(TblUsrCredit);
                            UsrService.SaveChange();

                            AddCrditLog(CreditIncrese, TblCrditLog.FaPrice, TblCrditLog.FaPrice,
                         (int)PayID, enSmsCount, faSmsCount, tblAcc.IUsrId.ToString());// Add CrditLog
                            usrCredit = UsrService.GetCredit(tblAcc.IUsrManagerId);
                            if (usrCredit == null) checkHasAdmin = false;

                        }
                        else
                            usrCredit = UsrService.GetCredit(tblAcc.IUsrId);


                    }
                    else checkHasAdmin = false;


                }

                return true;

            }
            catch (Exception ex)
            {
                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\DeleteBulk.txt", true))
                {
                    outfile.Write("bulkId : " + bulkId.ToString() + "  from BulkService::" + DateTime.Now.ToString() + "::" + ex.Message + "::" + ex.StackTrace + Environment.NewLine);
                }

                return false;
            }
        }

        public bool AddCrditLog(int CreditIncrese, int ISMSFaPrice, int ISMSEnPrice, int PaymentId,
            int enSmsCount, int faSmsCount, string IUsrId)
        {
            SMSService ss = new SMSService();
            try
            {
                using (CreditLogService clService = new CreditLogService())
                {
                    CreditLog cl = new CreditLog();
                    cl.IsIncrease = true;
                    cl.Amount = CreditIncrese;
                    cl.UserName = IUsrId;
                    cl.Action = "Bulk Delete";
                    cl.Date = DateTime.Now;
                    cl.FaPrice = ISMSFaPrice;
                    cl.EnPrice = ISMSEnPrice;
                    cl.PaymentId = PaymentId;
                    cl.EnSmsCount = enSmsCount;
                    cl.FaSmsCount = faSmsCount;
                    cl.Source = "Dll";
                    cl.TotalCredit = ss.GetCurrentCredit(SSB.Helpers.Utility.PersianNowDate, IUsrId);
                    cl.TransactionType = (int)SSB.Helpers.TransactionType.RetriveCredit;
                    cl.Description = "برگشت شارژ بابت حذف ارسال انبوه";
                    clService.Save(cl);
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\bulkErr.txt", true))
                {
                    outfile.Write("from BulkService::" + DateTime.Now.ToString() + "::" + ex.Message + "::" + ex.StackTrace + Environment.NewLine);
                }

                return false;
            }
            return true;

        }
        public bool UpdateBulk(SerializeBulk bulk, SerializeBulk_Request[] BulkRequests)
        {


            SMSService ss = new SMSService();
            UsrCreditService UsrService = new UsrCreditService();
            Tbl_UsrCredit usrCredit = UsrService.GetCredit(bulk.UserId.ToString());
            int LineCode = int.Parse(bulk.LineNumberCode.ToString());
            string LineNumber = null;

            SMSModelContainer dc = new SMSModelContainer();

            //double rate = ss.GetTarrifrateRate(bulk.LineNumber);
            //if (usrCredit != null)
            //{


            //    List<Bulk_Request> list = new List<Bulk_Request>();
            //    Bulk bulks = SelectBulk(bulk.BulkId.Value);

            //    list = SelectBulkRequest(bulks.BulkId);

            //    int count = 0;
            //    for (int i = 0; i < list.Count; i++)
            //    {
            //        count += int.Parse(list[i].RequestCount.ToString());
            //    }
            //    int faSmsCount = 0;
            //    int enSmsCount = 0;

            //    if (SSB.Helpers.Utility.IsFarsiSMS(bulk.BulkText))
            //    {
            //        faSmsCount = count *
            //             SSB.Helpers.Utility.GetSmsPartCount(bulk.BulkText.Length - bulk.BulkText.Count(x => x == '\n'),
            //                                                 SSB.Helpers.Utility.
            //                                                     FarsiSMSLength, bulk.LineNumber);
            //    }
            //    else
            //    {
            //        enSmsCount = count *
            //                SSB.Helpers.Utility.GetSmsPartCount(bulk.BulkText.Length - bulk.BulkText.Count(x => x == '\n'),
            //                                                    SSB.Helpers.Utility.
            //                                                        EnglishSMSLength, bulk.LineNumber);
            //    }
            //    int CreditIncrese =int.Parse( (ss.CheckCredit(faSmsCount, enSmsCount, bulk.UserId)*rate).ToString());
            //    usrCredit.ICreditAmount += CreditIncrese;
            //    UsrService.Update(usrCredit);
            //    UsrService.SaveChange();
            //}


            BulkAccService SBulkAcc = new BulkAccService();
            BulkAcc TBLBulkAcc = SBulkAcc.GetBulks(bulk.BulkId.Value);
            if (TBLBulkAcc.Status != (byte)SSB.Helpers.BulkRequestState.Accepted)
                return false;
            if (DeleteBulkRequest(bulk.BulkId.Value))
            {

                TBLBulkAcc.AdminNote = bulk.AdminNote;
                TBLBulkAcc.BulkText = bulk.BulkText;
                TBLBulkAcc.IsPersian = bulk.Ispersian;//me
                TBLBulkAcc.RequestDate = DateTime.Parse(bulk.RequestDate); // me,
                TBLBulkAcc.SendDate = DateTime.Parse(bulk.SendDate); //me

                TBLBulkAcc.SendingType = bulk.SendingType; //me
                TBLBulkAcc.Status = bulk.Status;
                TBLBulkAcc.TotalPrice = bulk.TotalPrice;
                TBLBulkAcc.UserId = bulk.UserId;
                SBulkAcc.Update(TBLBulkAcc);
                SBulkAcc.SaveChange();
            }
            List<Bulk_RequestAcc> New_lstBulk_Request = new List<Bulk_RequestAcc>();
            BulkRequstAccService sBulkRequstAcc = new BulkRequstAccService();
            //Details
            //BulkRequests.ForEach(s =>
            //{


            //});
            foreach (var item in BulkRequests)
            {
                Bulk_RequestAcc br = new Bulk_RequestAcc()
                {
                    BulkValue = item.BulkValue,
                    CategoryId = item.CategoryId,
                    PhoneType = item.PhoneType,
                    RequestCount = item.RequestCount,
                    StartIndex = item.StartIndex,
                    ValueOperation = item.ValueOperation,
                    ValueType = item.ValueType,
                    BulkId = bulk.BulkId.Value
                };
                sBulkRequstAcc.Save(br);
            }
            sBulkRequstAcc.SaveChange();
            // bu.LineNumberCode = bs.GetLineNumberCodeByLineNumber(bulk.LineNumber);
            //StreamWriter sr = new StreamWriter(fn, true);
            //sr.Write("::l:" + bu.LineNumberCode+ "::u:" + bu.UserId+ Environment.NewLine);
            //sr.Close();
            //  return bs.UpdateBulk(bu, New_lstBulk_Request);


            //else
            return true;
        }
        public Bulk SelectBulk(int bulkid)
        {




            using (SMSModelContainer dc = new SMSModelContainer())
            {

                var list = dc.Bulks.Where(i => i.BulkId == bulkid).FirstOrDefault();

                return list;
            }



        }
        public List<Bulk_Request> SelectBulkRequest(int bulkId)
        {
            List<Bulk_Request> list = new List<Bulk_Request>();
            try
            {


                using (SMSModelContainer dc = new SMSModelContainer())
                {

                    list = dc.Bulk_Request.Where(i => i.BulkId.Value == bulkId).ToList();

                    return list;
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\bulkErr.txt", true))
                {
                    outfile.Write("from BulkService::" + DateTime.Now.ToString() + "::" + ex.Message + "::" + ex.StackTrace + Environment.NewLine);
                }

            }
            return list;

        }
        public bool DeleteBulkRequest(int bulkId)
        {
            try
            {
                using (SMSModelContainer dc = new SMSModelContainer())
                {
                    var list = dc.Bulk_Request.Where(i => i.BulkId.Value == bulkId);
                    foreach (var item in list)
                    {
                        dc.Bulk_Request.DeleteObject(item);
                    }

                    dc.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\bulkErr.txt", true))
                {
                    outfile.Write("from BulkService::" + DateTime.Now.ToString() + "::" + ex.Message + "::" + ex.StackTrace + Environment.NewLine);
                }
                return false;
            }
        }

    }
}