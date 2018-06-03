using RabbitMQ.Client;
using System;
using System.Text;

namespace TaskQueuesProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskQueuesProducer.Send();

        }
    }

    public class TaskQueuesProducer
    {
        static int processId = System.Diagnostics.Process.GetCurrentProcess().Id;
        public static void Send()
        {
            Console.WriteLine($"我是生产者{processId}");
            var factory = new ConnectionFactory()
            {
                HostName = "127.0.0.1"
            };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(
                            queue: "taskqueue",
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null
                        );
                    for (int item = 0; item < 20; item++)
                    {
                        string message = $"我是生产者{processId}发送的消息:{item}";
                        channel.BasicPublish(
                            exchange: "",
                            routingKey: "taskqueue",
                            basicProperties: null,
                            body: Encoding.UTF8.GetBytes(message));
                        Console.WriteLine(message);
                    }
                    Console.WriteLine("Press [ENTER] to exit");
                    Console.ReadLine();
                }
            }
        }
    }
}
