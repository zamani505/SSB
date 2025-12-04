using System;
using System.IO;
using SSB.Service.Core.com.sahandsms;

namespace SSB.Service.Core
{
    public class ParsianEnquiry
    {
        public bool IsValidTransaction(int Amount,int OrderId,string InvoiceNumber,string TransactionNumber,string UserId)
        {
            PaymentWebService paymentWeb = new PaymentWebService();

            bool t = paymentWeb.IsPaid(InvoiceNumber, OrderId.ToString(), Amount.ToString(), TransactionNumber,UserId);
            //using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\payment.txt", true))
            //{
            //    outfile.Write("Is valid:" + t.ToString() + ",tr:" + TransactionNumber+",inv"+(string.IsNullOrWhiteSpace(InvoiceNumber.Trim())?"NULL":InvoiceNumber) + Environment.NewLine);
            //}
            return t;
        }
    }
}