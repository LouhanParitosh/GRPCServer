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
                OrderId = request.ProductId,
                Status = "Order Updated Successfully"
            };

            return Task.FromResult(response);
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
