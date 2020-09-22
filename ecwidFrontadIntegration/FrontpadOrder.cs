using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ecwidFrontadIntegration
{
    public class FrontpadOrder
    {
        [JsonProperty("secret")]
        public string Secret { get; set; }

        [JsonProperty("product")]
        public List<string> ProductsId { get; set; }

        [JsonProperty("product_kol")]
        public List<string> ProductsNumber { get; set; }

        [JsonProperty("name")]
        public string ClientName { get; set; }

        [JsonProperty("phone")]
        public string ClientPhone { get; set; }

        [JsonProperty("mail")]
        public string Email { get; set; }
        
        [JsonProperty("street")] 
        public string FullAddress { get; set; }

        [JsonProperty("descr")]
        public string Description { get; set; }        
    }
}
