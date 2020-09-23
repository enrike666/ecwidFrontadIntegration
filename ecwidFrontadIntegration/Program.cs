using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ecwidFrontadIntegration
{
    class Program
    {
        static void Main(string[] args)
        {      
            Logger logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            var httpClient = new HttpClient();

            string jsonSettings = File.ReadAllText("appSettings.json");
            AppSettings appSettings = JsonConvert.DeserializeObject<AppSettings>(jsonSettings);

            var processSetting = new ProcessSetting(appSettings, logger, httpClient);

            while(true)
            {
                Process(processSetting);                
                Thread.Sleep(900000);
            }
        }

        public static void Process(object processSetting)
        {
            var ps =(ProcessSetting)processSetting;
            var result = ps.HttpClient.GetAsync("https://app.ecwid.com/api/v3/11719420/orders?token=" + ps.AppSettings.TokenEcwid).Result;
            var jsonResult = result.Content.ReadAsStringAsync().Result;
            if (result.StatusCode != HttpStatusCode.OK)
            {
                ps.Logger.Error("Не удалось запросить заказы из Ecwid. StatusCode: " + result.StatusCode);
                
            } else
            {
                Root root = JsonConvert.DeserializeObject<Root>(jsonResult);

                string jsonInf = File.ReadAllText("appInfo.json");
                AppInfo appInfo = JsonConvert.DeserializeObject<AppInfo>(jsonInf);

                foreach (EcwidOrder ecwidOrder in root.EcwidOrders)
                {
                    if (ecwidOrder.OrderNumber > appInfo.LastOrderId)
                    {
                        var frontpadOrder = ecwidOrder.ConvertToFrontpadOrder();
                        frontpadOrder.Secret = ps.AppSettings.SecretFrontpad;

                        var frontpadResponseBody = Helper.CreateFrontpadResponseDicDictionary(frontpadOrder, ecwidOrder.Products);

                        var content = new FormUrlEncodedContent(frontpadResponseBody);
                        content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
                        var response = ps.HttpClient.PostAsync("https://app.frontpad.ru/api/index.php?new_order", content).Result;

                        var bodyResponse = response.Content.ReadAsStringAsync().Result;

                        FrontPadResponse frontPadResponse = JsonConvert.DeserializeObject<FrontPadResponse>(bodyResponse);
                        if (frontPadResponse.Result == "success")
                        {
                            appInfo.LastOrderId = ecwidOrder.OrderNumber;
                            Helper.UpdateLastOrderId(appInfo);
                            ps.Logger.Info("Заказ orderNumber = " + ecwidOrder.OrderNumber + " Передан в FrontPad c order_number = " + frontPadResponse.OrderNumber);
                        }
                        else if (frontPadResponse.Result == "error")
                        {
                            ps.Logger.Error("Заказ orderNumber = " + ecwidOrder.OrderNumber + " Ошибка от FrontPad: " + frontPadResponse.Error);
                            continue;
                        }
                    }
                }

                ps.Logger.Info("Заказы за 15 минут обработаны");
            } 
        }
    }

    public class FrontPadResponse
    {
        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("order_id")]
        public string OrderId { get; set; }

        [JsonProperty("order_number")]
        public string OrderNumber { get; set; }      
    }

    public class AppSettings
    {
        [JsonProperty("privateTokenEcwid")]
        public string TokenEcwid { get; set; }

        [JsonProperty("secretKeyFrontpad")]
        public string SecretFrontpad { get; set; }
    }

    public class AppInfo
    {
        [JsonProperty("lastOrderId")]
        public int LastOrderId { get; set; }
    }

    public class ProcessSetting
    {
        public HttpClient HttpClient { get; set; }
        public Logger Logger { get; set; }
        public AppSettings AppSettings { get; set; }

        public ProcessSetting(AppSettings appSettings, Logger logger, HttpClient httpClient)
        {
            HttpClient = httpClient;
            Logger = logger;
            AppSettings = appSettings;
        }
    }
}
