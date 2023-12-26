using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Newtonsoft.Json.Linq;

using static OrderServiceGrpcServer.OrderService;

namespace OrderServiceGrpcServer.Services
{
    public class OrderServiceInstance : OrderServiceBase
    {
        public override Task<OrderResponse> PlaceOrder(OrderRequest request, ServerCallContext context)
        {
            RemoveItemFromList(request);

            var random = new Random();
            var response = new OrderResponse
            {
                OrderId = random.Next(10000, 100000).ToString(),
                Status = "Order Placed Successfully"
            };

            return Task.FromResult(response);
        }

        public override Task<OrderResponse> UpdateOrder(OrderRequest request, ServerCallContext context)
        {
            var response = new OrderResponse
            {
                OrderId = request.OrderId,
                Status = request.ProductId.Equals("1") ? "Order Cancelled Successfully" : "Order Updated Successfully"
            };

            return Task.FromResult(response);
        }

        public override Task<ListOfItems> GetAllAvailableItems(Empty request, ServerCallContext context)
        {
            ListOfItems listOfItems = new ListOfItems();
            string jsonFilePath = "../OrderServiceGrpcServer/Model/itemsList.json";
            string jsonContent = File.ReadAllText(jsonFilePath);

            // Parse JSON
            JArray productsArray = JArray.Parse(jsonContent);

            foreach (JToken productToken in productsArray)
            {
               Item item = new Item
               {
                   Id = productToken["id"].ToString(),
                   Name = productToken["name"].ToString(),
                   Price = (double)productToken["price"]
               };

                listOfItems.Items.Add(item);
            }

            return Task.FromResult(listOfItems);
        }


        private void RemoveItemFromList(OrderRequest request)
        {
            string jsonFilePath = "../OrderServiceGrpcServer/Model/itemsList.json";
            string jsonContent = File.ReadAllText(jsonFilePath);

            // Parse JSON
            JArray productsArray = JArray.Parse(jsonContent);

            int userChoice = Convert.ToInt32(request.ProductId);

            JToken productToRemove = productsArray.FirstOrDefault(p => (int)p["id"] == userChoice);

            // Adjust index since user input is 1-based, but array is 0-based
            int chosenProductIndex = userChoice - 1;

            productsArray.RemoveAt(chosenProductIndex);

            // Update the JSON file with the modified array
            File.WriteAllText(jsonFilePath, productsArray.ToString());
        }
    }
}
