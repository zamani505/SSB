using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using SSB.Service.Core.Service;
using SSB.Service.Core.Model;
using SSB.Helpers;

namespace SSB.Service.Core
{
    public class MainSmsService : IMainSmsService
    {
        private BulkService _bulkService = new BulkService();
        public int N;
        public int Totalcredit = 0;
        public int ErrorNumber = 0;
        private string ConnStr = ConfigurationManager.AppSettings["ConnectionString"].ToString();
        private string TmpConnStr = ConfigurationManager.AppSettings["TmpConnectionString"].ToString();

        // parametrs of magfa webservice method
        public long[] checkingIds;
        public int[] encodings;
        public int[] mclass;
        public string[] messages;
        public string[] mobiles;
        public string[] origs;
        public int[] priorities;
        //public long[] results;
        public string[] UDH;

        //read setting from web.config

        // db variables
        public DataSet ds = new DataSet();
        public DataTable dt = new DataTable();
        //public SqlDataReader dr;

        //************************** methods **********************
        public void RecieveSms(object[] Parametrs)
        {

            DataTable dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = sqlConn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SpTbl_ReciveSms_Insert";
                        cmd.Parameters.Clear();
                        string Today = Utility.PersianNowDate;
                        cmd.Parameters.AddWithValue("@RcvSmsMessageID", Parametrs[0]);
                        cmd.Parameters.AddWithValue("@RcvSmsText", Parametrs[1]);
                        cmd.Parameters.AddWithValue("@RcvSmsfrom", Parametrs[2]);
                        cmd.Parameters.AddWithValue("@RcvSmsTo", Parametrs[3]);
                        cmd.Parameters.AddWithValue("@RcvSmsInteredDate", Parametrs[4]);
                        cmd.Parameters.AddWithValue("@RcvSmsDeliveredTime", Parametrs[5]);
                        cmd.Parameters.AddWithValue("@RcvSmsRuleID", Parametrs[6]);
                        cmd.Parameters.AddWithValue("@RcvSmsStatus", Parametrs[7]);
                        cmd.Parameters.AddWithValue("@RcvSmsSmsC", Parametrs[8]);
                        cmd.Parameters.AddWithValue("@RcvsmsCharSet", Parametrs[9]);
                        cmd.Parameters.AddWithValue("@RcvsmsKeyWord", Parametrs[10]);
                        cmd.Parameters.AddWithValue("@RcvSmsUDH", Parametrs[11]);
                        cmd.Parameters.AddWithValue("@RcvSmsReeded", Parametrs[12]);
                        cmd.Parameters.AddWithValue("@OperatorId", Parametrs[13]);
                        sqlConn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                finally
                {

                    sqlConn.Close();
                }

            }
        }
        public DataTable Select_UsrCredit3(string UserID)
        {
            dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {
                    dt.TableName = "dt";
                    adptr.SelectCommand = new SqlCommand();
                    adptr.SelectCommand.Connection = sqlConn;
                    adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adptr.SelectCommand.CommandText = "SpTbl_UsrCredit_Select2";


                    adptr.SelectCommand.Parameters.AddWithValue("@IUsrId", UserID);
                    adptr.Fill(dt);
                }
            }

