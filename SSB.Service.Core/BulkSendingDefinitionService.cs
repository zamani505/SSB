using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSB.Core.Service;
namespace SSB.Service.Core
{
   public  class BulkSendingDefinitionService :BaseService<UserAccount>
    {

        public BulkSendingDefinitionService()
            : base(new AcountingEntities())
        {
        }
        
        public List<Tbl_BulkSendingDefinition> GetAll()
        {
            List<Tbl_BulkSendingDefinition> list = null;
            using (AcountingEntities dc = new AcountingEntities())
            {
                list = (from s in dc.Tbl_BulkSendingDefinition
                                        select s).ToList();
            }
            return list;
        }
        public List<Tbl_BulkSendingDefinition> GetAllByUser(string userid)
        {
            List<Tbl_BulkSendingDefinition> list = null;
            using (AcountingEntities dc = new AcountingEntities())
            {
                list = (from s in dc.Tbl_BulkSendingDefinition where s.IUserID==userid.Trim()
                        select s).ToList();
            }
            return list;
        }
    }
}
