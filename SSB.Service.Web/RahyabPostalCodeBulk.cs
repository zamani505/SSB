using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SSB.Service.Core;
using System.IO;
using SSB.Helpers;
namespace SSB.Service.Web
{
    public class RahyabPostalCodeBulk
    {
        private string UserName = System.Configuration.ConfigurationManager.AppSettings["UserNameRahyab"];
        private string Passwrod = System.Configuration.ConfigurationManager.AppSettings["PasswordRahyab"];
        private string Company = System.Configuration.ConfigurationManager.AppSettings["CompanyRahyab"];
        RahyabService.RahyabSPCSendService Rahyab_RPC = new RahyabService.RahyabSPCSendService();
        public int PostalSend(string Number, string Message, byte SentType, int CityId, int AreaCode, int RegionCode, int From, int To, int Count, ref string ErrorMessage)
        {
            int Result = 0;
            try
            {
                Result = Rahyab_RPC.MCI_PostalSend(Number, UserName, Passwrod, Company, Message, SentType, CityId, AreaCode, RegionCode, From, To, Count, ref ErrorMessage);

            }
            catch (Exception ex)
            {
                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\RahyabPostalCodeBulk_PostalSend_Error.txt"))
                {
                    outfile.Write(DateTime.Now.ToString() + " Ex:" + ex.Message + Environment.NewLine + "::" + ex.StackTrace + Environment.NewLine +
                        "############################" + Environment.NewLine);
                }
                ///throw;
            }
            return Result;
        }
        public int GetCityPostalCode(string City, ref string ErrorMessage)
        {
            int Result = 0;
            try
            {
                Result = Rahyab_RPC.GetPostalCityCode(City, ref ErrorMessage);

            }
            catch (Exception ex)
            {
                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\GetCityPostalCode_PostalSend_Error.txt"))
                {
                    outfile.Write(DateTime.Now.ToString() + " Ex:" + ex.Message + Environment.NewLine + "::" + ex.StackTrace + Environment.NewLine +
                        "############################" + Environment.NewLine);
                }
                ///throw;
            }
            return Result;

        }
        public int GetGlobalProvinceCode(string Province, ref string ErrorMessage)
        {
            int Result = 0;
            try
            {
                Result = Rahyab_RPC.GetGlobalProvinceCode(Province, ref ErrorMessage);

            }
            catch (Exception ex)
            {
                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\GetCityPostalCode_PostalSend_Error.txt"))
                {
                    outfile.Write(DateTime.Now.ToString() + " Ex:" + ex.Message + Environment.NewLine + "::" + ex.StackTrace + Environment.NewLine +
                        "############################" + Environment.NewLine);
                }
                ///throw;
            }
            return Result;

        }
        public string GetStatus(string Number, int SendId, ref string ErrorMesage)
        {
            string Res = "";
            try
            {
                Res = Rahyab_RPC.GetStatus_MCISend(Number, UserName, Passwrod, Company, SendId, ref ErrorMesage);

            }
            catch (Exception ex)
            {
                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\GetStatus_PostalSend_Error.txt"))
                {
                    outfile.Write(DateTime.Now.ToString() + " Ex:" + ex.Message + Environment.NewLine + "::" + ex.StackTrace + Environment.NewLine +
                        "############################" + Environment.NewLine);
                }
                ///throw;
            }
            return Res;
        }

    }
}