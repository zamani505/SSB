using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSB.Core.Service;
namespace SSB.Service.Core.Service
{
    public class BulkAccService : BaseService<BulkAcc>
    {
        public BulkAccService()
            : base(new AcountingEntities())
        {


        }
        public BulkAcc GetBulks(int Bulkid)
        {

            using (AcountingEntities dc = new AcountingEntities())
            {
                var bulk = dc.BulkAccs.Where(s => s.BulkId==Bulkid).FirstOrDefault();

                // usrCredit = (from a in dc.UsrCredits where a.IUsrId == usrId select a).FirstOrDefault();
                return bulk;
            }


        }
    }
}
