using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace TaskQueuesConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskQueuesConsumer.Recevie();
        }
    }

    public class TaskQueuesConsumer
    {
        static int processId = 0;
        static TaskQueuesConsumer()
        {
            processId = System.Diagnostics.Process.GetCurrentProcess().Id;

        }

        public static void Recevie()
        {
            Console.WriteLine($"我是消费者{processId}");
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
                    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                    consumer.Received += Consumer_Received;
                    //noack=false 不自动消息确认 这时需要手动调用   channel.BasicAck(); 进行消息确认
                    //noack=true 自动消息确认，当消息被RabbitMQ发送给消费者（consumers）之后，马上就会在内存中移除
                    channel.BasicConsume(queue: "taskqueue", autoAck: true, consumer: consumer);
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }

        public static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Body);
            Console.WriteLine($"接收到消息:{message}");
            //Thread.Sleep(new Random().Next(1000, 1000 * 5)); //模拟消息处理耗时操作
            Console.WriteLine($"已处理完消息");
            //对应前面的 BasicConsume 中 noack=false 发送消息确认回执
            //EventingBasicConsumer consumer = sender as EventingBasicConsumer;
            //consumer.Model.BasicAck(e.DeliveryTag, false);

        }
    }
}
