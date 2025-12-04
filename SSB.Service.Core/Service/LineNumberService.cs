using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSB.Core.Service;
namespace SSB.Service.Core.Service
{
    class LineNumberService:BaseService<linenumber>
    {
        public LineNumberService()
            : base(new AcountingEntities())
        {
        }

    }
}
