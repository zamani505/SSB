using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using SSB.Service.Core;

namespace SSB.Service.Web
{
    /// <summary>
    /// Summary description for PaymentWebservice
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class PaymentWebservice : System.Web.Services.WebService
    {

        [WebMethod]
        public string[] IsValidUser(string Username)
        {
            if (string.IsNullOrEmpty(Username))
                return new string[0];
            PaymentService pay = new PaymentService();
            return pay.IsValidUser(Username);
        }
        [WebMethod]
        public int[] GetSmsPrice(int Amount, string userId)
        {
            return new PaymentService().GetSmsPrice(Amount, userId);
        }

        [WebMethod]
        public int InsertPayment(int OrderId, int ChargeType, int Amount, DateTime PaymentDate, string UserId, int BankId, string TransactionNumber, string InvoiceNumber,string Desc)
        {
            //sr.Write("::orderId::" + OrderId + "::TransNum::" + TransactionNumber + "::" + DateTime.Now.ToString() + Environment.NewLine);
            //sr.Close();

            try
            {
                PaymentService pay = new PaymentService();
                int money = (Amount * 6) / 100;
                money = Amount - money;
                int[] price = new int[2];
                price = GetSmsPrice(money, UserId);
                //using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\InsertPayment_log.txt",true))
                //{
                //    outfile.Write("OrderId:" + OrderId.ToString() + " , ChargeType: " + ChargeType.ToString() + " , Amount : " + Amount.ToString() + " , PaymentDate : " + PaymentDate.ToString() + " , UserId: " + UserId.ToString() + " , BankId : " + BankId.ToString() + " , TransactionNumber : " +
                //        TransactionNumber.ToString() + " , InvoiceNumber: " + InvoiceNumber.ToString() + " , price[0] :" + price[0].ToString() + " , price[1]:" + price[1].ToString() + " , Desc:" + Desc.ToString()+Environment.NewLine+"######"+Environment.NewLine);
                //}
                //int res = pay.InsertPayment_ForAdinCharj(OrderId, ChargeType, Amount, PaymentDate, UserId, BankId, TransactionNumber, InvoiceNumber, price[0], price[1], Desc);
                int res = pay.InsertPayment(OrderId, ChargeType, Amount, PaymentDate, UserId, BankId, TransactionNumber, InvoiceNumber, price[0], price[1], Desc);
                //using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\InsertPayment_log.txt", true))
                //{
                //    outfile.Write("res:" + res + Environment.NewLine + "######" + Environment.NewLine);
                //}
                return res;

            }
            catch (Exception ex)
            {
                string fn = Server.MapPath("~/log.txt");
                StreamWriter sr = new StreamWriter(fn, true);
                sr.Write("::" + ex.Message + "::" + ex.StackTrace + " , Inner:"+ex.InnerException+Environment.NewLine);
                sr.Flush();
                sr.Close();

                throw;
            }
        }

        [WebMethod]
        public bool IsPaid(string InvoiceNumber, int Amount,string TransactionNumber,string UserId)
        {
            if (string.IsNullOrEmpty(TransactionNumber) || string.IsNullOrEmpty(UserId))
                throw new Exception("Parameters not valid");
            Utility.CreateLog("Invoice:"+InvoiceNumber+",Transaction:"+TransactionNumber+"Amount:"+Amount+Environment.NewLine);
            return new PaymentService().IsPaid(InvoiceNumber, Amount, TransactionNumber, UserId);
        }
    }
}
