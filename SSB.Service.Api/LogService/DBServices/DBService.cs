using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
namespace SSB.Service.SSBApi.LogService
{
    public class DBService
    {
        #region props
        public static string _connectionString = ConfigurationManager.AppSettings["LogConnection"].ToString();
        public static byte _auditLog = byte.Parse(ConfigurationManager.AppSettings["AuditLog"] ?? "1");
        public static byte _auditLogRequest = byte.Parse(ConfigurationManager.AppSettings["AuditLogRequest"] ?? "1");
        public static byte _auditLogRespnse = byte.Parse(ConfigurationManager.AppSettings["AuditLogResponse"] ?? "1");
        public SqlCommand _sqlCommand;
        public SqlConnection _connection;
        #endregion
        #region ctors
        public DBService()
        {

        }
        #endregion
        #region public methods
        public long NewLog(SMSAuditLog log)
        {
            long id = 0;
            if (_auditLog == 0) return 0;
            if (_auditLogRequest == 0) log.Request = "";
            using (_sqlCommand = new SqlCommand())
            {
                using (_connection = new SqlConnection(_connectionString))
                {
                    Open();
                    _sqlCommand.Connection = _connection;
                    _sqlCommand.CommandType = CommandType.Text;
                    _sqlCommand.CommandText = $"insert into SMSAuditLog(Username,ServiceName,Request,CreateTime,Ip)Values('{log.Username}','{log.ServiceName}','{log.Request}','{log.CreateTime},{log.Ip}')";
                    var result = _sqlCommand.ExecuteScalar();
                    id = result != null ? Convert.ToInt64(result) : -1;
                    Close();
                }
            }
            return id;
        }
        public void UpdateLog(SMSAuditLog log)
        {
            if (_auditLog == 0) return ;
            if (_auditLogRespnse == 0) log.Response = "";
            using (_sqlCommand = new SqlCommand())
            {
                using (_connection = new SqlConnection(_connectionString))
                {
                    Open();
                    _sqlCommand.Connection = _connection;
                    _sqlCommand.CommandType = CommandType.Text;
                    _sqlCommand.CommandText = $"update SMSAuditLog set Response='{log.Response}',ExcutionTime={log.ExcutionTime},Exception='{log.Exception}' where Id={log.Id}";
                    _sqlCommand.ExecuteScalar();
                    Close();
                }
            }
        }
        #endregion
        #region private methods
        private void Open()
        {
            if (_connection.State == System.Data.ConnectionState.Closed)
                _connection.Open();
        }
        private void Close()
            => _connection.Close();
        #endregion
    }
}