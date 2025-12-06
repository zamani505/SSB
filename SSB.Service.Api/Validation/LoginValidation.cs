
using System.Collections.Generic;
using System.Web.Services.Description;
using System;
using SSB.Service.Web.avanak;
using SSB.Service.Core;
using System.Linq;
using UserInfo = SSB.Service.Core.UserInfo;

namespace SSB.Service.SSBApi.Validation
{
    public class LoginValidation
    {
        #region props
        SMSService _service;
        #endregion
        #region ctors
        public LoginValidation()
        {
            _service = new SMSService();
        }
        #endregion
        #region public methods
        public string Validate(string username, string password) {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
               return " نام کاربری و رمز عبور داده نشده است.";
            List<UserInfo> info = _service.Authenticate(username, password).ToList();
            if (info == null || info.Count == 0)
                return  "18" ;
            if (!info[0].IUsrActive.GetValueOrDefault(false))
                return  "16" ;
            return "";
        }
        #endregion
    }
}