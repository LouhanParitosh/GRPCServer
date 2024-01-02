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

            //Declaring a Fanout Exchange
            channel.ExchangeDeclare(exchange: "fanout_exchange", type: ExchangeType.Fanout);

            // Declaring a Topic Exchange
            channel.ExchangeDeclare(exchange: "topic_exchange", type: ExchangeType.Topic);
            var fanoutQueueName = channel.QueueDeclare().QueueName;

            // Binding the queue to the Fanout Exchange
            channel.QueueBind(queue: fanoutQueueName,
                              exchange: "fanout_exchange",
                              routingKey: "");

            var topicQueueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: topicQueueName,
                              exchange: "topic_exchange",
                              routingKey: "#");

            // Creating a consumer for the Fanout Exchange
            var fanoutConsumer = new EventingBasicConsumer(channel);
            fanoutConsumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received from Fanout Exchange: {message}");
            };

            var topicConsumer = new EventingBasicConsumer(channel);
            topicConsumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received from Topic Exchange: {message}");
            };

            // Consuming messages from the Fanout Exchange
            channel.BasicConsume(queue: fanoutQueueName,
                                 autoAck: true,
                                 consumer: fanoutConsumer);

            // Consuming messages from the Topic Exchange
            channel.BasicConsume(queue: topicQueueName,
                                 autoAck: true,
                                 consumer: topicConsumer);

            Console.WriteLine("Press [Enter] to exit.");
            Console.ReadLine();


        }
    }
}