            return dt;
        }
        public string GetExpireDate(string UserID)
        {
            string expireDate = null;
            UserAccountService userService = new UserAccountService();
            UserAccount userAccount = userService.GetByCond(p => p.IUsrId.Equals(UserID.Trim())).FirstOrDefault();
            expireDate = userAccount.IUsrAccExpireDate.ToString();
            return expireDate;
        }
        public DataTable SelectVASService()
        {
            dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {
                    dt.TableName = "dt";
                    adptr.SelectCommand = new SqlCommand();
                    adptr.SelectCommand.Connection = sqlConn;
                    adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adptr.SelectCommand.CommandText = "SPTbl_VASServiceSelectAll";
                    adptr.Fill(dt);
                }
            }

            return dt;
        }
        public bool InsertSentSMS(string smsText, string SendingDate, string SendingTime, string UserId,
            string recieveNumber
            )
        {
            try
            {
                string query =
                    "INSERT INTO Tbl_SentSMS (ISMSText,ISendingDate,ISendingTime,IUsrId,ISenderNumber,IReciverNumber,ISMSStatuse,MagfaSmsId,ISMSStatusCode,OperatorId,Id" +
                    ")VALUES(@ISMSText,@ISendingDate,@ISendingTime, @UsrId,@ISenderNumber,@IReciverNumber,@ISMSStatuse,@MagfaSmsId,@ISMSStatusCode,@OperatorId,@Id " +
                    ")";
                using (SqlConnection sqlConn = new SqlConnection(ConnStr))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        try
                        {
                            cmd.Connection = sqlConn;
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = query;
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@ISMSText", smsText);
                            cmd.Parameters.AddWithValue("@ISendingDate", SendingDate);
                            cmd.Parameters.AddWithValue("@ISendingTime", SendingTime);
                            cmd.Parameters.AddWithValue("@UsrId", UserId);
                            cmd.Parameters.AddWithValue("@ISenderNumber", "201101");
                            cmd.Parameters.AddWithValue("@IReciverNumber", recieveNumber);
                            cmd.Parameters.AddWithValue("@ISMSStatuse", "");
                            cmd.Parameters.AddWithValue("@MagfaSmsId", -1);
                            cmd.Parameters.AddWithValue("@ISMSStatusCode", 17);
                            cmd.Parameters.AddWithValue("@OperatorId", 10);
                            cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());

                            if (sqlConn.State != ConnectionState.Open) { sqlConn.Open(); };
                            cmd.ExecuteNonQuery();
                        }
                        finally
                        {
                            sqlConn.Close();
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
                // throw;
            }
            return true;
        }

        public DataTable SelectCrditLogReport(string DateFrom, string DateTo, string userID, int type)
        {


            dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {
                    dt.TableName = "dt";
                    adptr.SelectCommand = new SqlCommand();
                    adptr.SelectCommand.Connection = sqlConn;
                    adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adptr.SelectCommand.CommandText = "SpTbl_CreditLogReport";
                    adptr.SelectCommand.Parameters.AddWithValue("@DateFrom", DateFrom);
                    adptr.SelectCommand.Parameters.AddWithValue("@DateTo", DateTo);
                    adptr.SelectCommand.Parameters.AddWithValue("@UserID", userID);
                    adptr.SelectCommand.Parameters.AddWithValue("@ReportType", type);

                    adptr.Fill(dt);
                }
            }

            return dt;
        }
        public DataTable Levels(string username)
        {
            dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {
                    dt = new DataTable();
                    dt.TableName = "dtPermision";
                    adptr.SelectCommand = new SqlCommand();
                    adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adptr.SelectCommand.CommandText = "[SPView_GetRoleItems]";

                    adptr.SelectCommand.Parameters.AddWithValue("@Username", username);
                    adptr.SelectCommand.Connection = sqlConn;
                    adptr.Fill(dt);
                }
            }
            return dt;
        }
        public string[] UpdateMsgStatus(long MsgId, SentSmsType statuscode)
        {
            string[] result = new string[2];
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        SqlDataReader dr;

                        cmd.Connection = sqlConn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SPTbl_SentSMS_UpdateMsgStatusCode";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@MagfaSmsId", MsgId);
                        cmd.Parameters.AddWithValue("@ISMSStatuse", (int)statuscode);
                        if (sqlConn.State != ConnectionState.Open) { sqlConn.Open(); };
                        dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            result[0] = dr["IUsrId"].ToString();
                            result[1] = dr["Id"].ToString();
                        }
                    }
                    finally
                    {
                        sqlConn.Close();
                    }
                }
            }
            return result;
        }
        public string[] UpdateMsgStatus_TmpSmsDelivery(long batchId, string mobileNumber, SentSmsType statuscode)
        {
            string[] result = new string[2];
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        SqlDataReader dr;

                        cmd.Connection = sqlConn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SPTbl_TmpSmsDelivery_UpdateMsgStatus";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@BatchId", batchId);
                        cmd.Parameters.AddWithValue("@MobileNumber", mobileNumber);
                        cmd.Parameters.AddWithValue("@Staus", (int)statuscode);
                        if (sqlConn.State != ConnectionState.Open) { sqlConn.Open(); };
                        dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            result[0] = dr["MessageId"].ToString();
                        }
                    }
                    finally
                    {
                        sqlConn.Close();
                    }
                }
            }
            return result;
        }


        public SSB.Service.Core.CreditLog[] GetCreditLog(string username, DateTime? fromTime, DateTime? toTime)
        {
            var _creditLog = new CreditLogService();
            if (!fromTime.HasValue && !toTime.HasValue)
                return _creditLog.GetByCond(x => x.UserName == username).ToArray();
            return _creditLog.GetByCond(x => x.UserName == username && (x.Date >= fromTime && x.Date <= toTime)).ToArray();
        }

        public DataTable GetUserInfo(string UsrId, string PassWord)
        {
            dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {
                    //using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\step1mainservice.txt"))
                    //{
                    //    outfile.Write("step1mainservice");

                    // }
                    try
                    {
                        

                        adptr.SelectCommand = new SqlCommand();
                        adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                        adptr.SelectCommand.CommandText = "SpView_GetUsrInfo_SelectUsrInfo";
                        adptr.SelectCommand.Parameters.Clear();
                        adptr.SelectCommand.Parameters.AddWithValue("@UsrId", UsrId);
                        adptr.SelectCommand.Parameters.AddWithValue("@PassWord", PassWord);
                        string TodayDate = Utility.PersianNowDate;
                        adptr.SelectCommand.Parameters.AddWithValue("@Today", TodayDate);
                        adptr.SelectCommand.Connection = sqlConn;
                        dt.TableName = "dt";
                        adptr.Fill(dt);
                    }
                    catch (Exception ex)
                    {
                        using (
                            StreamWriter outfile =
                                new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\errorLogGetuserinfo.txt",
                                                 true))
                        {
                            outfile.Write(ex.Message + ":::::::::::" + ex.StackTrace);

                        }
                        //throw;
                    }

                }
                return dt;
            }
        }

        public void Insert_TmpAtiehReciveStatus( string line, int   Count)
        {
              using (SqlConnection sqlconn = new SqlConnection(ConnStr))
                {
                    SqlCommand sqlcom = new SqlCommand("SpTmpAtiehReciveStatus_Insert", sqlconn);
                    sqlcom.CommandType = CommandType.StoredProcedure;
                    sqlcom.Parameters.AddWithValue("@Linenumber", line);
                    sqlcom.Parameters.AddWithValue("@Count", Count);
                    if (sqlconn.State != ConnectionState.Open)
                    {
                        sqlconn.Open();
                        sqlcom.ExecuteNonQuery();
                    }
                }
        }


        public int GetCredit(string userId)
        {
            SMSService _service = new SMSService();
            return _service.GetCurrentCredit(Utility.PersianNowDate, userId);
        }
        public DataTable GetAllCredit(string UsrId)
        {
            dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {
                    adptr.SelectCommand = new SqlCommand();
                    adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adptr.SelectCommand.CommandText = "SPView_GetUsrCredit_Select";
                    string Today = Utility.PersianNowDate;
                    adptr.SelectCommand.Parameters.AddWithValue("@Today", Today);
                    adptr.SelectCommand.Parameters.AddWithValue("@UsrID", UsrId);
                    adptr.SelectCommand.Connection = sqlConn;
                    dt.TableName = "dt";
                    adptr.Fill(dt);
                }
            }
            return dt;
        }
        public DataTable Select_UserAccountRole4(string UserID)
        {
            dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {
                    dt.TableName = "dt";
                    adptr.SelectCommand = new SqlCommand();
                    adptr.SelectCommand.Connection = sqlConn;
                    adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adptr.SelectCommand.CommandText = "[SPTbl_UserAccountRole_Select4]";


                    adptr.SelectCommand.Parameters.AddWithValue("@IUsrId", UserID);

                    adptr.Fill(dt);
                }
            }
            return dt;
        }
        public DataTable Get_PaymentComment()
        {
            dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {
                    dt.TableName = "dtPayCommand";
                    adptr.SelectCommand = new SqlCommand();
                    adptr.SelectCommand.Connection = sqlConn;
                    adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adptr.SelectCommand.CommandText = "SpTbl_PaymentComment_Select";
                    adptr.Fill(dt);
                }
            }
            return dt;
        }
        public void Update_Tbl_UserCredit(Utility.Tbl_UserCredit_TableType[] Messages)
        {
            try
            {
                var tGroup = Messages.GroupBy(p => p.UserId);
                foreach (var item in tGroup)
                {
                    DataTable DT = new DataTable("Tbl_UserCredit_TableType");
                    DT.Columns.Add("UserId", typeof(string));
                    DT.Columns.Add("SmsCount", typeof(int));
                    DT.Columns.Add("IsFarsi", typeof(int));

                    foreach (var mes in item)
                    {
                        DataRow DR = DT.NewRow();
                        DR["UserId"] = mes.UserId;
                        DR["SmsCount"] = mes.SmsCount;
                        DR["IsFarsi"] = mes.IsFarsi;
                        DT.Rows.Add(DR);
                    }

                    SqlParameter parameter = new SqlParameter();
                    parameter.ParameterName = "@Messages";
                    parameter.SqlDbType = System.Data.SqlDbType.Structured;
                    parameter.Value = DT;
                    SqlParameter dateParameter = new SqlParameter();
                    dateParameter.ParameterName = "@CurrentDatePersian";
                    dateParameter.SqlDbType = System.Data.SqlDbType.NVarChar;
                    dateParameter.Value = Utility.PersianDateString(DateTime.Now);
                    using (SqlConnection connection = new SqlConnection(ConnStr))
                    {
                        using (SqlCommand cmd = new SqlCommand("spUpdate_Tbl_UserCredit_IncreaseCredit", connection))
                        {

                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(parameter);
                            cmd.Parameters.Add(dateParameter);
                            connection.Open();
                            cmd.ExecuteNonQuery();
                            connection.Close();
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                //SSB.SendEngine.BulkDataAccess.Common.ErrorLogCreator.ErrorLog("Send", "Update_Tbl_UserCredit", ex.Message.ToString()
                //        , ErrorUnitName.Queue, true);
                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\Update_Tbl_UserCredit.txt"))
                {
                    outfile.Write(ex.Message + "::" + ex.StackTrace);
                }
            }
        }
        public int UpdateCreditRcvsms(int SmsCount, string UsrId)
        {
            int Credit = 0;
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        SqlDataReader dr;
                        cmd.Connection = sqlConn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SpTbl_Usrcredit_UpdateRcvsms";
                        cmd.Parameters.Clear();
                        string Today = Utility.PersianNowDate;
                        cmd.Parameters.AddWithValue("@Today", Today);
                        cmd.Parameters.AddWithValue("@UsrId", UsrId);
                        cmd.Parameters.AddWithValue("@RcvSmsCount", SmsCount);
                        if (sqlConn.State != ConnectionState.Open)
                        {
                            sqlConn.Open();
                        }
                        ;
                        dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {

                            Credit = (int)dr["UsrCredit"];
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorNumber = 4;
                        throw;
                    }
                    finally
                    {
                        sqlConn.Close();
                    }

                }
            }
            return Credit;
        }

        public int getinteger(int n, int m)
        {
            if (m == 160)
            {
                if (n > 160)
                {
                    //  n = n - 153;
                    m = 153;
                }
                else
                {
                    m = 160;
                }
            }
            else
            {
                if (n > 70)
                {
                    //  n = n - 67;
                    m = 67;
                }
                else
                {
                    m = 70;
                }
            }
            double r;
            int i;
            i = n / m;
            r = n % m;
            if (r != 0)
            {
                return i + 1;
            }
            else
            {
                if (i == 0) return 1;
                else return i;
            }
        }
        //public int[] Getmessagestatus(long[] arrmessageId)
        //{

        //    ArrayList arrmsgId = new ArrayList();
        //    dt = new DataTable();
        //    using (SqlConnection sqlConn = new SqlConnection(ConnStr))
        //    {
        //        using (SqlDataAdapter adptr = new SqlDataAdapter())
        //        {
        //            adptr.SelectCommand = new SqlCommand();
        //            adptr.SelectCommand.Connection = sqlConn;
        //            adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
        //            adptr.SelectCommand.CommandText = "SpTbl_SentSms_SelectMagfaId";
        //            dt.TableName = "dt";
        //            adptr.Fill(dt);
        //            for (int i = 0; i < arrmessageId.Length; i++)
        //            {
        //                var quary = from a in dt.AsEnumerable()
        //                            where (a.Field<int>("ChatchId") == arrmessageId[i])
        //                            select (a.Field<long>("MagfaSmsId"));
        //                arrmsgId.AddRange(quary.ToArray());
        //            }

        //            //this.test.Proxy = this.Proxyobj;
        //            //this.test.Credentials = new NetworkCredential(MagfaUsrName, MagfaPassword);
        //        }
        //    }
        //    long[] srtArray = (long[])arrmsgId.ToArray(typeof(long));
        //    return this.test.getRealMessageStatuses(srtArray);
        //}
        public DataTable GetRcvSms(string ToMobile, string DateFrom, string DateTo, String TimeFrom, String TimeTo, string UsrId)
        {
            dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {

                    adptr.SelectCommand = new SqlCommand();
                    adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adptr.SelectCommand.CommandText = "SpTbl_RecieveSms_Select";
                    string Today = Utility.PersianNowDate;
                    adptr.SelectCommand.Parameters.AddWithValue("@Today", Today);
                    adptr.SelectCommand.Parameters.AddWithValue("@UsrId", UsrId);
                    adptr.SelectCommand.Parameters.AddWithValue("@RcvSmsTo", ToMobile);
                    adptr.SelectCommand.Parameters.AddWithValue("@DateFrom", DateFrom);
                    adptr.SelectCommand.Parameters.AddWithValue("@DateTo", DateTo);
                    adptr.SelectCommand.Parameters.AddWithValue("@TimeFrom", TimeFrom);
                    adptr.SelectCommand.Parameters.AddWithValue("@TimeTo", TimeTo);
                    // int credit=0;
                    adptr.SelectCommand.Connection = sqlConn;
                    dt.TableName = "dt";
                    adptr.Fill(dt);
                }
            }
            return dt;
        }
        public void BulkInsertSentSms(DataTable dt)
        {
            //dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
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
                Bulkinsert.ColumnMappings.Add("Id", "Id");
                Bulkinsert.DestinationTableName = "dbo.Tbl_SentSMS";
                Bulkinsert.WriteToServer(dt);
                sqlConn.Close();
            }
        }
        public DataTable Select_ExtensibleSettig(string UserID)
        {
            dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {
                    dt.TableName = "dt";
                    adptr.SelectCommand = new SqlCommand();
                    adptr.SelectCommand.Connection = sqlConn;
                    adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adptr.SelectCommand.CommandText = "[SPTbl_Extensible_Select]";

                    adptr.SelectCommand.Parameters.AddWithValue("@IUsrId", UserID);
                    adptr.Fill(dt);
                }
            }

            return dt;
        }
        public DataTable Select_LineNumber2(string UserID)
        {
            dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {
                    dt.TableName = "dt";
                    adptr.SelectCommand = new SqlCommand();
                    adptr.SelectCommand.Connection = sqlConn;
                    adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adptr.SelectCommand.CommandText = "[SPTbl_LineNumber_Select3]";

                    adptr.SelectCommand.Parameters.AddWithValue("@IUsrId", UserID);
                    adptr.Fill(dt);
                }
            }
            return dt;
        }
        public DataTable Get_UserAccount(string ManagerId)
        {
            dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {
                    dt.TableName = "dtUsrAcc";
                    adptr.SelectCommand = new SqlCommand();
                    adptr.SelectCommand.Connection = sqlConn;
                    adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adptr.SelectCommand.CommandText = "SPTbl_UserAccount_Select2";

                    adptr.SelectCommand.Parameters.AddWithValue("@IUsrManagerId", ManagerId);
                    adptr.Fill(dt);
                }
            }
            return dt;
        }
        public DataTable Get_PaymentType()
        {
            dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {
                    dt.TableName = "dtpayType";
                    adptr.SelectCommand = new SqlCommand();
                    adptr.SelectCommand.Connection = sqlConn;
                    adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adptr.SelectCommand.CommandText = "SpTbl_PaymentType_Select";
                    adptr.Fill(dt);
                }
            }
            return dt;
        }
        public DataTable Select_CustomerPayment(string IUsrId)
        {
            dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {
                    dt.TableName = "dtPayment";
                    adptr.SelectCommand = new SqlCommand();
                    adptr.SelectCommand.Connection = sqlConn;
                    adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adptr.SelectCommand.CommandText = "SPTbl_Payment_Select";
                    adptr.SelectCommand.Parameters.Clear();

                    adptr.SelectCommand.Parameters.AddWithValue("@IUsrId", IUsrId);
                    adptr.Fill(dt);
                }
            }
            return dt;
        }
        public void Update_Customer
     (
         string IProviencyCode,
         string ICityCode,
         string IActTypeCode,
         string ICName,
         string ICLName,
         string ICTel,
         string ICMobile,
         string ICFax,
         string ICEmail,
         string ICWebSite,
         string ICPrsAdrs,
         string ICZipeCode,
         string ICCoName,
         bool ICCoType,
         string ICCoActivity,
         string ICDescrtiption,

         string IUsrId,
         string IUsrPaswrd
     )
        {
            dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {

                    adptr.SelectCommand = new SqlCommand();
                    adptr.SelectCommand.Connection = sqlConn;
                    adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adptr.SelectCommand.CommandText = "SPTbl_Customer_Update2";

                    adptr.SelectCommand.Parameters.AddWithValue("@IProviencyCode", IProviencyCode);
                    adptr.SelectCommand.Parameters.AddWithValue("@ICityCode", ICityCode);
                    adptr.SelectCommand.Parameters.AddWithValue("@IActTypeCode", IActTypeCode);
                    adptr.SelectCommand.Parameters.AddWithValue("@ICName", ICName);
                    adptr.SelectCommand.Parameters.AddWithValue("@ICLName", ICLName);
                    adptr.SelectCommand.Parameters.AddWithValue("@ICTel", ICTel);
                    adptr.SelectCommand.Parameters.AddWithValue("@ICMobile", ICMobile);
                    adptr.SelectCommand.Parameters.AddWithValue("@ICFax", ICFax);
                    adptr.SelectCommand.Parameters.AddWithValue("@ICEmail", ICEmail);
                    adptr.SelectCommand.Parameters.AddWithValue("@ICWebSite", ICWebSite);
                    adptr.SelectCommand.Parameters.AddWithValue("@ICPrsAdrs", ICPrsAdrs);
                    adptr.SelectCommand.Parameters.AddWithValue("@ICZipeCode", ICZipeCode);
                    adptr.SelectCommand.Parameters.AddWithValue("@ICCoName", ICCoName);
                    adptr.SelectCommand.Parameters.AddWithValue("@ICCoType", ICCoType);
                    adptr.SelectCommand.Parameters.AddWithValue("@ICCoActivity", ICCoActivity);
                    adptr.SelectCommand.Parameters.AddWithValue("@ICDescrtiption", ICDescrtiption);

                    adptr.SelectCommand.Parameters.AddWithValue("@IUsrId", IUsrId);
                    adptr.SelectCommand.Parameters.AddWithValue("@IUsrPaswrd", IUsrPaswrd);
                    adptr.Fill(dt);
                }
            }
        }

        public DataTable Select_Customer(string IUsrId)
        {
            dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {
                    dt.TableName = "dtselectcust";
                    adptr.SelectCommand = new SqlCommand();
                    adptr.SelectCommand.Connection = sqlConn;
                    adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adptr.SelectCommand.CommandText = "SPTbl_Customer_Select";
                    adptr.SelectCommand.Parameters.Clear();

                    adptr.SelectCommand.Parameters.AddWithValue("@IUsrId", IUsrId);
                    adptr.Fill(dt);
                }
            }

            return dt;
        }
        public DataTable Select_Customer3(string UserID)
        {
            dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {
                    dt.TableName = "dt";
                    adptr.SelectCommand = new SqlCommand();
                    adptr.SelectCommand.Connection = sqlConn;
                    adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adptr.SelectCommand.CommandText = "[SPTbl_Customer_Select3]";


                    adptr.SelectCommand.Parameters.AddWithValue("@UserId", UserID);
                    adptr.Fill(dt);

                }
            }
            return dt;
        }
        public DataTable Select_UserAccountRole5(string RoleEnName)
        {
            dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {
                    dt.TableName = "dt";
                    adptr.SelectCommand = new SqlCommand();
                    adptr.SelectCommand.Connection = sqlConn;
                    adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adptr.SelectCommand.CommandText = "[SPTbl_UserAccountRole_Select5]";


                    adptr.SelectCommand.Parameters.AddWithValue("@RoleEnName", RoleEnName);
                    adptr.Fill(dt);
                }
            }

            return dt;
        }

        public object[] Send(string[] messages, string[] mobiles, string[] origs, int[] encodings, string[] UDH, int[] mclass, int[] priorities
            , long[] checkingIds, string UsrId, Guid[] Ids, byte TransActionType)
        {
            // bool HasCredit = false;
            int FaSmsCount = 0;
            int EnSmsCount = 0;
            int CurrentCredit = 0;
            int UsedCredit = 0;
            SMSModelContainer model = new SMSModelContainer();
            DataTable Retdt = new DataTable();
            SMSService _service = new SMSService();

            Retdt = SSB.Helpers.Utility.GetBuklDtAccounting();
            try
            {

                //if (results != null)
                {
                    int lenght = mobiles.Length;

                    try
                    {
                        int operatorId = _service.GetOperatorId(origs[0]);
                        int priority = 0;
                        priority = _service.SetPriority(lenght, UsrId);
                        for (int i = 0; i < lenght; i++)
                        {
                            string SendingDate = Utility.PersianNowDate;
                            string SendingTime = DateTime.Now.ToShortTimeString().ToString();
                            object[] NewRow = Utility.NewBulkTableRowAccounting(-1, messages[i], SendingDate, SendingTime,
                                                                                origs[i], mobiles[i],
                                                                                SentSmsType.SuccessefulSent, UsrId, Ids[i],
                                                                                operatorId, priority);

                            Retdt.Rows.Add(NewRow);
                            int enterCount = messages[i].Count(f => f == '\n');
                            int length = messages[i].Length - enterCount;

                            if (Utility.IsFarsiSMS(messages[i]))
                                FaSmsCount += Utility.GetSmsPartCount(length, Utility.FarsiSMSLength, origs[i]);
                            else
                                EnSmsCount += Utility.GetSmsPartCount(length, Utility.EnglishSMSLength, origs[i]);
                        }
                        bool hasCredit = false;

                        string status = "";
                        try
                        {
                            status = System.Configuration.ConfigurationManager.AppSettings["IsRemot"].ToString();

                        }
                        catch (Exception)
                        {

                            status = "1";
                        }

                        hasCredit = _service.HasEnoughCredit(EnSmsCount, FaSmsCount, UsrId, origs[0], model);
                        if (hasCredit == false)
                        {
                            long[] res2 = new long[origs.Length];
                            for (int i = 0; i < origs.Length; i++)
                            {
                                res2[i] = 0;
                            }

                            object[] RetValues2 = { res2, Totalcredit, UsedCredit, _service.GetTarrifrateRate(origs[0]) };
                            return RetValues2;

                        }
                        CurrentCredit = GetTotalCredit(UsrId);
                        Totalcredit = UpdateCreditByRate(UsrId, FaSmsCount, EnSmsCount, _service.GetTarrifrateRate(origs[0]), TransActionType);

                        DataTable dttemp;
                        bool checkWhile = true;
                        int start = 1;
                        int end = 4000;
                        DataColumn dc = new DataColumn();
                        dc.ColumnName = "row";
                        dc.DataType = System.Type.GetType("System.Int64");
                        dc.DefaultValue = 1;
                        if (status.Equals("0"))
                        {
                            Retdt.Columns.Add(dc);
                            int irow = 1;
                            foreach (DataRow row in Retdt.Rows)
                            {

                                row["row"] = irow;
                                irow++;
                            }
                            #region DivWith4000
                            while (checkWhile)
                            {
                                DataView dv = null;

                                dv = new DataView(Retdt, "row>=" + start.ToString() + " and row<=" + end.ToString() + "", "row", DataViewRowState.Added);
                                dv.Sort = "row Asc";
                                dttemp = dv.ToTable();
                                dttemp.Columns.RemoveAt(12);
                                //Retdt.Columns.RemoveAt(12);


                                new Utility().BulkInsertSentSmsAccounting(dttemp, new SqlConnection(ConnStr));
                                if ((end + 4000) <= Retdt.Rows.Count)
                                {
                                    start += 4000;
                                    end += 4000;
                                }
                                else if (end < Retdt.Rows.Count)
                                {
                                    start += 4000;
                                    end += Retdt.Rows.Count - end;
                                }
                                else checkWhile = false;



                                //Retdt.Select()
                            }
                            #endregion
                        }
                        else
                        {
                            //hasCredit = _service.HasEnoughCredit(EnSmsCount, FaSmsCount, UsrId, origs[0], model);
                            //if (hasCredit == false)
                            //{
                            //    long[] res2 = new long[end];
                            //    for (int i = 0; i < end; i++)
                            //    {
                            //        res2[i] = 0;
                            //    }

                            //    object[] RetValues2 = { res2, Totalcredit, UsedCredit, _service.GetTarrifrateRate(origs[0]) };
                            //    return RetValues2;

                            //}
                            new Utility().BulkInsertSentSmsAccounting(Retdt, new SqlConnection(ConnStr));
                        }



                    }
                    catch (Exception ex)
                    {
                        ErrorNumber = 2; //عدم ذخیره پیامک ها}
                        using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\err.txt", true))
                        {
                            outfile.Write("from MainSmsService::" + DateTime.Now.ToString() + "::" + ex.Message + "::" + ex.StackTrace + Environment.NewLine);
                        }
                        throw;
                    }


                }
            }
            catch (Exception ex)
            {
                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\err.txt", true))
                {
                    outfile.Write("from MainSmsService::" + DateTime.Now.ToString() + "::" + ex.Message + "::" + ex.StackTrace + Environment.NewLine);
                }
                throw;

            }
            if (CurrentCredit != -1 & Totalcredit != -1) { UsedCredit = CurrentCredit - Totalcredit; }
            long[] res = new long[messages.Length];
            for (int i = 0; i < messages.Length; i++)
            {
                res[i] = -1;
            }

            object[] RetValues = { res, Totalcredit, UsedCredit, _service.GetTarrifrateRate(origs[0]) };
            return RetValues;
        }

        public object[] SendBulk(string[] messages, string[] mobiles, string[] origs, int[] encodings, string[] UDH, int[] mclass, int[] priorities, long[] checkingIds, string UsrId, Guid[] Ids)
        {
            // bool HasCredit = false;
            int FaSmsCount = 0;
            int EnSmsCount = 0;
            int CurrentCredit = 0;
            int UsedCredit = 0;
            SMSModelContainer model = new SMSModelContainer();
            DataTable Retdt = new DataTable();
            SMSService _service = new SMSService();

            Retdt = SSB.Helpers.Utility.GetBuklDtAccounting();
            try
            {

                //if (results != null)
                {
                    int lenght = mobiles.Length;

                    try
                    {
                        int operatorId = _service.GetOperatorId(origs[0]);
                        int priority = 0;
                        priority = _service.SetPriority(lenght, UsrId);
                        for (int i = 0; i < lenght; i++)
                        {
                            string SendingDate = Utility.PersianNowDate;
                            string SendingTime = DateTime.Now.ToShortTimeString().ToString();
                            object[] NewRow = Utility.NewBulkTableRowAccounting(-1, messages[i], SendingDate, SendingTime,
                                                                                origs[i], mobiles[i],
                                                                                SentSmsType.SuccessefulSent, UsrId, Ids[i],
                                                                                operatorId, priority);

                            Retdt.Rows.Add(NewRow);
                            int enterCount = messages[i].Count(f => f == '\n');
                            int length = messages[i].Length - enterCount;

                            if (Utility.IsFarsiSMS(messages[i]))
                                FaSmsCount += Utility.GetSmsPartCount(length, Utility.FarsiSMSLength, origs[i]);
                            else
                                EnSmsCount += Utility.GetSmsPartCount(length, Utility.EnglishSMSLength, origs[i]);
                        }
                        bool hasCredit = false;

                        string status = "";
                        try
                        {
                            status = System.Configuration.ConfigurationManager.AppSettings["IsRemot"].ToString();

                        }
                        catch (Exception)
                        {

                            status = "1";
                        }

                        hasCredit = _service.HasEnoughCredit(EnSmsCount, FaSmsCount, UsrId, origs[0], model);
                        if (hasCredit == false)
                        {
                            long[] res2 = new long[origs.Length];
                            for (int i = 0; i < origs.Length; i++)
                            {
                                res2[i] = 0;
                            }

                            object[] RetValues2 = { res2, Totalcredit, UsedCredit, _service.GetTarrifrateRate(origs[0]) };
                            return RetValues2;

                        }
                        CurrentCredit = GetTotalCredit(UsrId);
                        Totalcredit = UpdateCreditByRate(UsrId, FaSmsCount, EnSmsCount, _service.GetTarrifrateRate(origs[0]), (byte)SSB.Helpers.TransactionType.ManualSend);

                        DataTable dttemp;
                        bool checkWhile = true;
                        int start = 1;
                        int end = 4000;
                        DataColumn dc = new DataColumn();
                        dc.ColumnName = "row";
                        dc.DataType = System.Type.GetType("System.Int64");
                        dc.DefaultValue = 1;
                        if (status.Equals("0"))
                        {
                            Retdt.Columns.Add(dc);
                            int irow = 1;
                            foreach (DataRow row in Retdt.Rows)
                            {

                                row["row"] = irow;
                                irow++;
                            }
                            #region DivWith4000
                            while (checkWhile)
                            {
                                DataView dv = null;

                                dv = new DataView(Retdt, "row>=" + start.ToString() + " and row<=" + end.ToString() + "", "row", DataViewRowState.Added);
                                dv.Sort = "row Asc";
                                dttemp = dv.ToTable();
                                dttemp.Columns.RemoveAt(12);
                                //Retdt.Columns.RemoveAt(12);


                                new Utility().TempBulkInsertSentSmsAccounting(dttemp, new SqlConnection(ConnStr), new SqlConnection(TmpConnStr));
                                if ((end + 4000) <= Retdt.Rows.Count)
                                {
                                    start += 4000;
                                    end += 4000;
                                }
                                else if (end < Retdt.Rows.Count)
                                {
                                    start += 4000;
                                    end += Retdt.Rows.Count - end;
                                }
                                else checkWhile = false;



                                //Retdt.Select()
                            }
                            #endregion
                        }
                        else
                        {
                            //hasCredit = _service.HasEnoughCredit(EnSmsCount, FaSmsCount, UsrId, origs[0], model);
                            //if (hasCredit == false)
                            //{
                            //    long[] res2 = new long[end];
                            //    for (int i = 0; i < end; i++)
                            //    {
                            //        res2[i] = 0;
                            //    }

                            //    object[] RetValues2 = { res2, Totalcredit, UsedCredit, _service.GetTarrifrateRate(origs[0]) };
                            //    return RetValues2;

                            //}
                            new Utility().TempBulkInsertSentSmsAccounting(Retdt, new SqlConnection(ConnStr), new SqlConnection(TmpConnStr));
                        }



                    }
                    catch (Exception ex)
                    {
                        ErrorNumber = 2; //عدم ذخیره پیامک ها}
                        using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\err.txt", true))
                        {
                            outfile.Write("from MainSmsService::" + DateTime.Now.ToString() + "::" + ex.Message + "::" + ex.StackTrace + Environment.NewLine);
                        }
                        throw;
                    }


                }
            }
            catch (Exception ex)
            {
                using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\err.txt", true))
                {
                    outfile.Write("from MainSmsService::" + DateTime.Now.ToString() + "::" + ex.Message + "::" + ex.StackTrace + Environment.NewLine);
                }
                throw;

            }
            if (CurrentCredit != -1 & Totalcredit != -1) { UsedCredit = CurrentCredit - Totalcredit; }
            long[] res = new long[messages.Length];
            for (int i = 0; i < messages.Length; i++)
            {
                res[i] = -1;
            }

            object[] RetValues = { res, Totalcredit, UsedCredit, _service.GetTarrifrateRate(origs[0]) };
            return RetValues;
        }

        public bool HaveEnaughCredit(int FaSmsCount, int EnSmsCount, string UsrId, string lineNumber)
        {

            SMSService _service = new SMSService();
            return _service.HasEnoughCredit(EnSmsCount, FaSmsCount, UsrId, lineNumber);
        }
        public bool HaveEnaughCreditForVoiceMessage(int FaSmsCount, int SMSPrice, string UsrId)
        {

            SMSService _service = new SMSService();
            return _service.HasEnoughCreditForVoiceMessage(FaSmsCount,SMSPrice, UsrId);
        }
        public DataTable Report5(string userID)
        {
            dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {
                    dt.TableName = "dtrep5";
                    adptr.SelectCommand = new SqlCommand();
                    adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adptr.SelectCommand.CommandText = "[SpTbl_UserAccountInfo_Select]";

                    adptr.SelectCommand.Parameters.AddWithValue("@UserID", userID);
                    adptr.SelectCommand.Connection = sqlConn;
                    adptr.Fill(dt);
                }
            }
            return dt;
        }

        public DataTable ResetPassWord(string username, string pass)
        {
            dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {

                    adptr.SelectCommand = new SqlCommand();
                    adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adptr.SelectCommand.CommandText = "spResetPassword";

                    adptr.SelectCommand.Parameters.AddWithValue("@UserID", username);
                    adptr.SelectCommand.Parameters.AddWithValue("@pass", pass);

                    adptr.SelectCommand.Connection = sqlConn;
                    adptr.Fill(dt);
                }
            }
            return dt;
        }
        public DataTable Get_BulkSendingDefinition()
        {
            dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {
                    dt.TableName = "dtBulk";
                    adptr.SelectCommand = new SqlCommand();
                    adptr.SelectCommand.Connection = sqlConn;
                    adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adptr.SelectCommand.CommandText = "SPTbl_BulkSendingDefinition_Select";
                    adptr.Fill(dt);
                }
            }
            return dt;
        }


        public DataTable Select_CustomerCredit(string IUsrId)
        {
            dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {
                    dt.TableName = "dtCustCredit";

                    adptr.SelectCommand = new SqlCommand();
                    adptr.SelectCommand.Connection = sqlConn;
                    adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adptr.SelectCommand.CommandText = "SPView_GetUsrCredit_SelectAll3";
                    adptr.SelectCommand.Parameters.Clear();

                    adptr.SelectCommand.Parameters.AddWithValue("@IUsrID", IUsrId);
                    adptr.Fill(dt);
                }
            }
            return dt;
        }

        public DataTable IsValidAdmin(string UsrId)
        {
            dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {
                    dt.TableName = "dtAdminLnum";

                    adptr.SelectCommand = new SqlCommand();
                    adptr.SelectCommand.Connection = sqlConn;
                    adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adptr.SelectCommand.CommandText = "SpTblUsrAccount_IsAdminValid";
                    adptr.SelectCommand.Parameters.Clear();

                    adptr.SelectCommand.Parameters.AddWithValue("@UsrId", UsrId);
                    adptr.Fill(dt);
                }
            }

            return dt;
        }

        public DataTable Select_UserLineNumber(string UserID)
        {
            dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {
                    dt.TableName = "dt";
                    adptr.SelectCommand = new SqlCommand();
                    adptr.SelectCommand.Connection = sqlConn;
                    adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adptr.SelectCommand.CommandText = "[SPView_GetLineNumber_Select]";

                    adptr.SelectCommand.Parameters.AddWithValue("@UsrName", UserID);

                    adptr.Fill(dt);
                }
            }
            return dt;
        }

        //*********************************************

        public int GetTotalCredit(string UsrId)
        {
            int Credit = 0;
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        SqlDataReader dr;
                        cmd.Connection = sqlConn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SpTbl_UsrCredit_SelectTotalCredit";
                        cmd.Parameters.Clear();
                        string Today = Utility.PersianNowDate;
                        cmd.Parameters.AddWithValue("@Today", Today);
                        cmd.Parameters.AddWithValue("@UsrId", UsrId);
                        if (sqlConn.State != ConnectionState.Open) { sqlConn.Open(); };
                        dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            int.TryParse(dr["UsrCredit"].ToString(), out Credit);
                        }
                    }
                    catch (Exception ex)
                    {
                        Credit = -1;
                        ErrorNumber = 3;
                        throw;
                    }
                    finally
                    {
                        sqlConn.Close();
                    }

                }
            }
            return Credit;
        }
        public DataTable Report6(string userID)
        {
            dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {
                    dt.TableName = "dtrep6";
                    adptr.SelectCommand = new SqlCommand();
                    adptr.SelectCommand.Connection = sqlConn;
                    adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adptr.SelectCommand.CommandText = "[SPTbl_LineNumber_Select2]";

                    adptr.SelectCommand.Parameters.AddWithValue("@IUsrId", userID);
                    adptr.Fill(dt);
                }
            }
            return dt;
        }

        public int UpdateCredit(string UserID, string Pass, string Amount)
        {
            int Credit = 0;
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {

                        if (this.GetUserInfo(UserID, Pass).Rows.Count > 0)
                        {

                            SqlDataReader dr;
                            cmd.Connection = sqlConn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "SpTbl_Usrcredit_UpdateCredit2";
                            cmd.Parameters.Clear();
                            string Today = Utility.PersianNowDate;
                            int AM = int.Parse(Amount);
                            cmd.Parameters.AddWithValue("@UserID", UserID);
                            cmd.Parameters.AddWithValue("@Amount", AM);
                            if (sqlConn.State != ConnectionState.Open)
                            {
                                sqlConn.Open();
                            }
                            dr = cmd.ExecuteReader();
                            if (dr.Read())
                            {
                                int.TryParse(dr["UsrCredit"].ToString(), out Credit);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Credit = -1;
                        ErrorNumber = 3;// عدم به روز رسانی اعتبار
                        throw;
                    }
                    finally
                    {
                        sqlConn.Close();
                    }

                }
            }
            return Credit;
        }

        public double GetTarrifOperator(string linenumber)
        {
            SMSService _service = new SMSService();
            return _service.GetTarrifrateRate(linenumber);
        }

        public struct MyUserUrlStruct
        {
            public string RecieveServerURL;
            public string UserId;
        }
        public List<MyUserUrlStruct> GetServerUrlByLineNumber(string LineNumber)
        {
            List<MyUserUrlStruct> lstUserServerUrl = new List<MyUserUrlStruct>();
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {
                    try
                    {
                        SqlDataReader dr;
                        //SqlCommand com = new SqlCommand();
                        //    com.Connection = sqlConn;
                        //com.CommandText = "SpTbl_UsrAccount_SelectRecieveServerUrl";
                        //com.CommandType = CommandType.StoredProcedure;
                        //com.Parameters.AddWithValue("@LineNumber", LineNumber)
                        adptr.SelectCommand = new SqlCommand();
                        adptr.SelectCommand.Connection = sqlConn;
                        adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                        adptr.SelectCommand.CommandText = "SpTbl_UsrAccount_SelectRecieveServerUrl";

                        adptr.SelectCommand.Parameters.AddWithValue("@LineNumber", LineNumber);
                        if (sqlConn.State != ConnectionState.Open) { sqlConn.Open(); };

                        dr = adptr.SelectCommand.ExecuteReader();
                        while(dr.Read())
                        {
                            MyUserUrlStruct item = new MyUserUrlStruct();
                            item.UserId = dr["IUsrId"].ToString();
                            item.RecieveServerURL = dr["RecieveServerURL"].ToString();
                            lstUserServerUrl.Add(item);
                        }
                    }
                    finally
                    {
                        sqlConn.Close();
                    }
                }
            }
            return lstUserServerUrl;
        }

        public string GetServerUrl(string UsrId)
        {
            string Url = "";
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlDataAdapter adptr = new SqlDataAdapter())
                {
                    try
                    {
                        SqlDataReader dr;
                        adptr.SelectCommand = new SqlCommand();
                        adptr.SelectCommand.Connection = sqlConn;
                        adptr.SelectCommand.CommandType = CommandType.StoredProcedure;
                        adptr.SelectCommand.CommandText = "SpTbl_Extensible_SelectServerUrl";
                        adptr.SelectCommand.Parameters.AddWithValue("@UsrId", UsrId);
                        if (sqlConn.State != ConnectionState.Open) { sqlConn.Open(); };
                        dr = adptr.SelectCommand.ExecuteReader();
                        if (dr.Read())
                        {
                            Url = dr["ServerUrl"].ToString();
                        }
                    }
                    finally
                    {
                        sqlConn.Close();
                    }
                }
            }
            return Url;
        }

        public int CheckCredit(string Username, int SmsFaCount, int SmsEnCount)
        {
            SMSService srv = new SMSService();
            return srv.CheckCredit(SmsFaCount, SmsEnCount, Username);
        }
        public DataTable UpdateSMSStatusForGSM(string Userid, int SMSCount, string DateFrom, string DateTo, byte SMSStatus, bool IsUpdate,string Line)
        {
            DataTable data = new DataTable();
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(ConnStr))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = sqlConn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SpGetSMSForGSM";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@UsrId", Userid);
                        cmd.Parameters.AddWithValue("@SmsCount", SMSCount);
                        cmd.Parameters.AddWithValue("@from", DateFrom);
                        cmd.Parameters.AddWithValue("@to", DateTo);
                        cmd.Parameters.AddWithValue("@statusCode", SMSStatus);
                        cmd.Parameters.AddWithValue("@Line", Line);

                        if (sqlConn.State != ConnectionState.Open)
                        {
                            sqlConn.Open();
                        }
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = cmd;
                        da.Fill(data);
                        if (IsUpdate)
                        {
                            cmd.CommandText = "SpUpdateSMSForGSM";
                            cmd.ExecuteNonQuery();
                        }

                    }
                }
            }
            catch (Exception)
            {
                
               throw;
            }
            return data;
        }
        public int UpdateCreditForBlackList(string UsrId, int FaSmsCount, int EnSmsCount, double rate, byte TransActionType)
        {
            int Credit = 0;
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        SqlDataReader dr;
                        cmd.Connection = sqlConn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SpTbl_Usrcredit_IncreaseCredit";
                        cmd.Parameters.Clear();
                        string Today = Utility.PersianNowDate;
                        cmd.Parameters.AddWithValue("@Today", Today);
                        cmd.Parameters.AddWithValue("@UsrId", UsrId);
                        cmd.Parameters.AddWithValue("@FaSmsCount", FaSmsCount);
                        cmd.Parameters.AddWithValue("@EnSmsCount", EnSmsCount);
                        cmd.Parameters.AddWithValue("@Rate", rate);
                        cmd.Parameters.AddWithValue("@TrActionType", TransActionType);
                        if (sqlConn.State != ConnectionState.Open)
                        {
                            sqlConn.Open();
                        }
                        dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            int.TryParse(dr["UsrCredit"].ToString(), out Credit);
                        }
                    }
                    catch (Exception ex)
                    {
                        Credit = -1;
                        ErrorNumber = 3;// عدم به روز رسانی اعتبار
                        using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\err.txt", true))
                        {
                            outfile.Write("from MainSmsService::" + DateTime.Now.ToString() + "::" + ex.Message + "::" + ex.StackTrace + Environment.NewLine);
                        }
                        throw;
                    }
                    finally
                    {
                        sqlConn.Close();
                    }

                }
            }
            return Credit;
        }
      
        public int UpdateCreditByRate(string UsrId, int FaSmsCount, int EnSmsCount, double rate, byte TransActionType)
        {
            int Credit = 0;
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        SqlDataReader dr;
                        cmd.Connection = sqlConn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SpTbl_Usrcredit_UpdateCredit";
                        cmd.Parameters.Clear();
                        string Today = Utility.PersianNowDate;
                        cmd.Parameters.AddWithValue("@Today", Today);
                        cmd.Parameters.AddWithValue("@UsrId", UsrId);
                        cmd.Parameters.AddWithValue("@FaSmsCount", FaSmsCount);
                        cmd.Parameters.AddWithValue("@EnSmsCount", EnSmsCount);
                        cmd.Parameters.AddWithValue("@Rate", rate);
                        cmd.Parameters.AddWithValue("@TrActionType", TransActionType);
                        if (sqlConn.State != ConnectionState.Open)
                        {
                            sqlConn.Open();
                        }
                        dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            int.TryParse(dr["UsrCredit"].ToString(), out Credit);
                        }
                    }
                    catch (Exception ex)
                    {
                        Credit = -1;
                        ErrorNumber = 3;// عدم به روز رسانی اعتبار
                        using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\err.txt", true))
                        {
                            outfile.Write("from MainSmsService::" + DateTime.Now.ToString() + "::" + ex.Message + "::" + ex.StackTrace + Environment.NewLine);
                        }
                        throw;
                    }
                    finally
                    {
                        sqlConn.Close();
                    }

                }
            }
            return Credit;
        }
        public int SaveLine(string usrID, int peyment, int Rcv, string uName, string LineNumber)
        {
            int Credit = 0;
            try
            {

                using (SqlConnection sqlConn = new SqlConnection(ConnStr))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        try
                        {
                            SqlParameter parm = new SqlParameter("@rtn", SqlDbType.Int);
                            parm.Direction = ParameterDirection.ReturnValue;
                            cmd.Connection = sqlConn;
                            cmd.Parameters.Add(parm);
                            cmd.Parameters.Add(new SqlParameter("@IUsrId", usrID.Trim()));
                            cmd.Parameters.Add(new SqlParameter("@PaymentAmount", peyment));
                            cmd.Parameters.Add(new SqlParameter("@IRcvSmsPrice", Rcv));
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "SpTbl_UsrCredit_DecreaseForBuyLine";
                            sqlConn.Open();
                            cmd.ExecuteNonQuery();
                            Credit = int.Parse(parm.Value.ToString());
                            sqlConn.Close();
                        }
                        catch (Exception ex)
                        {
                            Credit = -1;
                            using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\SaveLine.txt", true))
                            {
                                outfile.Write(ex.Message + "::" + ex.StackTrace);
                            }
                        }
                        finally
                        {
                            sqlConn.Close();
                        }
                    }
                }
                //******************save line***********
                if (Credit != -1)
                {
                    try
                    {
                        linenumber line = new linenumber();
                        LineNumberService sLine = new LineNumberService();
                        ReservLineNumberService RService = new ReservLineNumberService();

                        line.IUsrId = uName;
                        if (LineNumber.Contains("+98") == false)
                            line.ILineNumber = "+98" + LineNumber;
                        else
                            line.ILineNumber = LineNumber;

                        line.ILineModuleNumber = "";
                        line.ILineServiceType = "دریافت";
                        line.ILineStatuse = "فعال";
                        line.ILineLastModify = Utility.PersianNowDate;
                        line.ILineDuration = "1";
                        sLine.Save(line);
                        ReservedLineNumbers reservedLineNumbers = new ReservedLineNumbers();
                        reservedLineNumbers = RService.UpdateStatus(LineNumber);
                        RService.AutoSave = false;
                        reservedLineNumbers.State = 0;
                        RService.Update(reservedLineNumbers);
                        RService.SaveChange();
                        sLine.SaveChange();
                    }
                    catch (Exception)
                    {

                        Credit = -1;
                    }

                }

            }

            catch (Exception ex)
            {

                // throw;
            }

            return Credit;
        }

        public int SaveBulk(SerializeBulk bulk, SerializeBulk_Request[] BulkRequests, string secureKey)
        {
            return _bulkService.SaveBulk(bulk, BulkRequests);
        }

        public DataTable GetListFreeNumber()
        {
            DataTable dtNum = new DataTable("ListNumber");

            try
            {
                SMSService srv = new SMSService();

                dtNum = srv.GetListFreeNumber();

            }
            catch (Exception)
            {

                throw;
            }

            return dtNum;
        }

        //Bulk Methods


        public SSB.Service.Core.Model.SerializeBulkState[] GetBulkState(int[] lstBulkId, string SecurKey)
        {
            return _bulkService.GetBulkState(lstBulkId);
        }

        public byte GetBulkStatus(int bulkId)
        {
            return _bulkService.GetBulkStatus(bulkId);
        }
        public bool DeleteBulk(int bulkId, string SecurKey)
        {
            return _bulkService.DeleteBulk(bulkId);
        }

        public bool UpdateBulk(SerializeBulk bulk, SerializeBulk_Request[] BulkRequests, string SecurKey)
        {
            return _bulkService.UpdateBulk(bulk, BulkRequests);
        }

        public int GetSumFaSmsCount(string userId)
        {
            SMSService _service = new SMSService();
            return _service.GetSumFaSmsCount(userId);
        }
        public int GetSumEnSmsCount(string userId)
        {
            SMSService _service = new SMSService();
            return _service.GetSumEnSmsCount(userId);
        }

        public double GetTarrifrateRate(string lineNumer)
        {
            SMSService _service = new SMSService();
            return _service.GetTarrifrateRate(lineNumer);
        }

        public int UpdateCreditForVoiceMessage(string UsrId, int FaSmsCount, int EnSmsCount, int SMSPrice, byte TrActionType)
        {
            int Credit = 0;
            using (SqlConnection sqlConn = new SqlConnection(ConnStr))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        SqlDataReader dr;
                        cmd.Connection = sqlConn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SpTbl_Usrcredit_UpdateCreditForVoiceMessage";
                        cmd.Parameters.Clear();
                        string Today = Utility.PersianNowDate;
                        cmd.Parameters.AddWithValue("@UsrId", UsrId);
                        cmd.Parameters.AddWithValue("@FaSmsCount", FaSmsCount);
                        cmd.Parameters.AddWithValue("@EnSmsCount", EnSmsCount);
                        cmd.Parameters.AddWithValue("@SMSPrice", SMSPrice);
                        cmd.Parameters.AddWithValue("@Today", Today);
                        cmd.Parameters.AddWithValue("@TrActionType", TrActionType);
                        if (sqlConn.State != ConnectionState.Open)
                        {
                            sqlConn.Open();
                        }
                        dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            int.TryParse(dr["UsrCredit"].ToString(), out Credit);
                        }
                    }
                    catch (Exception ex)
                    {
                        Credit = -1;
                        ErrorNumber = 3;// عدم به روز رسانی اعتبار
                        using (StreamWriter outfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\err.txt", true))
                        {
                            outfile.Write("from MainSmsService::" + DateTime.Now.ToString() + "::" + ex.Message + "::" + ex.StackTrace + Environment.NewLine);
                        }
                        throw;
                    }
                    finally
                    {
                        sqlConn.Close();
                    }

                }
            }
            return Credit;
        }
    }
}
