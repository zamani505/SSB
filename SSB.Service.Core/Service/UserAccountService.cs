using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SSB.Core.Service;
namespace SSB.Service.Core.Service
{
    public class UserAccountService :BaseService<UserAccount>
    {
        public UserAccountService():base(new AcountingEntities())
        {
        }

        public object GetCustomeUserAccount(string ManagerName, string UserID)
        {
            try
            {
                using (AcountingEntities dc = new AcountingEntities())
                {
                    var q = dc.UserAccounts.Where(s => s.IUsrId == UserID && s.IUsrManagerId.Trim() == ManagerName.Trim())

                        .Join(dc.Customers,
                                s => s.IUsrCCode,
                                k => k.ICCode,
                                (m, n) => new { MobileNumber = n.ICMobile }
                            )
                            .Select(gg => new { MobileNumber = gg.MobileNumber })
                        //.Select(s=>s)
                            .FirstOrDefault();
                    return q;

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            //throw new NotImplementedException();
        }
        public bool CheckHasUser(string UserID)
        {
            try
            {
                using (AcountingEntities dc = new AcountingEntities())
                {
                    var q = (from a in dc.UserAccounts where a.IUsrId.Equals(UserID.Trim()) select a).FirstOrDefault();
                    if (q != null)
                        return true;
                    else return false;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            //throw new NotImplementedException();
        }

        public UserAccount Getuser(string UserID)
        {
            try
            {
                using (AcountingEntities dc = new AcountingEntities())
                {
                    var q = (from a in dc.UserAccounts where a.IUsrId.Equals(UserID.Trim()) select a).FirstOrDefault();
                    return q;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            //throw new NotImplementedException();
        }
    }
}