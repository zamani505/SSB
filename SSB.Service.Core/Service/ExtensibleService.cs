using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSB.Core.Service;
namespace SSB.Service.Core
{
    public class ExtensibleService : BaseService<tbl_Extensible>
    {
        public ExtensibleService()
            : base(new AcountingEntities())
        {
        }
        public void Insert(tbl_Extensible tx)
        {
            using (AcountingEntities db = new AcountingEntities())
            {
                db.tbl_Extensible.Add(tx);
                db.SaveChanges();
            }
        }
    }
}