using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSB.Core.Service;

namespace SSB.Service.Core.Service
{
    class ReservLineNumberService : BaseService<ReservedLineNumbers>
    {
        public ReservLineNumberService()
            : base(new AcountingEntities())
        {


        }

        public ReservedLineNumbers UpdateStatus(string line)
        {
            try
            {
                using (AcountingEntities dc = new AcountingEntities())
                {
                    var q =
                        (from p in dc.ReservedLineNumbers where p.LineNumber.Equals(line.Trim() ) select p).
                            FirstOrDefault();
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
