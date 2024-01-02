using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using System.Text;

using static OrderServiceGrpcServer.OrderService;

namespace OrderServiceGrpcServer.Services
{
    public class OrderServiceInstance : OrderServiceBase
    {

        /// <summary>
        /// Place Order feature
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<OrderResponse> PlaceOrder(OrderRequest request, ServerCallContext context)
        {
            RemoveItemFromList(request);

            var random = new Random();
            var response = new OrderResponse
            {
                OrderId = random.Next(10000, 100000).ToString(),
                Status = "Order Placed Successfully"
            };

            PublishEventOfOrderCreation(response);

            return Task.FromResult(response);
        }

        /// <summary>
        /// Update Order Feature
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<OrderResponse> UpdateOrder(OrderRequest request, ServerCallContext context)
        {
            var response = new OrderResponse
            {
                OrderId = request.OrderId,
                Status = request.ProductId.Equals("1") ? "Order Cancelled Successfully" : "Order Updated Successfully"
            };

            PublishEventOfOrderUpdation(response);

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


        /// <summary>
        /// Removing item from the JSON
        /// </summary>
        /// <param name="request"></param>
        private void RemoveItemFromList(OrderRequest request)
        {
            string jsonFilePath = "../OrderServiceGrpcServer/Model/itemsList.json";
            string jsonContent = File.ReadAllText(jsonFilePath);

            // Parsing JSON
            JArray productsArray = JArray.Parse(jsonContent);

            int userChoice = Convert.ToInt32(request.ProductId);

            JToken productToRemove = productsArray.FirstOrDefault(p => (int)p["id"] == userChoice);
            int chosenProductIndex = userChoice - 1;

            productsArray.Remove(productToRemove);

            // Updating the JSON file with the modified array
            File.WriteAllText(jsonFilePath, productsArray.ToString());
        }

        public void PublishEventOfOrderCreation(OrderResponse orderResponse)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declaring a Fanout Exchange
            channel.ExchangeDeclare(exchange: "fanout_exchange", type: ExchangeType.Fanout);


            string message = orderResponse.Status;

            var body = Encoding.UTF8.GetBytes(message);

            // Publishing to the Fanout Exchange
            channel.BasicPublish(exchange: "fanout_exchange",
                                    routingKey: "",
                                    basicProperties: null,
                                    body: body);

            Console.WriteLine($"Sent: {message}");
        }

        public void PublishEventOfOrderUpdation(OrderResponse orderResponse)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declaring a Topic Exchange
            channel.ExchangeDeclare(exchange: "topic_exchange", type: ExchangeType.Topic);

            string message = orderResponse.Status;

            var body = Encoding.UTF8.GetBytes(message);

            // Publishing to the Topic Exchange
            channel.BasicPublish(exchange: "topic_exchange",
                                    routingKey: "#",
                                    basicProperties: null,
                                    body: body);

            Console.WriteLine($"Sent: {message}");

        }
    }
}
