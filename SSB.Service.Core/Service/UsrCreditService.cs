using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Infrastructure;
using SSB.Core.Service;

namespace SSB.Service.Core
{
    public class UsrCreditService : BaseService<Tbl_UsrCredit>
    {
        public UsrCreditService()
            : base(new AcountingEntities())
        {
            
        }

        public Tbl_UsrCredit GetCredit(string usrId)
        {

            using (AcountingEntities dc = new AcountingEntities())
            {
                var usrCredit = dc.Tbl_UsrCredit.Where(s => s.IUsrId.Contains(usrId)).FirstOrDefault();
                
               // usrCredit = (from a in dc.UsrCredits where a.IUsrId == usrId select a).FirstOrDefault();
                return usrCredit;
            }


        }
        public Tbl_UsrCredit GetCreditForDeleteBulk(string usrId)
        {

            using (AcountingEntities dc = new AcountingEntities())
            {
                var usrCredit = dc.Tbl_UsrCredit.Where(s => s.IUsrId.Equals(usrId)).FirstOrDefault();

                // usrCredit = (from a in dc.UsrCredits where a.IUsrId == usrId select a).FirstOrDefault();
                return usrCredit;
            }


        }
    }
}
