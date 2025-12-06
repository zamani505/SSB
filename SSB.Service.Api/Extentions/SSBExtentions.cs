using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSB.Service.SSBApi.Extentions
{
    public static class SSBExtentions
    {
        public static string CreateToken(this string username, string pass)
        => Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes($"{username}:{pass}:{Guid.NewGuid()}"));


    }
}