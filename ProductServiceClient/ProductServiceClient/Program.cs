using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;

namespace ProductClient.Services
{

    class ProductClient
{
        static async Task Main(string[] args)
        {

            //Creating a gRPC Client
            using var channel = GrpcChannel.ForAddress("http://localhost:50051");
            var client = new OrderService.OrderServiceClient(channel);

            // gRPC call to get all available items from Server
            ListOfItems itemList = await client.GetAllAvailableItemsAsync(new Empty());

            //Placing the selected order
            var userResponse = RenderItemsAndBuyProduct(itemList);
            Console.WriteLine($"You have selected Product with ID: {userResponse}. Going to Checkout and place order....");
            Console.ReadLine();
            var placeorderrequest = new OrderRequest()
            {
                ProductId = userResponse
            };
            var placeOrderResponse = await client.PlaceOrderAsync(placeorderrequest);
            Console.Write(placeOrderResponse.ToString());
            Console.ReadLine();



            //Providing user option to update/Cancel the order
            Console.WriteLine("What do you want to do with this order?" + $"Order ID: {placeOrderResponse.OrderId}");
            Console.WriteLine("1. Cancel Order");
            Console.WriteLine("2. Edit Order");
            var userChoice = Console.ReadLine();

            var updateorderrequest = new OrderRequest()
            {
                ProductId = userChoice,
                OrderId = placeOrderResponse.OrderId
            };
            OrderResponse updateOrderResponse = await client.UpdateOrderAsync(updateorderrequest);
            Console.WriteLine(updateOrderResponse.ToString());
            Console.ReadLine();

        }

        /// <summary>
        /// Render the list of available items
        /// </summary>
        /// <param name="itemList"></param>
        /// <returns></returns>
        public static string RenderItemsAndBuyProduct(ListOfItems itemList)
        {
            Console.WriteLine("Choose any item from below. Select the ID to place the order!");
            // Iterate through the items in the response
            foreach (var item in itemList.Items)
            {
                Console.WriteLine($"Product ID: {item.Id}, Name: {item.Name}, Price: {item.Price}");
            }

            return Console.ReadLine();
        }

    }

}
