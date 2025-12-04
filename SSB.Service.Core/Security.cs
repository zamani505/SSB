using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSB.Service.Core.Service;

namespace SSB.Service.Core
{
    public class Security
    {
        public bool CheckSecurity(string securKey)
        {
            if (securKey != null && securKey != "")
            {
                string hd = SSB.Helpers.BaseSecurity.Decrypt(securKey);
                string userId = hd.Split(':').GetValue(0).ToString();
                UserAccountService uas = new UserAccountService();
                SMSService ss = new SMSService();
                if (ss.GetUserAccountWithUserId(userId) == false)
                    return false;
                else
                {
                    long tmpLong;
                    long.TryParse(hd.Split(':').GetValue(1).ToString(), out tmpLong);
                    DateTime time = new DateTime(tmpLong);
                    if (time > DateTime.Now.AddMinutes(5) || time < DateTime.Now.AddMinutes(-5))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
                return false;
        }
    }
}
