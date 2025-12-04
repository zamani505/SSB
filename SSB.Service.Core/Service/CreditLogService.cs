using SSB.Core.Service;

namespace SSB.Service.Core.Service
{
    public class CreditLogService : BaseService<CreditLog>
    {
        public CreditLogService():base(new AcountingEntities())
        {
        }
    }
}
