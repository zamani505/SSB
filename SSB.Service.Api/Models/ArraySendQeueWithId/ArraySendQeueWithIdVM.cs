using System;

namespace SSB.Service.SSBApi.Models.ArraySendQeueWithId
{
    public class ArraySendQeueWithIdVM
    {
        public string[] Messages { get; set; }
        public string[] Mobiles { get; set; }
        public string[] SenderNumbers { get; set; }
        public Guid[] Ids { get; set; }
    }
}