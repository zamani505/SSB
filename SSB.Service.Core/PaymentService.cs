using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using SSB.Helpers;

namespace SSB.Service.Core
{
    public class PaymentService
    {
        public string[] IsValidUser(string Username)
        {
            SMSModelContainer db = new SMSModelContainer();
            var user = (from i in db.UserInfoes
                        where i.IUsrId == Username
                        select i).FirstOrDefault();
            if (user == null)
                return new string[0];
            else
                return new string[] { user.ICName, user.ICLName };
        }

        public int InsertPayment(int OrderId, int PayType, int Amount, DateTime date, string UserId, int BankId, string TransactionNumber, string InvoiceNumber, int faPrice, int enPrice,string desc)
        {
            try
            {
                string[] s = IsValidUser(UserId);
                if (s == null || s.Length < 2)
                    return -5;
               

                SMSModelContainer model = new SMSModelContainer();
                ParsianEnquiry pE = new ParsianEnquiry();
                MellatAcount ma = new MellatAcount();

                ma.OrderID = OrderId.ToString();
                ma.TransactionNumber = TransactionNumber;
                //Check transaction realy occured in Mellat Bank
                //if (BankId == 1 && !MellatInquiry.IsPaid(ma))
                //if (BankId == 1 && !pE.IsValidTransaction(Amount, OrderId, InvoiceNumber, TransactionNumber, UserId))
                //{
                //    //if (BankId == 1 && !pE.IsValidTransaction(Amount, OrderId, InvoiceNumber, TransactionNumber, UserId))
                //    //using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\mellat.txt", true))
                //    //{
                //    //    outfile.Write(ma.TransactionNumber+ Environment.NewLine);
                //    //}
                //    return -4;
                //}
                if (BankId == 2 && !pE.IsValidTransaction(Amount, OrderId, InvoiceNumber, TransactionNumber, UserId))
                    return -4;
                //if (!pE.IsValidTransaction(Amount, OrderId, InvoiceNumber, TransactionNumber, UserId))
                //    return -4;
               
                if (!model.Tbl_Payment.Any(x => x.TransactionNumber == TransactionNumber && x.BankId == BankId))
                {
                   
                    model.SpTbl_OnlineUsrCredit_Insert(UserId, PayType, Amount, Utility.PersianDateString(date),
                                                       true, faPrice,
                                                       enPrice,
                                                       0, date, TransactionNumber, BankId, InvoiceNumber,desc);

                    //using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\payment.txt", true))
                    //{
                    //    outfile.Write(DateTime.Now.ToString() + ",Inserted,tr:" + TransactionNumber + ",userid:" + UserId + Environment.NewLine);
                    //}


                }
                else
                    return -3;
            }
            catch (Exception ex)
            {
                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\payment.txt", true))
                {
                    outfile.Write(ex.Message+"::"+ex.StackTrace + Environment.NewLine);
                }
                throw;
                return -1;

                //throw;
            }
            return 1;
        }
        public int InsertPayment_ForAdinCharj(int OrderId, int PayType, int Amount, DateTime date, string UserId, int BankId, string TransactionNumber, 
            string InvoiceNumber, int faPrice, int enPrice, string desc)
        {
            try
            {
                string[] s = IsValidUser(UserId);
                if (s == null || s.Length < 2)
                    return -5;


                AcountingEntities model = new AcountingEntities();
                SMSModelContainer PanleModel = new SMSModelContainer();
                ParsianEnquiry pE = new ParsianEnquiry();
                MellatAcount ma = new MellatAcount();

                ma.OrderID = OrderId.ToString();
                ma.TransactionNumber = TransactionNumber;
                //Check transaction realy occured in Mellat Bank
                //if (BankId == 1 && !MellatInquiry.IsPaid(ma))
                //if (BankId == 1 && !pE.IsValidTransaction(Amount, OrderId, InvoiceNumber, TransactionNumber, UserId))
                //{
                //    //if (BankId == 1 && !pE.IsValidTransaction(Amount, OrderId, InvoiceNumber, TransactionNumber, UserId))
                //    //using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\mellat.txt", true))
                //    //{
                //    //    outfile.Write(ma.TransactionNumber+ Environment.NewLine);
                //    //}
                //    return -4;
                //}
                if (BankId == 2 && !pE.IsValidTransaction(Amount, OrderId, InvoiceNumber, TransactionNumber, UserId))
                    return -4;
                //if (!pE.IsValidTransaction(Amount, OrderId, InvoiceNumber, TransactionNumber, UserId))
                //    return -4;
                bool IsFrist = true;
                if (!PanleModel.Tbl_Payment.Any(x => x.TransactionNumber == TransactionNumber && x.BankId == BankId))
                {

                    model.SpTbl_OnlineUsrCredit_Insert_ForAdminCharj(UserId, PayType, Amount, Utility.PersianDateString(date),
                                                       true, faPrice,
                                                       enPrice,
                                                       0, date, TransactionNumber, BankId, InvoiceNumber, desc,IsFrist);

                    //using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\payment.txt", true))
                    //{
                    //    outfile.Write(DateTime.Now.ToString() + ",Inserted,tr:" + TransactionNumber + ",userid:" + UserId + Environment.NewLine);
                    //}


                }
                else
                    return -3;
            }
            catch (Exception ex)
            {
                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\InsertPayment_ForAdinCharj.txt", true))
                {
                    outfile.Write(DateTime.Now+"   Ex: "+ ex.Message + Environment.NewLine+
                        "Stacktrace : " + ex.StackTrace + Environment.NewLine);
                }
                throw;
                return -1;

                //throw;
            }
            return 1;
        }

