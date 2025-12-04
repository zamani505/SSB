using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSB.Core.Service;
namespace SSB.Service.Core.Service
{
    public class BulkRequstAccService : BaseService<Bulk_RequestAcc>
    {
        public BulkRequstAccService()
            : base(new AcountingEntities())
        {


        }
        public List<Bulk_RequestAcc> GetBulksrequset(int Bulkid)
        {

            using (AcountingEntities dc = new AcountingEntities())
            {
                List<Bulk_RequestAcc> lstBulk_Request = new List<Bulk_RequestAcc>();
                 lstBulk_Request = dc.Bulk_RequestAcc.Where(s => s.BulkId == Bulkid).ToList();

                // usrCredit = (from a in dc.UsrCredits where a.IUsrId == usrId select a).FirstOrDefault();
                return lstBulk_Request;
            }


        }
    }
}
