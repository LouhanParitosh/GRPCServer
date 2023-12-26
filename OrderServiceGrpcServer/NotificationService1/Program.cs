using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace NotificationService1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("***********Notification Service 1*******************");
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declare a Fanout Exchange
            channel.ExchangeDeclare(exchange: "fanout_exchange", type: ExchangeType.Fanout);

            // Declare a non-durable, exclusive, auto-delete queue with a generated name
            var queueName = channel.QueueDeclare().QueueName;

            // Bind the queue to the Fanout Exchange
            channel.QueueBind(queue: queueName,
                              exchange: "fanout_exchange",
                              routingKey: "");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received: {message}");
            };

            // Start consuming messages from the queue
            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine("Press [Enter] to exit.");
            Console.ReadLine();
        }
    }
}
