using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ecwidFrontadIntegration
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Root
    {       
        [JsonProperty("count")]
        public int Count { get; set; }
       
        [JsonProperty("items")]
        public List<EcwidOrder> EcwidOrders { get; set; }
    }

    public class ClientInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }
    }

    public class Product
    {
        [JsonProperty("productId")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("quantity")] 
        public string Quantity { get; set; }
    }

    public class EcwidOrder
    {
        [JsonProperty("orderNumber")]
        public int OrderNumber { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("shippingPerson")]
        public ClientInfo ClientInfo { get; set; }

        [JsonProperty("items")]
        public List<Product> Products { get; set; }

        public FrontpadOrder ConvertToFrontpadOrder()
        {
            List<string> productsId = new List<string>();
            List<string> productsNumber = new List<string>();

            getProductIdAndNumberArrays(Products, ref productsId, ref productsNumber);

            string phone = Helper.ParsePhone(ClientInfo.Phone);
            var frontpadOrder = new FrontpadOrder
            {
                Secret = "", //устанавливается после выполнения метода уровнем выше
                ClientName = ClientInfo.Name,
                ClientPhone = phone,
                ProductsId = productsId,
                ProductsNumber = productsNumber,
                Email = Email,
                FullAddress = ClientInfo.City + ", " + ClientInfo.Street,
                Description = OrderNumber.ToString()
            };

            return frontpadOrder;
        }

        static private void getProductIdAndNumberArrays(List<Product> products, ref List<string> productsId, ref List<string> productsNumber)
        {            
            foreach (Product product in products)
            {
                productsId.Add(product.Id);
                productsNumber.Add(product.Quantity);
            }
        }
    }

    
}
