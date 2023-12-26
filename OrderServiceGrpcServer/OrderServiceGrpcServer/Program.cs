using Grpc.Core;
using OrderServiceGrpcServer.Services;

namespace OrderServiceGrpcServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Server server = new Server
            {

                Services = { OrderService.BindService(new OrderServiceInstance()) },
                Ports = { new ServerPort("localhost", 50051, ServerCredentials.Insecure) }
            };

            server.Start();
            Console.WriteLine("Order Service listening on port 50051");
            Console.ReadLine();

            server.ShutdownAsync().Wait();
        }
    }
}