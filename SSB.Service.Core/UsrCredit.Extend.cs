using System;
namespace SSB.Service.Core
{
    public partial class UsrCredit : ICloneable
    {
        public object Clone()
        {
            return base.MemberwiseClone();
        }

    }
}