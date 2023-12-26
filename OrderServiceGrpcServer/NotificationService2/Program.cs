using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace NotificationService2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("***********Notification Service 2*******************");
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declare a Fanout Exchange
            channel.ExchangeDeclare(exchange: "fanout_exchange", type: ExchangeType.Fanout);

            // Declare a Topic Exchange
            channel.ExchangeDeclare(exchange: "topic_exchange", type: ExchangeType.Topic);

            // Create a non-durable, exclusive, auto-delete queue for the Fanout Exchange
            var fanoutQueueName = channel.QueueDeclare().QueueName;

            // Bind the queue to the Fanout Exchange
            channel.QueueBind(queue: fanoutQueueName,
                              exchange: "fanout_exchange",
                              routingKey: "");

            // Create a non-durable, exclusive, auto-delete queue for the Topic Exchange
            var topicQueueName = channel.QueueDeclare().QueueName;

            // Use "#" as the routing key to match all messages in the Topic Exchange
            channel.QueueBind(queue: topicQueueName,
                              exchange: "topic_exchange",
                              routingKey: "#");

            // Create a consumer for the Fanout Exchange
            var fanoutConsumer = new EventingBasicConsumer(channel);
            fanoutConsumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received from Fanout Exchange: {message}");
            };

            // Create a consumer for the Topic Exchange
            var topicConsumer = new EventingBasicConsumer(channel);
            topicConsumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received from Topic Exchange: {message}");
            };

            // Start consuming messages from the Fanout Exchange
            channel.BasicConsume(queue: fanoutQueueName,
                                 autoAck: true,
                                 consumer: fanoutConsumer);

            // Start consuming messages from the Topic Exchange
            channel.BasicConsume(queue: topicQueueName,
                                 autoAck: true,
                                 consumer: topicConsumer);

            Console.WriteLine("Press [Enter] to exit.");
            Console.ReadLine();


        }
    }
}
