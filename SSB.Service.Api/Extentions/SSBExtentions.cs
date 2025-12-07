using SSB.Service.SSBApi.Models.ArraySendQeue;
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

        public static string[] FixPhoneNumber(this string[] numbers)
        {
            for (int i = 0; i < numbers.Count(); i++)
                numbers[i] = Helpers.Utility.FixPhoneNumber(numbers[i]);
            return numbers;
        }
        public static string GetVerbName(this string verbName)
            => verbName.Substring(verbName.LastIndexOf('/') + 1);
    }
}