        public int[] GetSmsPrice(int Amount, string userId)
        {
            int[] result = new int[2];
            try
            {

            long pr = 0;
            if (userId != null)
            {

                SMSModelContainer dbo = new SMSModelContainer();
                var extensible = (from p in dbo.tbl_Extensible where p.IUsrId == userId select p);
                foreach (var item in extensible)
                {
                    if (item.FixFaPrice != null && item.FixEnPrice != null)
                    {
                        result[0] = item.FixFaPrice.Value;
                        result[1] = item.FixEnPrice.Value;
                    }
                    break;
                }
                if (result[0] == 0)
                {
                    BulkSendingDefinitionService bdfs = new BulkSendingDefinitionService();
                    List<Tbl_BulkSendingDefinition> Listbdfs = new List<Tbl_BulkSendingDefinition>();
                    Listbdfs = bdfs.GetAllByUser(userId);
                   
                    for (int i = 0; i < Listbdfs.Count; i++)
                    {
                        pr = int.Parse(Listbdfs[i].IBulkMaxQuantity.ToString()) * int.Parse(Listbdfs[i].IBulkSMSFaPrice.ToString());

                        if (Amount <= pr)
                        {
                            result[0] = int.Parse(Listbdfs[i].IBulkSMSFaPrice.ToString());
                            result[1] = int.Parse(Listbdfs[i].IBulkSMSEnPrice.ToString());
                            return result;
                        }
                    }
                    if (result[0] == 0) {
                        
                        Listbdfs = bdfs.GetAll();
                        for (int i = 0; i < Listbdfs.Count; i++)
                        {
                            pr = int.Parse(Listbdfs[i].IBulkMaxQuantity.ToString()) * int.Parse(Listbdfs[i].IBulkSMSFaPrice.ToString());
                            if (Amount <= pr)
                            {
                                result[0] = int.Parse(Listbdfs[i].IBulkSMSFaPrice.ToString());
                                result[1] = int.Parse(Listbdfs[i].IBulkSMSEnPrice.ToString());
                                return result;
                            }
                        }
                    
                    }
                }
            }
            else
            {

                BulkSendingDefinitionService bdfs = new BulkSendingDefinitionService();
                List<Tbl_BulkSendingDefinition> Listbdfs = new List<Tbl_BulkSendingDefinition>();
                Listbdfs = bdfs.GetAll();
                for (int i = 0; i < Listbdfs.Count; i++)
                {
                    pr = int.Parse(Listbdfs[i].IBulkMaxQuantity.ToString()) * int.Parse(Listbdfs[i].IBulkSMSFaPrice.ToString());
                    if (Amount <= pr)
                    {
                        result[0] = int.Parse(Listbdfs[i].IBulkSMSFaPrice.ToString());
                        result[1] = int.Parse(Listbdfs[i].IBulkSMSEnPrice.ToString());
                        return result;
                    }
                }
            }
            }
            catch (Exception ex)
            {
                if (ex is System.Reflection.ReflectionTypeLoadException)
                {
                    var typeLoadException = ex as ReflectionTypeLoadException;
                    var loaderExceptions = typeLoadException.LoaderExceptions;
                    using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\payment2.txt", true))
                    {
                        outfile.Write("Get Sms Price:" + loaderExceptions[0].Message + Environment.NewLine);
                    }

                }

                throw;
            }

            return result;
        }

        public bool IsPaid(string Invoice,  int Amount,string TransactionNumber,string UserId)
        {
            bool res = false;
            
            using (SMSModelContainer ctx = new SMSModelContainer())
            {
                var t =
                    ctx.Tbl_Payment.Where(
                        x => x.PaymentAmount == Amount && x.TransactionNumber == TransactionNumber && x.IUsrId == UserId);
                if (!string.IsNullOrEmpty(Invoice))
                    t = t.Where(x => x.InvoiceNumber == Invoice);
                var obj = t.FirstOrDefault();
                if (obj != null && obj.PayFlag.HasValue && obj.PayFlag.Value)
                    res = true;
            }
            return res;
        }

        public int GetPaymentID(string UserId)
        {
            int PayID = 0;

            using (SMSModelContainer ctx = new SMSModelContainer())
            {
                PayID =(int)(from p in ctx.Tbl_Payment where p.IUsrId.Equals(UserId) select p.PaymentCode).Take(1).FirstOrDefault();
                    //ctx.Tbl_Payment.Where(
                    //    x => x.IUsrId == UserId );

               
            }
            return PayID;
        }

        public int Save(Tbl_Payment payment)
        {
            using (var db = new SMSModelContainer())
            {
                db.Tbl_Payment.AddObject(payment);
                db.SaveChanges();
            }
            return payment.PaymentCode;

        }

    }
}
