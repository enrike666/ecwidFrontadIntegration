using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ecwidFrontadIntegration
{
    public static class Helper
    {
        public static string ParsePhone(string phone)
        {
            long[] matches = Regex.Matches(phone, "\\d+")
                     .Cast<Match>()
                     .Select(x => long.Parse(x.Value))
                     .ToArray();

            var digitsPhone = "";
            foreach (long match in matches)
            {
                digitsPhone += match.ToString();
            }

            var resultPhone = "";
            if ((digitsPhone.Length == 10) && (digitsPhone[0] == '9'))
            {
                resultPhone = "7" + digitsPhone;
            }
            else if (digitsPhone.Length == 11)
            {
                resultPhone = "7" + digitsPhone.Substring(1);
            }
            else
            {
                //записать в лог ошибку
            }

            return resultPhone;
        }

        public static void UpdateLastOrderId(AppInfo appInfo)
        {
            var jsonNew = JsonConvert.SerializeObject(appInfo);
            File.WriteAllText("appInfo.json", jsonNew);
        }
    }
}
