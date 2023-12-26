using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Newtonsoft.Json.Linq;
using System.Net;

namespace ProductClient.Services {

class ProductClient
{
        static async Task Main(string[] args)
        {
            //Place Order Call
            using var channel = GrpcChannel.ForAddress("http://localhost:50051");
            var client = new OrderService.OrderServiceClient(channel);

            JObject chosenProduct = ReadFromItemList();

            var placeorderrequest = new OrderRequest()
            {
                ProductId = chosenProduct["id"].ToString()
            };
            OrderResponse response = await client.PlaceOrderAsync(placeorderrequest);
            Console.Write(response.ToString());
            Console.ReadLine();

            Console.WriteLine("What do you want to do with this order?" + $"Order ID: {response.OrderId}");
            Console.WriteLine("1. Cancel Order");
            Console.WriteLine("2. Edit Order");

            int userChoice;

            // Validate user input
            do
            {
                Console.Write("Enter the number of your choice: ");

                // Try to parse the user input as an integer
                if (!int.TryParse(Console.ReadLine(), out userChoice) || (userChoice != 1 && userChoice != 2))
                {
                    Console.WriteLine("Invalid input. Please enter 1 or 2.");
                }

            } while (userChoice != 1 && userChoice != 2);

            OrderResponse updateOrderResponse = await client.UpdateOrderAsync(placeorderrequest);
            Console.WriteLine(updateOrderResponse.ToString());
            Console.ReadLine();

        }

        private static JObject ReadFromItemList()
        {
            string jsonFilePath = "../ProductServiceClient/Model/itemsList.json";

            string jsonContent = File.ReadAllText(jsonFilePath);

            // Parse JSON
            JArray productsArray = JArray.Parse(jsonContent);
            Console.WriteLine("Choose a product:");

            // Display a numbered list of products
            for (int i = 0; i < productsArray.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {productsArray[i]["name"]} - ${productsArray[i]["price"]}");
            }

            int userChoice;

            // Validate user input
            do
            {
                Console.Write("Enter the number of your choice: ");

                // Try to parse the user input as an integer
                if (!int.TryParse(Console.ReadLine(), out userChoice) || userChoice < 1 || userChoice > productsArray.Count)
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }

            } while (userChoice < 1 || userChoice > productsArray.Count);

            // Adjust index since user input is 1-based, but array is 0-based
            int chosenProductIndex = userChoice - 1;

            // Get the chosen product
            JObject chosenProduct = (JObject)productsArray[chosenProductIndex];

            // Display the details of the chosen product
            Console.WriteLine($"You have chosen: {chosenProduct["name"]} - ${chosenProduct["price"]}");
            Console.WriteLine("Additional Details:");

            productsArray.RemoveAt(chosenProductIndex);

            // Update the JSON file with the modified array
            File.WriteAllText(jsonFilePath, productsArray.ToString());

            return chosenProduct;

        }

    }

}
