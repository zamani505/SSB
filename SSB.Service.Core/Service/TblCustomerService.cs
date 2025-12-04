using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSB.Core.Service;

namespace SSB.Service.Core.Service
{
    class TblCustomerService : BaseService<Customers>
    {
        public TblCustomerService()
            : base(new AcountingEntities())
        {


        }
        public bool CheckCustomer(string Mobile)
        {
            bool check = false;
            try
            {
                using (AcountingEntities dc = new AcountingEntities())
                {
                    var q = (from p in dc.Customers where p.ICMobile.Equals(Mobile) select p).FirstOrDefault();
                    if (q != null) check = true;
                }

            }
            catch (Exception)
            {

                //throw;
            }
            return check;
        }
    }
}
