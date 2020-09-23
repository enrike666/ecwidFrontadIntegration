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

        public static Dictionary<string, string> CreateFrontpadResponseDicDictionary(FrontpadOrder frontpadOrder, List<Product> items)
        {            
            Dictionary<string, string> frontpadResponseBody = new Dictionary<string, string>();

            frontpadResponseBody.Add("secret", frontpadOrder.Secret);
            frontpadResponseBody.Add("name", frontpadOrder.ClientName);
            frontpadResponseBody.Add("phone", frontpadOrder.ClientPhone);
            frontpadResponseBody.Add("mail", frontpadOrder.Email);
            frontpadResponseBody.Add("street", frontpadOrder.FullAddress);
            frontpadResponseBody.Add("descr", frontpadOrder.Description);

            for (int i = 0; i < items.Count; i++)
            {
                frontpadResponseBody.Add("product[" + i + "]", items[i].Id);
                frontpadResponseBody.Add("product_kol[" + i + "]", items[i].Quantity);
            }

            return frontpadResponseBody;
        }
    }
}
